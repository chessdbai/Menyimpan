$commonModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'CommonBuildScripts.psm1'
Import-Module $commonModLocation -Force

function Create-DistFolder() {
  $projectRoot = Get-ProjectRoot
  $tmpPath = Join-Path -Path $projectRoot -ChildPath "dist"
  $alreadyExists = Test-Path -PathType Container -Path $tmpPath
  if ($alreadyExists -ne $true) 
  {
    New-Item -ItemType Directory -Force -Path $tmpPath | Out-Null;
  }
  return $tmpPath
}

$projectRoot = Get-ProjectRoot
$srcPath = Join-Path -Path $projectRoot -ChildPath "src"
$projectName = Get-ProjectName

Write-Host "Building $projectName in $projectRoot..."

# make build ARCH=x86-64 COMP=clang
$makeProcess = Start-Process `
  -FilePath "make" `
  -WorkingDirectory $srcPath `
  -Args "build ARCH=x86-64 COMP=clang" `
  -PassThru
Wait-Process -InputObject $makeProcess
$exitCode = $makeProcess.ExitCode
If ($exitCode -ne 0)
{
  Write-Warn `
    -Message "Stopping the build since a the build subprocess for project $projectName failed." `
    -ErrorAction Stop
}  

$buildArtifact = Join-Path -Path $srcPath -ChildPath "stockfish"
$distFolder = Create-DistFolder
$destPath = Join-Path -Path $distFolder -ChildPath "bluefish"
Copy-Item -Path $buildArtifact -Destination $destPath