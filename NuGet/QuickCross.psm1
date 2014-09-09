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


# EnsureProjectFolderItem exists to compensate that $project.ProjectItems.AddFromFile() does not
# work correctly in Shared Code projects when the item is in a subfolder below the project folder
# (it always adds the item to the project root - verified in VS 2013 Update 2 RC).
# Another limitation is that when a subfolder already exists on disk, you can neither include it in 
# the project nor create it as a project item with AddFolder()
# The workaround is to temporarily rename the folder on disk if it exists.
# Then add the existing project item using ProjectItems.AddFromFile() on the obtained parent folder 
# project item node, instead of on the project node.
# Note that a rename may fail because files in the folder may be opened even though they are not 
# included in the project. Sigh.
function EnsureProjectFolderItem
{
    Param(
        [Parameter(Mandatory=$true)] $project,
        [Parameter(Mandatory=$true)] [string]$folderPath
    )

    if ($folderPath.TrimEnd('\') -eq (Split-Path -Path $project.FullName -Parent).TrimEnd('\')) { return $project }

    $folderPathItem = $dte.Solution.FindProjectItem($folderPath)

    if ($folderPathItem -eq $null) {
        $renameDone = $false
        if (Test-Path -Path $folderPath) {
            try   { 
                $renamedFolderPath = $folderPath + '.tmp'
                Write-Host "Temporarily renaming $folderPath to $renamedFolderPath ..."
                Move-Item -Path $folderPath -Destination $renamedFolderPath -Force 
                $renameDone = $true
            }
            catch { Write-Error "Cannot temporarily rename folder:`r`n   '$folderPath' to: `r`n   '$renamedFolderPath'.`r`nPlease close any windows that access this folder or files and folders within it, and then try again." }
        }

        try {
            $folderPathItem = $project.ProjectItems.AddFolder($folderPath, '')
            # As long as the last folder in the path does not exist, AddFolder will create and include as much of the path as needed - in a VS Shared Code project
        }
        finally {
            if ($renameDone) {
                Write-Host "Reversing temporarily renamed $renamedFolderPath back to $folderPath ..."
                Remove-Item -Path $folderPath -Recurse -Force
                Move-Item -Path $renamedFolderPath -Destination $folderPath -Force
            }
        }
    }

    $folderPathItem
}

function IsVsSharedCodeProject
{
    Param(
        [Parameter(Mandatory=$true)] $project
    )

    $project.FullName.EndsWith('.shproj')
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
    if (Test-Path -Path $destinationPath) { Write-Host ('NOT adding project item because it already exists: {0}' -f (SolutionRelativePath -path $destinationPath)); return $false }
    Write-Host ('Adding project item: {0}' -f (SolutionRelativePath -path $destinationPath))

    $destinationFolder = Split-Path -Path $destinationPath -Parent
    if ((IsVsSharedCodeProject -project $project)) {
        # It looks like the implementation for VS Shared Projects behaves different for the same DTE interfaces - at least in VS2013 Update 2 RC. So we handle that separately.
        # Ensure that the destination folder exists on disk as well as a project item, and get the parent folder's project item object so we can add the file to it.
        $parentItem = EnsureProjectFolderItem -project $project -folderPath $destinationFolder
    } else {
        if (-not(Test-Path -Path $destinationFolder)) { 
            $null = New-Item $destinationFolder -ItemType Directory -Force 
        }
        $parentItem = $project
    }

    Copy-Item -Path $templatePath -Destination $destinationPath -Force
    ReplaceStringsInFile -filePath $destinationPath -replacements $contentReplacements

    $null = $parentItem.ProjectItems.AddFromFile($destinationPath)
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
            $parentItem = $project
            if ($destinationPathExists) {
                Write-Host ('NOT adding project item because it already exists: {0}' -f (SolutionRelativePath -path $destinationPath))
            } else {
                $destinationFolder = Split-Path -Path $destinationPath -Parent
                if ((IsVsSharedCodeProject -project $project)) {
                    # It looks like the implementation for VS Shared Projects behaves different for the same DTE interfaces - at least in VS2013 Update 2 RC. So we handle that separately.
                    # Ensure that the destination folder exists on disk as well as a project item, and get the parent folder's project item object so we can add the file to it.
                    $parentItem = EnsureProjectFolderItem -project $project -folderPath $destinationFolder
                } else {
                    if (-not(Test-Path -Path $destinationFolder)) { 
                        $null = New-Item $destinationFolder -ItemType Directory -Force 
                    }
                }

                Copy-Item -Path $_.FullName -Destination $destinationPath -Force
                ReplaceStringsInFile -filePath $destinationPath -replacements $contentReplacements
            }
            $null = $parentItem.ProjectItems.AddFromFile($destinationPath)
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
            $libraryProject = GetTheSharedCodeProject
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
		if ((IsVsSharedCodeProject -project $project)) {
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

$TheSharedCodeProject = $null

function GetTheSharedCodeProject
{
    Param(
       [array]$projects,
       [switch]$force,
       [switch]$allowEmpty
    )

    if ($force) { $script:TheSharedCodeProject = $null }

    if ($TheSharedCodeProject -eq $null) {
        if ($projects.Length -eq 0) { $projects = GetAllProjects }
        foreach ($project in $projects)
        {
            if ((GetProjectType -project $project) -eq 'shared')
            {
                if ($allowEmpty -and (($TheSharedCodeProject -eq $null) -or ($TheSharedCodeProject.Name.Length -gt $project.Name.Length))) { $script:TheSharedCodeProject = $project }
                if ($project.ProjectItems.Item('QuickCross') -ne $null) {
                    $script:TheSharedCodeProject = $project
                    break
                }
            }
        }
    }

    $TheSharedCodeProject
}

function ReportIfSharedCodeProjectIsNotInstalled
{
    if ($TheSharedCodeProject -eq $null) {
        Write-Host "No shared code project was found with QuickCross installed. Please ensure that the solution contains either a shared code project for all target platforms, or a class library project for a single target platform, and then run the Install-Mvvm command."
    }
    return ($TheSharedCodeProject -eq $null)
}

function GetApplicationProjects
{
    Param(
       [array]$projects
    )

    $applicationProjects = @()
    if (($projects -eq $null) -or ($projects.Length -eq 0)) { $projects = GetAllProjects }
    foreach ($project in $projects)
    {
        if ((GetProjectType -project $project) -eq 'application') { $applicationProjects += $project }
    }
    $applicationProjects
}

function Install-Mvvm
{
    [CmdletBinding(HelpURI="http://github.com/MacawNL/QuickCross#install-mvvm")]
    Param(
       [string]$ProjectName,
       [switch]$NoMainView
    )

    [array]$allProjects = GetAllProjects
    $sharedProject = $null
    [array]$applicationProjects = @()

    if ("$ProjectName" -eq '') {
        $applicationProjects = GetApplicationProjects -projects $allProjects
        $sharedProject = GetTheSharedCodeProject -projects $allProjects -force -allowEmpty
        if ((ReportIfSharedCodeProjectIsNotInstalled)) { return }
    } else {
        $project = Get-Project $ProjectName
        if ($project -eq $null)  { Write-Host "Project '$projectName' not found in solution."; return }
        switch ((GetProjectType -project $project))
        {
            'shared'      {
                $sharedProject = GetTheSharedCodeProject -projects $allProjects -force
                if ($sharedProject.UniqueName -ne $project.UniqueName) {
                    $currentSharedProjectName = $sharedProject.Name
                    Write-Host "No action is taken because the $ProjectName project is a shared code project, but the QuickCross shared code is already installed in the $currentSharedProjectName project. To install the QuickCross shared code in the $ProjectName project, either move the shared code (including the QuickCross folder) from the $currentSharedProjectName project to the $ProjectName project, or remove the QuickCross folder from the $currentSharedProjectName project and re-run the command Install-Mvvm -ProjectName $ProjectName"
                    return
                }
            }

            'application' {
                $applicationProjects += $project
                $sharedProject = GetTheSharedCodeProject -projects $allProjects -force -allowEmpty
                if ((ReportIfSharedCodeProjectIsNotInstalled)) { return }
            }

            default       {
                Write-Host "Project '$projectName' is not an application or shared code project. Please specify an application project, or a shared code project for all target platforms, or a class library project for a single target platform."; 
                return
            }
        }
    }

    Write-Host "QuickCross files will be added to these projects (existing files will not be modified)"
    if ($sharedProject -ne $null) {
        Write-host "- Shared code     : " $sharedProject.Name
    }
    if ($applicationProjects.Count -gt 0) {
        Write-host "- Application code: " ($applicationProjects | Select -ExpandProperty Name) 
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

    if ($sharedProject -ne $null)
    {
        $project = $sharedProject

        $defaultNamespace = $project.Properties.Item("DefaultNamespace").Value
        $platform = GetProjectPlatform -project $project

        Write-Host '-------------------------'
        Write-Host "Installing QuickCross shared code files in project $($sharedProject.Name) ..."

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
        if (-not $NoMainView) { New-ViewModel -ViewModelName Main }

		if (-not (IsVsSharedCodeProject -project $project)) {
            EnsureConditionalCompilationSymbol -project $project -define $platformDefines[$platform]
        }
    }

    foreach ($project in $applicationProjects)
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
                if (-not $NoMainView) { New-View -ViewName Main -ProjectName $ProjectName }
            }

            'android' {
                $null = AddProjectItem -project $project `
                                        -destinationProjectRelativePath ('{0}Navigator.cs' -f $appName) `
                                        -templatePackageFolder          'app.android' `
                                        -templateProjectRelativePath    'QuickCross\Templates\_APPNAME_Navigator.cs' `
                                        -contentReplacements            $csContentReplacements
                if (-not $NoMainView) { New-View -ViewName Main -ViewType MainLauncher -ProjectName $ProjectName }
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
                if (-not $NoMainView) { New-View -ViewName Main -ProjectName $ProjectName }
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
                if (-not $NoMainView) { New-View -ViewName Main -ProjectName $ProjectName }
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
        [switch]$NotInApplication
    )

    $project = GetTheSharedCodeProject
    if ((ReportIfSharedCodeProjectIsNotInstalled)) { return }
    $ProjectName = $project.Name
    
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

    $libraryProject = GetTheSharedCodeProject
    if ((ReportIfSharedCodeProjectIsNotInstalled)) { return }
    $libraryProjectDirectory = Split-Path -Path $libraryProject.FullName -Parent

    if ("$ProjectName" -eq '') {
        [array]$applicationProjects = GetApplicationProjects
        if ($applicationProjects.Count -eq 0) {
            Write-Host "NOT adding view code because no application projects were found. Please add one or mode application projects to the solution."
            return
        }
        if (("$ViewType" -ne '') -and ($applicationProjects.Count -gt 1)) {
            Write-Host "NOT adding view code because the ViewType parameter was specified, which is platform-specific, and more than one application project was found. Please specify the project to which the view should be added by using the ProjectName parameter."
            return
        }
    } else {
        $project = Get-Project $ProjectName
        if ($project -eq $null)  { Write-Host "Project '$projectName' not found in solution."; return }
        if ((GetProjectType -project $project) -ne 'application') {
            Write-Host "Project '$projectName' is not an application project. Please specify an application project.";
            return
        }
        [array]$applicationProjects = @($project)
    }

    # Create the view model if it does not exist:
    if ("$ViewModelName" -eq '') { $ViewModelName = $ViewName }
    New-ViewModel -ViewModelName $ViewModelName -NotInApplication:$WithoutNavigation

    foreach ($project in $applicationProjects) {
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
            $applicationProjectDirectory = Split-Path -Path $project.FullName -Parent
            AddCsCodeFromInlineTemplateInVsEditor -itemPath (Join-Path -Path $libraryProjectDirectory     -ChildPath ('I{0}Navigator.cs'  -f $appName)) -templateName 'view' -replacements $csContentReplacements
            AddCsCodeFromInlineTemplateInVsEditor -itemPath (Join-Path -Path $applicationProjectDirectory -ChildPath ('{0}Navigator.cs'   -f $appName)) -templateName 'view' -replacements $csContentReplacements
            AddCsCodeFromInlineTemplateInVsEditor -itemPath (Join-Path -Path $libraryProjectDirectory     -ChildPath ('{0}Application.cs' -f $appName)) -templateName 'view' -replacements $csContentReplacements
        }

        $platform = GetProjectPlatform -project $project
        $actualViewType = $ViewType
        switch ($platform)
        {
            'ios' {
                if ("$actualViewType" -eq '') { $actualViewType = 'Code' }
                $viewNameSuffix = ''
                if ($actualViewType.StartsWith('StoryBoard')) { $viewNameSuffix = '.TODO' }
                $null = AddProjectItem -project $project `
                                       -destinationProjectRelativePath ('{0}View{1}.cs' -f $ViewName, $viewNameSuffix) `
                                       -templatePackageFolder          'app.ios' `
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.cs' -f $actualViewType) `
                                       -contentReplacements            $csContentReplacements
                if ($actualViewType.StartsWith('Xib'))
                {
                    $null = AddProjectItem -project $project `
                                           -destinationProjectRelativePath ('{0}View{1}.designer.cs' -f $ViewName, $viewNameSuffix) `
                                           -templatePackageFolder          'app.ios' `
                                           -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.designer.cs' -f $actualViewType) `
                                           -contentReplacements            $csContentReplacements
                }
            }

            'android' {
                if ("$actualViewType" -eq '') { $actualViewType = 'Activity' }

			    if ($actualViewType -ne 'AlertDialog')
			    {
				    foreach ($markupType in @($actualViewType, ''))
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
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.cs' -f $actualViewType) `
                                       -contentReplacements            $csContentReplacements
            }

            'wp' {
                if ("$actualViewType" -eq '') { $actualViewType = 'Page' }
                foreach ($markupType in @($actualViewType, ''))
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
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.xaml.cs' -f $actualViewType) `
                                       -contentReplacements            $csContentReplacements
            }

            'ws' {
                if ("$actualViewType" -eq '') { $actualViewType = 'Page' }
                foreach ($markupType in @($actualViewType, ''))
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
                                       -templateProjectRelativePath    ('QuickCross\Templates\_VIEWNAME_{0}View.xaml.cs' -f $actualViewType) `
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