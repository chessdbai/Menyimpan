$commonModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'CommonBuildScripts.psm1'
Import-Module $commonModLocation -Force

$buildProjects = Get-DeclaredSubprojects
$projectRoot = Get-ProjectRoot

$buildProjects | ForEach-Object {
  $projectName = $_
  $subprojectPath = Join-Path -Path $projectRoot -ChildPath $projectName
  Write-Host "Cleaning project $_"
  $cleanProcess = Start-Process `
    -FilePath "dotnet" `
    -WorkingDirectory $subprojectPath `
    -ArgumentList "clean" `
    -PassThru
  Wait-Process -InputObject $cleanProcess
  $exitCode = $cleanProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Warn `
      -Message "Stopping the build since a the build subprocess for project $projectName failed." `
      -ErrorAction Stop
  }  
}