$commonModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'CommonBuildLogic.psm1'
Import-Module $commonModLocation -Force

$currentDirectory = Get-Location
$assemblyFile = Join-Path -Path $PSScriptRoot -ChildPath "../published/Placeholder.dll"
$version = Get-AssemblyVersion $assemblyFile
$tag = "placeholder:$version"
Write-Host "Using Docker tag Docker image $tag"
Write-Host "Building Docker Image..."

Build-DockerImage -Tag $tag
Tag-DockerImage -Tag $tag