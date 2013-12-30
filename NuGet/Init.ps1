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
if ((Get-Module QuickCross) -ne $null) 
{
    Write-Host "Removing existing QuickCross module"
    Remove-Module -Name QuickCross 
}
$modulePath = Join-Path -Path $toolsPath -ChildPath QuickCross.psm1
Write-Host "Importing QuickCross module from $modulePath"
Import-Module -Name $modulePath
$commands = Get-Command -Module QuickCross -Syntax | Out-String
$commands = $commands -replace '[\r\n]+', "`r`n"
Write-Host "Available QuickCross Commands:`r`n$commands"
Write-Host 'For detailed help, type "Get-Help <command> -Online"'
