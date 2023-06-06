# Define a function to stop the process
function Stop-ProcessGracefully {
    param($Process)

    if (!$Process.HasExited) {
        $Process | Stop-Process -Force
    }
}

function Close-ViteServer {
    # read OGame.MVC/wwwroot/vite.config.ts to get the port
    $vite_config = Get-Content -Path "OGame.MVC/wwwroot/vite.config.ts" -Raw
    $port = $vite_config -match 'port: (\d+)'
    $port = $Matches[1]

    $connection = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
    if ($connection) {
        $process = Get-Process -Id $connection.OwningProcess
        if ($process) {
            echo "Server found, stopping..."
            Stop-ProcessGracefully -Process $process
        }

        echo "Server restarting..."
    }
}

echo "Checking for running server..."
Close-ViteServer

# run yarn install
echo "Installing dependencies..."
yarn --cwd "OGame.MVC/wwwroot" install

$process = Start-Process -PassThru -FilePath "yarn.cmd" -ArgumentList "dev" -WorkingDirectory "OGame.MVC/wwwroot" -WindowStyle Hidden -RedirectStandardError "OGame.MVC/wwwroot/vite-error.log" -RedirectStandardOutput "OGame.MVC/wwwroot/vite.log"

echo "Server started"

# Register a script block that will be invoked when the user presses Ctrl+C
$null = Register-EngineEvent -SourceIdentifier PowerShell.Exiting -Action {
    Write-Host "Cleaning up before exit..."
    Stop-Process -Id $process.Id -Force
} -SupportEvent

$continueLoop = $true

while ($continueLoop) {
    if ($process.WaitForExit(1000)) {
        # Check if process was not externally stopped
        if ($process.ExitCode -ne 0) {
            $exitCode = $process.ExitCode

            $process = Start-Process -PassThru -FilePath "yarn.cmd" -ArgumentList "dev" -WorkingDirectory "OGame.MVC/wwwroot" -WindowStyle Hidden -RedirectStandardError "OGame.MVC/wwwroot/vite-error.log" -RedirectStandardOutput "OGame.MVC/wwwroot/vite.log"

            echo "Server crashed with exit code $($exitCode). Restarted."
        } else {
            $continueLoop = $false
        }
    }

    if ($Host.UI.RawUI.KeyAvailable) {
        $key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

        # Check if the key pressed is Ctrl+C
        if ($key.VirtualKeyCode -eq 67 -and ($key.ControlKeyState -band 1)) {
            Write-Host "Ctrl+C detected. Cleaning up before exit..."
            Stop-Process -Id $process.Id -Force
            $continueLoop = $false
        }
    }
}

Close-ViteServer
