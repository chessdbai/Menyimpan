$commonModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'CommonBuildLogic.psm1'
Import-Module $commonModLocation -Force
Import-Module AWS.Tools.ECR
Import-Module AWS.Tools.S3

Set-AWSCredential -ProfileName "chessdb-staging"
Set-DefaultAWSRegion -Region us-east-2

$loginCmdResponse = Get-ECRLoginCommand
$loginCmd = $loginCmdResponse.Command

$bucketName = 'chessdb-maki-source-artifacts'
$weightsFileName = 'weights'
Write-Host 'Downloading weights....'
Read-S3Object -BucketName $bucketName -Key $weightsFileName -File 'weights'

Login-DockerRepository $loginCmd

$assemblyFile = Join-Path -Path $PSScriptRoot -ChildPath "../published/Maki.dll"
$version = Get-AssemblyVersion $assemblyFile
$tag = "maki:$version"
Write-Host "Publishing image with tag $tag"
Publish-DockerImage $tag

$distPath = Join-Path -Path $PSScriptRoot -ChildPath "../dist"
New-Item -ItemType Directory -Force -Path $distPath
$distFilePath = Join-Path -Path $distPath -ChildPath "manifest.txt"
Set-Content -Path $distFilePath -Value "maki:$version"