$commonModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'CommonBuildScripts.psm1'
Import-Module $commonModLocation -Force
$dotnetToolsModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'DotNetToolScripts.psm1'
Import-Module $dotnetToolsModLocation -Force

$buildProjects = Get-DeclaredSubprojects
$projectRoot = Get-ProjectRoot

$buildProjects | ForEach-Object {
  $projectName = $_
  $subprojectPath = Join-Path -Path $projectRoot -ChildPath $projectName
  Write-Host "Restoring project $_"
  $restoreProcess = Start-Process `
    -FilePath "dotnet" `
    -WorkingDirectory $subprojectPath `
    -ArgumentList "restore" `
    -PassThru
  Wait-Process -InputObject $restoreProcess
  $exitCode = $restoreProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Stopping the build since the restore subprocess for project $projectName failed." `
      -ErrorAction Stop
  }  
}

$reportGeneratorInstalled = Get-DotNetGlobalToolInstalled -ToolName "dotnet-reportgenerator-globaltool"
if ($reportGeneratorInstalled -eq $false)
{
  Install-DotNetGlobalToool -ToolName = "dotnet-reportgenerator-globaltool"
}