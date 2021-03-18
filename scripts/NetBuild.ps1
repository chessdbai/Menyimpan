$commonModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'CommonBuildScripts.psm1'
Import-Module $commonModLocation -Force

$buildProjects = Get-DeclaredSubprojects
$projectRoot = Get-ProjectRoot

$buildProjects | ForEach-Object {
  $projectName = $_
  $subprojectPath = Join-Path -Path $projectRoot -ChildPath $projectName
  Write-Host "Building project $_"
  $buildProcess = Start-Process `
    -FilePath "dotnet" `
    -WorkingDirectory $subprojectPath `
    -ArgumentList 'build','-c','Release' `
    -PassThru
  Wait-Process -InputObject $buildProcess
  $exitCode = $buildProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Stopping the build since a the build subprocess for project $projectName failed." `
      -ErrorAction Stop
  }  


}