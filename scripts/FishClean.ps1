$commonModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'CommonBuildScripts.psm1'
Import-Module $commonModLocation -Force

$projectRoot = Get-ProjectRoot
$srcPath = Join-Path -Path $projectRoot -ChildPath "src"

Write-Host "Removing compiled C/C++ objects..."
ls $srcPath/*.o -Recurse | foreach {rm $_}

Write-Host "Removing 'dist' dir if exists..."
$distDir = Join-Path -Path $projectRoot -ChildPath "dist"
$alreadyExists = Test-Path -PathType Container -Path $distDir
if ($alreadyExists -eq $true) 
{
  Remove-Item $distDir -Recurse | Out-Null
}
