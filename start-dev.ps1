param(
    [Parameter(Mandatory=$false)]
    [bool]$RunDotNetWatch = $true
)

# Set the error action preference to stop so that the script stops if an error occurs
$ErrorActionPreference = "Stop"

# Function to check if all containers are up
function ContainersAreUp {
    $containers = "ogame-v3_db_1", "ogame-v3_cache_1"
    $runningContainers = podman container list --format "{{.Names}}"

    # Check if all containers are in the running containers list
    foreach ($container in $containers) {
        if ($runningContainers -notcontains $container) {
            return $false
        }
    }

    return $true
}

# Run podman machine start if it's not running (run podman machine inspect and check for "State": "stopped" in the first object in the array)
if ((podman machine inspect | ConvertFrom-Json)[0].State -eq "stopped") {
    podman machine start
}

# Start vite-watcher.ps1 script
Start-Process -FilePath "powershell" -ArgumentList "-File .\vite-watcher.ps1" -NoNewWindow

# Start podman-compose up in a separate process
Start-Process -FilePath "podman-compose" -ArgumentList "up" -NoNewWindow

# Wait until all containers are up
while (-not (ContainersAreUp)) {
    Write-Host "Waiting for containers to start..."
    Start-Sleep -Seconds 5
}

Write-Host "Containers are up!"

if ($RunDotNetWatch) {
    $env:DOTNET_WATCH_RESTART_ON_RUDE_EDIT = "true"

    # Run dotnet watch
    Start-Process -FilePath "dotnet" -ArgumentList "watch" -WorkingDirectory "OGame.MVC" -NoNewWindow
}
