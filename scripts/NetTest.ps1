$commonModLocation = Join-Path -Path $PSScriptRoot -ChildPath 'CommonBuildScripts.psm1'
Import-Module $commonModLocation -Force

$buildProjects = Get-DeclaredTestSubprojects
$projectRoot = Get-ProjectRoot
Write-Host "Project root: $projectRoot"

$projectName = Get-ProjectName
Write-Host "Project name: $projectName, Project Root: $projectRoot"

# dotnet test \
# /p:CoverletOutputFormat=opencover \
# /p:CollectCoverage=true \
# /p:CoverletOutput=$COVERAGE_FILE_PATH \
# --logger "junit;LogFilePath=$COVERAGE_FILE_PATH"

$projectReportsDir = Get-ProjectTestReportsPath
$rootReportsPath = Get-RootTestReportsPath
$lernaRoot = Get-LernaRoot
Write-Host "Lerna root: $lernaRoot"
Write-Host "Root test reports path: $rootReportsPath"
Write-Host "Project test reports path: $projectReportsDir"

$buildProjects | ForEach-Object {
  $projectName = $_
  $subprojectPath = Join-Path -Path $projectRoot -ChildPath $projectName

  $coverageFilePath = Join-Path -Path $projectReportsDir -ChildPath "coverage.xml"
  $testResultsFilePath = Join-Path -Path $projectReportsDir -ChildPath "test-results.xml"
  
  Write-Host "Building test project $_"
  $buildProcess = Start-Process `
    -FilePath "dotnet" `
    -WorkingDirectory $subprojectPath `
    -ArgumentList 'build' `
    -PassThru
  Wait-Process -InputObject $buildProcess
  $buildExitCode = $buildProcess.ExitCode
  If ($buildExitCode -ne 0)
  {
    Write-Error `
      -Message "Stopping the build since a the build subprocess for project $projectName failed." `
      -ErrorAction Stop
  }    
  Write-Host "Coverlay output path: $coverageFilePath"
  Write-Host "Unit Tests output path: $testResultsFilePath"
  Write-Host "Executing test project $_"
  $testProcess = Start-Process `
    -FilePath "dotnet" `
    -WorkingDirectory $subprojectPath `
    -ArgumentList 'test','/p:CoverletOutputFormat=cobertura','/p:CollectCoverage=true',"/p:CoverletOutput=$coverageFilePath","--logger `"junit;LogFilePath=$testResultsFilePath\`"" `
    -PassThru
  Wait-Process -InputObject $testProcess
  $testExitCode = $buildProcess.ExitCode
  If ($testExitCode -ne 0)
  {
    Write-Error `
      -Message "Stopping the build since a the build subprocess for project $projectName failed." `
      -ErrorAction Stop
  }

  $homeDir = Get-Variable HOME -valueOnly
  $reportToolPath = Join-Path -Path $homeDir -ChildPath '.dotnet/tools/reportgenerator'
  $reportProcess = Start-Process `
    -FilePath $reportToolPath `
    -WorkingDirectory $subprojectPath `
    -ArgumentList "-reports:$coverageFilePath","-targetdir:$projectReportsDir\coverage-html","-reporttypes:HTML" `
    -PassThru
  Wait-Process -InputObject $reportProcess
  $testExitCode = $reportProcess.ExitCode
  If ($testExitCode -ne 0)
  {
    Write-Error `
      -Message "Stopping the build since a the build subprocess for project $projectName failed." `
      -ErrorAction Stop
  }
}