param(
    [Parameter(Mandatory=$true)] [string]$installPath,
    [Parameter(Mandatory=$true)] [string]$toolsPath,
    [Parameter(Mandatory=$true)] $package,
    $project
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

Write-Host "" # We need to write an empty line to prevent an error when the solution is (re)loaded and the user starts typing in the package manager console.
if ((Get-Module MvvmQuickCross) -ne $null) 
{
    Write-Host "Removing existing MvvmQuickCross module"
    Remove-Module -Name MvvmQuickCross 
}
$modulePath = Join-Path -Path $toolsPath -ChildPath MvvmQuickCross.psm1
Write-Host "Importing MvvmQuickCross module from $modulePath"
Import-Module -Name $modulePath
$commands = Get-Command -Module MvvmQuickCross -Syntax | Out-String
$commands = $commands -replace '[\r\n]+', "`r`n"
Write-Host "Available MvvmQuickCross Commands:`r`n$commands"
Write-Host 'For detailed help, type "Get-Help <command> -Online"'
