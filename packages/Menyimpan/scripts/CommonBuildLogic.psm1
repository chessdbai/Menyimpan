function Get-AssemblyVersion() {
  param([string] $file)
  
  $version = [System.Reflection.AssemblyName]::GetAssemblyName($file).Version;
  
  #format the version and output it...
  $version
}

function Build-DockerImage() {
  param([string] $tag)

  $packProcess = Start-Process `
    -FilePath "docker" `
    -ArgumentList "build -t $tag ." `
    -PassThru
  Wait-Process -InputObject $packProcess
  $exitCode = $packProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Halting pack process since a the 'docker build' subprocess for project failed." `
      -ErrorAction Stop
  }
}

function Login-DockerRepository() {
  param([string] $cmd)

  $args = $cmd.split(' ') | Select-Object -Skip 1
  $loginProcess = Start-Process `
    -FilePath "docker" `
    -ArgumentList $args `
    -PassThru
  Wait-Process -InputObject $loginProcess
  $exitCode = $loginProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Halting pack process since a the 'docker build' subprocess for project failed." `
      -ErrorAction Stop
  }
}

function Tag-DockerImage() {
  param([string] $tag)

  $tagProcess = Start-Process `
    -FilePath "docker" `
    -ArgumentList "tag $tag 407299974961.dkr.ecr.us-east-2.amazonaws.com/$tag" `
    -PassThru
  Wait-Process -InputObject $tagProcess
  $exitCode = $tagProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Halting pack process since a the 'docker tag' subprocess for project failed." `
      -ErrorAction Stop
  }
}

function Publish-DockerImage() {
  param([string] $tag)

  $publishProcess = Start-Process `
    -FilePath "docker" `
    -ArgumentList "push 407299974961.dkr.ecr.us-east-2.amazonaws.com/$tag" `
    -PassThru
  Wait-Process -InputObject $publishProcess
  $exitCode = $publishProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Halting pack process since a the 'docker tag' subprocess for project failed." `
      -ErrorAction Stop
  }
}

Export-ModuleMember -Function *