function Find-Up($pathtosearch, $filename)
{
  if($pathtosearch -eq "")
  {
    Write-Error -Message "Could not find path: $path" -ErrorAction Stop
  }
  elseif (Test-Path "$pathtosearch\$filename" )
  {
    $pathtosearch
  }
  else
  {
    Find-Up (Split-Path $pathtosearch) $filename
  }
}

function Get-AssemblyVersion() {
  param([string] $file)
  
  $version = [System.Reflection.AssemblyName]::GetAssemblyName($file).Version;
  
  #format the version and output it...
  $version
}

function Get-LernaRoot {
  $myDir = Get-Location
  $rootDir = Find-Up $myDir "lerna.json"
  $rootDir
}

function Get-ProjectRoot {
  $rootDir = Get-LernaRoot
  $myDir = Get-Location
  $projectDir = Find-Up $myDir "package.json"
  If ($rootDir -eq $projectDir)
  {
    Write-Error `
      -Message "You are not currently in a buildable .NET Core lergna package." `
      -ErrorAction Stop
  }
  $projectDir
}

function Get-ProjectName {
  $projectRoot = Get-ProjectRoot
  $baseName = $projectRoot | Split-Path -Leaf
  $baseName
}

function Get-PackageJson {
  $rootDir = Get-LernaRoot
  $projectDir = Get-ProjectRoot
  $projectPackageJsonPath = Join-Path -Path $projectDir -ChildPath 'package.json'
  $packageJson = Get-Content -Path $projectPackageJsonPath | ConvertFrom-Json -AsHashtable
  $packageJson
}

function Get-DeclaredSubprojects {
  $packageJson = Get-PackageJson
  $packageJson.dotnet.projects
}

function Get-DeclaredTestSubprojects {
  $packageJson = Get-PackageJson
  $packageJson.dotnet.testProjects
}

function Get-RootTestReportsPath {
  $rootDir = Get-LernaRoot
  $reportsDir = Join-Path -Path $rootDir -ChildPath 'test-reports'
  $alreadyExists = Test-Path -PathType Container -Path $reportsDir
  if ($alreadyExists -ne $true) 
  {
    New-Item -ItemType Directory -Force -Path $reportsDir | Out-Null;
  }
  return $reportsDir
}

function Get-ProjectTestReportsPath {
  $reportsRoot = Get-RootTestReportsPath
  $projectName = Get-ProjectName
  $projectReportsDir = Join-Path -Path $reportsRoot -ChildPath $projectName
  $alreadyExists = Test-Path -PathType Container -Path $projectReportsDir
  if ($alreadyExists -ne $true) 
  {
    New-Item -ItemType Directory -Force -Path $projectReportsDir | Out-Null;
  }
  $projectReportsDir
}

function Get-DotNetGlobalToolInstalled([String] $ToolName) {
  $psi = New-object System.Diagnostics.ProcessStartInfo 
  $psi.CreateNoWindow = $true 
  $psi.UseShellExecute = $false 
  $psi.RedirectStandardOutput = $true 
  $psi.RedirectStandardError = $true 
  $psi.FileName = 'dotnet' 
  $psi.Arguments = @("tool list --global")
  $process = New-Object System.Diagnostics.Process 
  $process.StartInfo = $psi 
  [void]$process.Start()
  $output = $process.StandardOutput.ReadToEnd() 
  $process.WaitForExit() 
  $exitCode = $process.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Listing installed global tools failed." `
      -ErrorAction Stop
  }  
  $installed = $output -Match $ToolName
  $installed
}


Export-ModuleMember -Function Get-AssemblyVersion,Get-RootTestReportsPath,Get-DeclaredSubprojects,Get-DeclaredTestSubprojects,Get-ProjectRoot,Get-LernaRoot,Get-ProjectTestReportsPath,Get-ProjectName,Get-DotNetGlobalToolInstalled,Install-DotNetGlobalTool