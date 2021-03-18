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

function Add-GlobalDotNetTool([String] $ToolName) {
  $installed = Get-DotNetGlobalToolInstalled -ToolName $ToolName
  if ($installed -eq $true)
  {
    Write-Error `
      -Message "The tool is already currently installed." `
      -ErrorAction Stop
  }  
  $installToolProcess = Start-Process `
    -FilePath "dotnet" `
    -ArgumentList "tool","install","--global",$ToolName `
    -PassThru
  Wait-Process -InputObject $installToolProcess
  $exitCode = $installToolProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Restore failed because the ReportGenerator tool could not be added as a dotnet global tool." `
      -ErrorAction Stop
  }  
}

function Remove-GlobalDotNetTool([String] $ToolName) {
  $installed = Get-DotNetGlobalToolInstalled -ToolName $ToolName
  if ($installed -eq $false)
  {
    Write-Error `
      -Message "The tool is not currently installed." `
      -ErrorAction Stop
  }  
  $uninstallToolProcess = Start-Process `
    -FilePath "dotnet" `
    -ArgumentList "tool","uninstall","--global",$ToolName `
    -PassThru
  Wait-Process -InputObject $uninstallToolProcess
  $exitCode = $uninstallToolProcess.ExitCode
  If ($exitCode -ne 0)
  {
    Write-Error `
      -Message "Restore failed because the ReportGenerator tool could not be added as a dotnet global tool." `
      -ErrorAction Stop
  }  
}

Export-ModuleMember -Function *