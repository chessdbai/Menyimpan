$commonModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'CommonBuildScripts.psm1'
Import-Module $commonModLocation -Force

$buildProjects = Get-DeclaredSubprojects
$projectRoot = Get-ProjectRoot

# dotnet pack -c Release

$buildProjects | ForEach-Object {
  $projectName = $_
  $subprojectPath = Join-Path -Path $projectRoot -ChildPath $projectName
  Write-Host "Packing project $_"
  $packProcess = Start-Process `
    -FilePath "dotnet" `
    -ArgumentList 'pack','-c','Release' `
    -WorkingDirectory $subprojectPath `
    -PassThru
  Wait-Process -InputObject $packProcess
  $exitCode = $packProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Stopping the build since a the pack subprocess for project $projectName failed." `
      -ErrorAction Stop
  }  

  $nupkgDir = Join-Path -Path $subprojectPath -ChildPath "bin/Release"
  Write-Host "Looking for .nupkg file in $nupkgDir"
  $nupkgName = Get-ChildItem $nupkgDir -Filter '*.nupkg' | Select-Object -First 1
  Write-Host "Published .nupkg file to $nupkgName"
  $pushProcess = Start-Process `
    -FilePath "dotnet" `
    -WorkingDirectory $subprojectPath `
    -ArgumentList 'nuget','push',$nupkgName,'-s','chessdb/chessdb-and-npm' `
    -PassThru
  Wait-Process -InputObject $pushProcess
  $exitCode = $pushProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Stopping the build since a the pack subprocess for project $projectName failed." `
      -ErrorAction Stop
  }  
}