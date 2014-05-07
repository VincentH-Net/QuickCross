$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function ReplaceStringsInString
{
    Param(
        [Parameter(Mandatory=$true)] [string]$text,
        [Hashtable]$replacements
    )

    if ($replacements -ne $null) 
    {
        foreach ($replacement in $replacements.GetEnumerator())
        {
            $text = $text -replace $replacement.Name, $replacement.Value
        }
    }
    $text
}

function ReplaceStringsInFile
{
    Param(
        [Parameter(Mandatory=$true)] [string]$filePath,
        [Hashtable]$replacements
    )

    if ($replacements -ne $null) 
    {
        $content = [System.IO.File]::ReadAllText($filePath)
        $content = ReplaceStringsInString -text $content -replacements $replacements
        [System.IO.File]::WriteAllText($filePath, $content, [System.Text.Encoding]::UTF8)
    }
}

function GetProjectFolderItem
{
    Param(
        [Parameter(Mandatory=$true)] $projectOrItem,
        [Parameter(Mandatory=$true)] [string]$relativePath
    )

    $relativePath = $relativePath.Trim('\')
    if (($relativePath.Length -eq 0) -or ($projectOrItem -eq $null)) { return $projectOrItem }
    $subFolder = $relativePath.Split('\')[0]
    $subItem = $projectOrItem.ProjectItems.Item($subFolder)
    $subPath = $relativePath.   ### TODO HERE
    return (GetProjectFolderItem -projectOrItem $item -relativePath 
    if (($item -eq $null) -or ()) { return $item }

    $item
}

function AddProjectItem
{
    Param(
        [Parameter(Mandatory=$true)] $project,
        [Parameter(Mandatory=$true)] [string]$destinationProjectRelativePath,
        [Parameter(Mandatory=$true)] [string]$templatePackageFolder,
        [Parameter(Mandatory=$true)] [string]$templateProjectRelativePath,
        [Hashtable]$contentReplacements,
        [switch]$isOptionalItem
    )

    $projectFolder = Split-Path -Path $project.FullName -Parent

    $templatePath = Join-Path -Path $projectFolder -ChildPath $templateProjectRelativePath
    if (-not(Test-Path $templatePath))
    {
        $toolsPath = $PSScriptRoot
        $templatePath = Join-Path -Path $toolsPath -ChildPath "$templatePackageFolder\$templateProjectRelativePath"
        if (-not(Test-Path $templatePath)) 
        { 
            if ($isOptionalItem) { return $false } else { throw "Template file not found: $templatePath" }
        }
    }

    $destinationPath = Join-Path -Path $projectFolder -ChildPath $destinationProjectRelativePath
    $destinationFolder = Split-Path -Path $destinationPath -Parent
    if (-not(Test-Path -Path $destinationFolder)) { 
        $null = $project.ProjectItems.AddFolder($destinationProjectRelativePath, '')
        # $null = New-Item $destinationFolder -ItemType Directory -Force 
    }

    if (Test-Path -Path $destinationPath) { Write-Host ('NOT adding project item because it already exists: {0}' -f (SolutionRelativePath -path $destinationPath)); return $false }
    Write-Host ('Adding project item: {0}' -f (SolutionRelativePath -path $destinationPath))
    Copy-Item -Path $templatePath -Destination $destinationPath -Force
    ReplaceStringsInFile -filePath $destinationPath -replacements $contentReplacements

    $null = $project.ProjectItems.AddFromFile($destinationPath)
    $null = $dte.ItemOperations.OpenFile($destinationPath)
    return $true
}

function SolutionRelativePath
{
    Param([Parameter(Mandatory=$true)] [string]$path)

    $solutionFolder = (Split-Path -Path $dte.Solution.FullName -Parent) + '\'
    if ($path.StartsWith($solutionFolder)) { return $path.Substring($solutionFolder.Length) }
    return $path
}

function AddProjectItemsFromDirectory
{
    Param(
        [Parameter(Mandatory=$true)] $project,
        [Parameter(Mandatory=$true)] [string]$sourceDirectory,
        [string]$destinationDirectory,
        [Hashtable]$nameReplacements,
        [Hashtable]$contentReplacements
    )

    if (-not(Test-Path -Path $sourceDirectory)) { return }
    if ("$destinationDirectory" -eq '') { $destinationDirectory = Split-Path -Path $project.FullName -Parent }

    Get-ChildItem $sourceDirectory | ForEach-Object {
        $itemName = ReplaceStringsInString -text $_.Name -replacements $nameReplacements
        $destinationPath = Join-Path -Path $destinationDirectory -ChildPath $itemName
        $destinationPathExists = Test-Path -Path $destinationPath
        if ($_.PSIsContainer)
        {
            if (-not $destinationPathExists) { $null = New-Item -Path $destinationPath -ItemType directory }
            AddProjectItemsFromDirectory -project $project -sourceDirectory $_.FullName -destinationDirectory $destinationPath -nameReplacements $nameReplacements -contentReplacements $contentReplacements
        } else {
            if ($destinationPathExists) {
                Write-Host ('NOT adding project item because it already exists: {0}' -f (SolutionRelativePath -path $destinationPath))
            } else {
                Copy-Item -Path $_.FullName -Destination $destinationPath -Force
                ReplaceStringsInFile -filePath $destinationPath -replacements $contentReplacements
            }
            $null = $project.ProjectItems.AddFromFile($destinationPath)
        }
    }
}

function EnsureConditionalCompilationSymbol
{
    Param(
        [Parameter(Mandatory=$true)] $project,
        [Parameter(Mandatory=$true)] [string]$define
    )

    if ($define -ne $null)
    {
        # Add the #define for the target framework, if needed.
        Write-Host "Ensuring $define conditional compilation symbol for all $($project.Name) project configurations and platforms"
        $project.ConfigurationManager.ConfigurationRowNames | foreach-object {
            $project.ConfigurationManager.ConfigurationRow($_) | foreach-object { 
                $property = $_.Properties.Item('DefineConstants')
                if ($property -ne $null)
                {
                    $value = "$($property.value)".Trim()
                    if ($value -notmatch "\W*$define\W*") {
                        if ($value -ne '') { $value += ';' }
                        $value += $define
                        $property.value = $value
                    }
                }
            } 
        }
    }
}

function AddCsCodeFromInlineTemplate
{
    Param(
        [Parameter(Mandatory=$true)] [string]$csCode,
        [Parameter(Mandatory=$true)] [string]$templateName,
        [Hashtable]$replacements
    )

    # Format of an inline code template (first is an empty line):

    #
    # /* TODO: For each $templateName, any text
    # * optional comment lines
    # template lines
    # * optional comment lines
    # */

    $match = [regex]::Match($csCode, "(?m)\r\n[ \t]*\r\n[ \t]*/\*[ \t]*TODO:[ \t]+For[ \t]+each[ \t]+$templateName,[^\r\n]*\r\n([ \t]*\*[ \t][^\r\n]*\r\n)*(?<Template>([ \t]*([^\* \t\r\n][^\r\n]*)?\r\n)+)([ \t]*\*[ \t][^\r\n]*\r\n)*[ \t]*(\*[ \t][^\r\n]*)?\*/")
    if (-not ($match.Success)) { return $null }
    $captures = $match.Captures
    if (($captures -eq $null) -or ($captures.Count -eq 0)) { return $null }

    for ($i = $captures.Count - 1; $i -ge 0; $i--) # Iterate backwards through the text to keep unprocessed match positions valid while we insert text
    {
        $capture = $captures.Item($i)
        $insertPosition = $capture.Index + 2 # Insert right after the starting newline of the match
        $capture.Groups['Template'].Captures | ForEach-Object {
            $template = $_.Value
            $newCode = ReplaceStringsInString -text $template -replacements $replacements

            # Check if the new code already exists:
            if ($newCode -match '(?m)\s*(?<Id>[\w][\w\t ]*)')
            {
                $id = $Matches.Id
                if ($csCode -match ('(?m)\b{0}\b' -f $id)) {
                    Write-Host ('NOT adding {0} ... code because it is already present' -f $id)
                    continue
                }
                Write-Host ('    {0} ...' -f $id)
            } else {
                Write-Host $newCode
            }

            $csCode = $csCode.Insert($insertPosition, $newCode)
        }
    }

    $csCode
}

function AddCsCodeFromInlineTemplateInVsEditor
{
    Param(
        [Parameter(Mandatory=$true)] [string]$itemPath,
        [Parameter(Mandatory=$true)] [string]$templateName,
        [Hashtable]$replacements
    )

    if (-not (Test-Path $itemPath)) {
        Write-Host ('NOT adding {0} code to file because file does not exist: {1}' -f $templateName, (SolutionRelativePath -path $itemPath))
        return
    }

    $window = $dte.ItemOperations.OpenFile($itemPath)
    $document = $window.Document
    $textDocument = $document.Object('TextDocument')
    $editPoint = $textDocument.StartPoint.CreateEditPoint()
    $csCode = $editPoint.GetText($textDocument.EndPoint)
    Write-Host ('Adding {0} code to file {1} ...' -f $templateName, (SolutionRelativePath -path $itemPath))
    $csCode = AddCsCodeFromInlineTemplate -csCode $csCode -templateName $templateName -replacements $replacements
    if ($csCode -eq $null) {
        Write-Host ('NOT added {0} code to file because the file does not contain an inline template that matches "/* TODO: For each {0}, ... */" : {1}' -f $templateName, (SolutionRelativePath -path $itemPath))
        return
    }
    $editPoint.ReplaceText($textDocument.EndPoint, $csCode, 1) # 1 = vsEPReplaceTextKeepMarkers
}

function GetContentReplacements
{
    Param(
        [Parameter(Mandatory=$true)] $project,
        [switch]$cs,
        [switch]$isApplication
    )

    if ($cs) {
        $defaultNamespace = $project.Properties.Item("DefaultNamespace").Value
        $contentReplacements = @{
            'QuickCross.Templates' = $defaultNamespace;
            '(?m)^\s*#if\s+TEMPLATE\s+[^\r\n]*[\r\n]+' = '';
            '(?m)^\s*#endif\s+//\s*TEMPLATE[^\r\n]*[\r\n]*' = ''
        } 
        if ($isApplication) {
            $libraryProject = $script:sharedCodeProjects[0]
            $libraryDefaultNamespace = $libraryProject.Properties.Item("DefaultNamespace").Value
            $contentReplacements.Add('QuickCrossLibrary\.Templates', $libraryDefaultNamespace)
            $libraryAssemblyName =  $libraryProject.Properties.Item("AssemblyName").Value
            $contentReplacements.Add('QuickCrossLibraryAssembly', $libraryAssemblyName)
        }
    } else {
        $contentReplacements = @{ }
    }

    $solutionName = Split-Path ($project.DTE.Solution.FullName) -Leaf
    $appName = $solutionName.Split('.')[0]
    $contentReplacements.Add('_APPNAME_', $appName)

    $contentReplacements
}

$vsProjectKindSolutionFolder = '{66A26720-8FB5-11D2-AA7E-00C04F688DDE}' # ProjectKinds.vsProjectKindSolutionFolder

function GetSubProjects
{
    Param([Parameter(Mandatory=$true)] $solutionFolderProject)

	$projects = @()

    foreach ($item in $solutionFolderProject.ProjectItems) {
        $project = $item.SubProject
		if ($project -ne $null) {
			if ($project.Kind -eq $vsProjectKindSolutionFolder) {
				$projects += GetSubProjects -solutionFolderProject $project
			} else {
				$projects += $project
			}
		}
    }

	$projects
}

function GetAllProjects
{
	$projects = @()

    foreach ($project in $dte.Solution.Projects)
    {
		if ($project -ne $null) {
			if ($project.Kind -eq $vsProjectKindSolutionFolder) {
				$projects += GetSubProjects -solutionFolderProject $project
			} else {
				$projects += $project
			}
		}
    }

    $projects
}

function GetProjectPlatform
{
    Param([Parameter(Mandatory=$true)] $project, [switch]$allowUnknown)

    try   { $targetFrameworkMoniker = $project.Properties.Item("TargetFrameworkMoniker").Value } 
    catch { $targetFrameworkMoniker = 'none' }
    # E.g. valid target framework monikers are:
    # Windows Store:   .NETCore,Version=v4.5
    # Windows Phone:   WindowsPhone,Version=v8.0
    # Xamarin.Android: MonoAndroid,Version=v4.2
    # Xamarin.iOS:     MonoTouch,Version=v1.0

    $targetFrameworkName = $targetFrameworkMoniker.Split(',')[0]
    switch ($targetFrameworkName)
    {
        'MonoAndroid'     { $platform = 'android' }
        'MonoTouch'       { $platform = 'ios'     }
        'WindowsPhone'    { $platform = 'wp'      } # Windows Phone 8 Silverlight
        '.NETCore'        { $platform = 'ws'      } # Windows Store 8
        'WindowsPhoneApp' { $platform = 'wpa'     } # Windows Phone 8.1
        '.NETPortable'    { $platform = 'pcl'     } # Portable Class Library
        'none'            { $platform = 'none'    } # Miscellaneous Files
        default {
            if ($allowUnknown) { $platform = '' } else { throw "Unsupported target framework: " + $targetFrameworkName } 
        }
    }

    $platform
}

function GetProjectType
{
    Param([Parameter(Mandatory=$true)] $project)

	$projectType = 'other'

	if ("$($project.FullName)" -ne '') {
		if ($project.FullName.EndsWith('.shproj')) {
			$projectType = 'shared'
		} else {
			$projectFileContent = [System.IO.File]::ReadAllText($project.FullName)
			$platform = GetProjectPlatform -project $project
			switch ($platform) {
				'android' { $isApplication = $projectFileContent -match '<\s*AndroidApplication\s*>\s*true\s*</\s*AndroidApplication\s*>' }
				'ios'     { $isApplication = $projectFileContent -match '<\s*OutputType\s*>\s*Exe\s*</\s*OutputType\s*>' }
				'wp'      { $isApplication = $projectFileContent -match '<\s*SilverlightApplication\s*>\s*true\s*</\s*SilverlightApplication\s*>' }
				'ws'      { $isApplication = $projectFileContent -match '<\s*OutputType\s*>\s*AppContainerExe\s*</\s*OutputType\s*>' }
				'wpa'     { $isApplication = $projectFileContent -match '<\s*OutputType\s*>\s*AppContainerExe\s*</\s*OutputType\s*>' }
				'pcl'     { $isApplication = $null }
				'none'    { $isApplication = $null }
				default   { throw "Unknown platform: " + $platform }
			}
			if ($isApplication -ne $null) {
				if ($isApplication) { $projectType = 'application' } else { $projectType = 'shared' }
			}
		}
	}

    $projectType
}

$sharedCodeProjects = @()
$applicationProjects = @()
$otherProjects = @()

function InitializeProjects
{
    Param(
       [string]$ProjectName
    )

    if ("$ProjectName" -eq '') {
        $projects = GetAllProjects
    } else {
        $project = Get-Project $ProjectName
        if ($project -eq $null)  { Write-Host "Project '$ProjectName' not found in solution."; return }
        if ((GetProjectType -project $project) -eq 'other') { Write-Host "Project '$ProjectName' is not an application or shared code project. Please specify an application project, a shared code project for all target platforms, or a class library project for a single target platform."; return }
        $projects = @($project);
    }

    $script:sharedCodeProjects = @()
    $script:applicationProjects = @()
    $script:otherProjects = @()
    foreach ($project in $projects)
    {
        switch ((GetProjectType -project $project))
        {
            'shared'      { $script:sharedCodeProjects  += $project }
            'application' { $script:applicationProjects += $project }
            'other'       { $script:otherProjects       += $project }
        }
    }

    if (("$ProjectName" -eq '') -and ($script:sharedCodeProjects.Count -eq 0)) {
        Write-Host "No shared code project found to install the QuickCross shared code into. Please add either a shared code project for all target platforms, or a class library project for a single target platform to the solution."
        return $false
    }
    if ($script:sharedCodeProjects.Count -gt 1) {
        Write-Host "NOT installing the QuickCross shared code because more than one shared code project was found. Please use the ProjectName parameter to specify in which one of these projects the QuickCross shared code should be installed:"
        Write-Host ($script:sharedCodeProjects | Select -ExpandProperty Name)
        return $false
    }

    return $true
}

function Install-Mvvm
{
    [CmdletBinding(HelpURI="http://github.com/MacawNL/QuickCross#install-mvvm")]
    Param(
       [string]$ProjectName
    )

    if (-not (InitializeProjects -ProjectName $ProjectName)) { return }

    Write-Host "QuickCross files will be added to these projects (existing files will not be modified)"
    if ($script:sharedCodeProjects.Count -gt 0) {
        Write-host "- Shared code     : " ($script:sharedCodeProjects | Select -ExpandProperty Name) 
    }
    if ($script:applicationProjects.Count -gt 0) {
        Write-host "- Application code: " ($script:applicationProjects | Select -ExpandProperty Name) 
    }
    if ($script:otherProjects.Count -gt 0) {
        Write-host "Other projects will not be modified because they are neither an application project nor a shared code project: " ($script:otherProjects | Select -ExpandProperty Name) 
    }
    Write-Host ""
    Write-Host "Press Y to continue, any other key to cancel..."
    if ($host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown").Character -ne 'Y') {
        Write-Host "Command canceled."
        return
    }

    $toolsPath = $PSScriptRoot
    # Get the application name from the solution file name
    $solutionName = Split-Path ($dte.Solution.FullName) -Leaf
    $appName = $solutionName.Split('.')[0]
    $platformDefines = @{
        'android' = '__ANDROID__';
        'ios'     = '__IOS__';
        'wp'      = 'WINDOWS_PHONE'
        'ws'      = 'NETFX_CORE';
        'wpa'     = 'NETFX_CORE';
    }

    foreach ($project in $script:sharedCodeProjects)
    {
        # TODO: ?check if the shared code is already present in another (referenced?) project in the solution, if not, install the shared code into the same project - to also support one-project solutions?
        #       OR: if shared code not installed, fail and give message to install and reference first? nonblocking Dialog needed?
        $ProjectName = $project.Name
        $defaultNamespace = $project.Properties.Item("DefaultNamespace").Value
        $platform = GetProjectPlatform -project $project

        Write-Host '-------------------------'
        Write-Host "Installing QuickCross shared code files in project $ProjectName ..."

        $nameReplacements = @{
            "_APPNAME_" = $appName
        }
        $csContentReplacements = GetContentReplacements -project $project -cs
        $contentReplacements = @{
            '_APPNAME_' = $appName;
            'QuickCross\.Templates' = $defaultNamespace
        }
        $librarySourceDirectory = Join-Path -Path $toolsPath -ChildPath library
        AddProjectItemsFromDirectory -project $project -sourceDirectory $librarySourceDirectory -contentReplacements $contentReplacements

        # Create default project items
        $null = AddProjectItem  -project $project `
                                -destinationProjectRelativePath ('I{0}Navigator.cs' -f $appName) `
                                -templatePackageFolder          'library' `
                                -templateProjectRelativePath    'QuickCross\Templates\I_APPNAME_Navigator.cs' `
                                -contentReplacements            $csContentReplacements
        $null = AddProjectItem  -project $project `
                                -destinationProjectRelativePath ('{0}Application.cs' -f $appName) `
                                -templatePackageFolder          'library' `
                                -templateProjectRelativePath    'QuickCross\Templates\_APPNAME_Application.cs' `
                                -contentReplacements            $csContentReplacements
        New-ViewModel -ViewModelName Main

		if (-not $project.FullName.EndsWith('.shproj')) {
            EnsureConditionalCompilationSymbol -project $project -define $platformDefines[$platform]
        }
    }

    foreach ($project in $script:applicationProjects)
    {
        $ProjectName = $project.Name
        $defaultNamespace = $project.Properties.Item("DefaultNamespace").Value
        $platform = GetProjectPlatform -project $project

        Write-Host '-------------------------'
        Write-Host "Installing QuickCross $platform application files in project $ProjectName ..."

        $nameReplacements = @{
            "_APPNAME_" = $appName
        }
        $csContentReplacements = GetContentReplacements -project $project -cs -isApplication
        $contentReplacements = @{
            '_APPNAME_' = $appName;
            'QuickCrossLibrary\.Templates' = $csContentReplacements['QuickCrossLibrary\.Templates'];
            'QuickCross\.Templates' = $defaultNamespace
        }

        $appSourceDirectory = Join-Path -Path $toolsPath -ChildPath "app.$platform"
        AddProjectItemsFromDirectory -project $project -sourceDirectory $appSourceDirectory -contentReplacements $contentReplacements

        # Create default project items
        switch ($platform)
        {
            'ios' {
                $null = AddProjectItem -project $project `
                                        -destinationProjectRelativePath ('{0}Navigator.cs' -f $appName) `
                                        -templatePackageFolder          'app.ios' `
                                        -templateProjectRelativePath    'QuickCross\Templates\_APPNAME_Navigator.cs' `
                                        -contentReplacements            $csContentReplacements
                New-View -ViewName Main
            }

            'android' {
                $null = AddProjectItem -project $project `
                                        -destinationProjectRelativePath ('{0}Navigator.cs' -f $appName) `
                                        -templatePackageFolder          'app.android' `
                                        -templateProjectRelativePath    'QuickCross\Templates\_APPNAME_Navigator.cs' `
                                        -contentReplacements            $csContentReplacements
                New-View -ViewName Main -ViewType MainLauncher
            }

            'wp' {
                $null = AddProjectItem -project $project `
                                        -destinationProjectRelativePath ('QuickCross\App.xaml.cs' -f $appName) `
                                        -templatePackageFolder          'app.wp' `
                                        -templateProjectRelativePath    'QuickCross\Templates\App.xaml.cs' `
                                        -contentReplacements            $csContentReplacements
                $null = AddProjectItem -project $project `
                                        -destinationProjectRelativePath ('{0}Navigator.cs' -f $appName) `
                                        -templatePackageFolder          'app.wp' `
                                        -templateProjectRelativePath    'QuickCross\Templates\_APPNAME_Navigator.cs' `
                                        -contentReplacements            $csContentReplacements
                New-View -ViewName Main
            }

            'ws' {
                $null = AddProjectItem -project $project `
                                        -destinationProjectRelativePath ('QuickCross\App.xaml.cs' -f $appName) `
                                        -templatePackageFolder          'app.ws' `
                                        -templateProjectRelativePath    'QuickCross\Templates\App.xaml.cs' `
                                        -contentReplacements            $csContentReplacements
                $null = AddProjectItem -project $project `
                                        -destinationProjectRelativePath ('{0}Navigator.cs' -f $appName) `
                                        -templatePackageFolder          'app.ws' `
                                        -templateProjectRelativePath    'QuickCross\Templates\_APPNAME_Navigator.cs' `
                                        -contentReplacements            $csContentReplacements
                New-View -ViewName Main
            }

            # TODO: Add wpa - maybe identical to ws?
        }

        EnsureConditionalCompilationSymbol -project $project -define $platformDefines[$platform]
    }
}

function New-ViewModel
{
    [CmdletBinding(HelpURI="http://github.com/MacawNL/QuickCross#new-viewmodel")]
    Param(
        [Parameter(Mandatory=$true)] [string]$ViewModelName,
        [string]$ProjectName,
        [switch]$NotInApplication
    )

    if (-not (InitializeProjects -ProjectName $ProjectName)) { return }
    $project = $script:sharedCodeProjects[0]
    $ProjectName = $project.Name
    
    if ((GetProjectType -project $project) -eq 'application')
    {
        Write-Host "Project $ProjectName is an application project; view models should be coded in a library project. Either specify a library project with the ProjectName parameter, or omit the ProjectName parameter to use the first class library project in the solution."
        return
    }

    $csContentReplacements = GetContentReplacements -project $project -cs
    $csContentReplacements.Add('_VIEWNAME_', $ViewModelName)

    $addedViewModel = AddProjectItem   -project $project `
                                       -destinationProjectRelativePath ('ViewModels\{0}ViewModel.cs' -f $ViewModelName) `
                                       -templatePackageFolder          'library' `
                                       -templateProjectRelativePath    'QuickCross\Templates\_VIEWNAME_ViewModel.cs' `
                                       -contentReplacements            $csContentReplacements

    if ($addedViewModel -and (-not $NotInApplication))
    {
        $libraryProjectDirectory = Split-Path -Path $project.FullName -Parent
        $appName = $csContentReplacements._APPNAME_
        AddCsCodeFromInlineTemplateInVsEditor -itemPath (Join-Path -Path $libraryProjectDirectory -ChildPath ('{0}Application.cs' -f $appName)) -templateName 'viewmodel' -replacements $csContentReplacements
    }
}

function New-View
{
    [CmdletBinding(HelpURI="http://github.com/MacawNL/QuickCross#new-view")]
    Param(
        [Parameter(Mandatory=$true)] [string]$ViewName,
        [string]$ViewType,
        [string]$ViewModelName,
        [string]$ProjectName,
        [switch]$WithoutNavigation
    )

    if (-not (InitializeProjects -ProjectName $ProjectName)) { return }
    if ($script:applicationProjects.Count -eq 0) {
        Write-Host "NOT adding view code because no application projects were found. Please add one or mode application projects to the solution."
        return
    }
    if (("$ViewType" -ne '') -and ($script:applicationProjects.Count -gt 1)) {
        Write-Host "NOT adding view code because the ViewType parameter was specified, which is platform-specific, and more than one application project was found. Please specify the project to which the view should be added by using the ProjectName parameter."
        return
    }

    # Create the view model if it does not exist:
    if ("$ViewModelName" -eq '') { $ViewModelName = $ViewName }
    New-ViewModel -ViewModelName $ViewModelName -NotInApplication:$WithoutNavigation

    foreach ($project in $script:applicationProjects) {
        $ProjectName = $project.Name
    
        if ((GetProjectType -project $project) -ne 'application')
        {
            Write-Host "Project $ProjectName is not an application project; views should be coded in an application project. Specify an application project with the ProjectName parameter or select an application project as the default project in the Package Manager Console."
            return
        }

        $csContentReplacements = GetContentReplacements -project $project -cs -isApplication
        $csContentReplacements.Add('_VIEWNAME_', $ViewName)

        if (-not $WithoutNavigation)
        {
            # Add the navigation code for the new view:
            $appName = $csContentReplacements._APPNAME_
            $libraryProject = $script:sharedCodeProjects[0]
            $libraryProjectDirectory = Split-Path -Path $libraryProject.FullName -Parent
            $applicationProjectDirectory = Split-Path -Path $project.FullName -Parent
            AddCsCodeFromInlineTemplateInVsEditor -itemPath (Join-Path -Path $libraryProjectDirectory     -ChildPath ('I{0}Navigator.cs'  -f $appName)) -templateName 'view' -replacements $csContentReplacements
            AddCsCodeFromInlineTemplateInVsEditor -itemPath (Join-Path -Path $applicationProjectDirectory -ChildPath ('{0}Navigator.cs'   -f $appName)) -templateName 'view' -replacements $csContentReplacements
            AddCsCodeFromInlineTemplateInVsEditor -itemPath (Join-Path -Path $libraryProjectDirectory     -ChildPath ('{0}Application.cs' -f $appName)) -templateName 'view' -replacements $csContentReplacements
        }

        $platform = GetProjectPlatform -project $project
        switch ($platform)
        {
            'ios' {
                if ("$ViewType" -eq '') { $ViewType = 'Code' }
                $viewNameSuffix = ''
                if ($ViewType.StartsWith('StoryBoard')) { $viewNameSuffix = '.TODO' }
                $null = AddProjectItem -project $project `
                                       -destinationProjectRelativePath ('{0}View{1}.cs' -f $ViewName, $viewNameSuffix) `
                                       -templatePackageFolder          'app.ios' `
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.cs' -f $ViewType) `
                                       -contentReplacements            $csContentReplacements
                if ($ViewType.StartsWith('Xib'))
                {
                    $null = AddProjectItem -project $project `
                                           -destinationProjectRelativePath ('{0}View{1}.designer.cs' -f $ViewName, $viewNameSuffix) `
                                           -templatePackageFolder          'app.ios' `
                                           -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.designer.cs' -f $ViewType) `
                                           -contentReplacements            $csContentReplacements
                }
            }

            'android' {
                if ("$ViewType" -eq '') { $ViewType = 'Activity' }

			    if ($ViewType -ne 'AlertDialog')
			    {
				    foreach ($markupType in @($ViewType, ''))
				    {
					    if (AddProjectItem -project $project `
									       -destinationProjectRelativePath ('Resources\Layout\{0}View.axml' -f $ViewName) `
									       -templatePackageFolder          'app.android' `
									       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.axml.template' -f $markupType) `
									       -contentReplacements            @{ '_VIEWNAME_' = $ViewName } `
									       -isOptionalItem:($markupType -ne ''))
					    { break }
				    }
			    }

                $null = AddProjectItem -project $project `
                                       -destinationProjectRelativePath ('{0}View.cs' -f $ViewName) `
                                       -templatePackageFolder          'app.android' `
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.cs' -f $ViewType) `
                                       -contentReplacements            $csContentReplacements
            }

            'wp' {
                if ("$ViewType" -eq '') { $ViewType = 'Page' }
                foreach ($markupType in @($ViewType, ''))
                {
                    if (AddProjectItem -project $project `
                                       -destinationProjectRelativePath ('{0}View.xaml' -f $ViewName) `
                                       -templatePackageFolder          'app.wp' `
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.xaml.template' -f $markupType) `
                                       -contentReplacements            $csContentReplacements `
                                       -isOptionalItem:($markupType -ne ''))
                    { break }
                }
                $null = AddProjectItem -project $project `
                                       -destinationProjectRelativePath ('{0}View.xaml.cs' -f $ViewName) `
                                       -templatePackageFolder          'app.wp' `
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.xaml.cs' -f $ViewType) `
                                       -contentReplacements            $csContentReplacements
            }

            'ws' {
                if ("$ViewType" -eq '') { $ViewType = 'Page' }
                foreach ($markupType in @($ViewType, ''))
                {
                    if (AddProjectItem -project $project `
                                       -destinationProjectRelativePath ('{0}View.xaml' -f $ViewName) `
                                       -templatePackageFolder          'app.ws' `
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.xaml.template' -f $markupType) `
                                       -contentReplacements            $csContentReplacements `
                                       -isOptionalItem:($markupType -ne ''))
                    { break }
                }
                $null = AddProjectItem -project $project `
                                       -destinationProjectRelativePath ('{0}View.xaml.cs' -f $ViewName) `
                                       -templatePackageFolder          'app.ws' `
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.xaml.cs' -f $ViewType) `
                                       -contentReplacements            $csContentReplacements
            }

            # TODO: add wpa

            default { Write-Host "New-View currenty only supports iOS, Android, Windows Phone and Windows Store application projects; platform $platform is currently not supported"; return }
        }
    }
}

Export-ModuleMember -Function Install-Mvvm
Export-ModuleMember -Function New-ViewModel
Export-ModuleMember -Function New-View