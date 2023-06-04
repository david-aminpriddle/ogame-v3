# starts yarn dev and restarts if it crashes

# Define a function to stop the process
function Stop-ProcessGracefully {
    param($Process)

    if (!$Process.HasExited) {
        $Process | Stop-Process -Force
    }
}

# Register a script block that will be invoked when the user presses Ctrl+C
$null = Register-EngineEvent -SourceIdentifier PowerShell.Exiting -Action {
    Write-Host "Cleaning up before exit..."
    Stop-ProcessGracefully -Process $process
} -SupportEvent

$process = Start-Process -PassThru -FilePath "yarn.cmd" -ArgumentList "dev" -WorkingDirectory "OGame.MVC/wwwroot" -WindowStyle Hidden

while ($true) {
    $process.WaitForExit()

    # Check if process was not externally stopped
    if ($process.ExitCode -ne 0) {
        $process = Start-Process -PassThru -FilePath "yarn.cmd" -ArgumentList "dev" -WorkingDirectory "OGame.MVC/wwwroot" -WindowStyle Hidden
    } else {
        break
    }
}
