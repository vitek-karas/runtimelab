#!/usr/bin/env pwsh

function Install-Workloads {
    # Check if dotnet is installed
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-Output "dotnet could not be found. Aborting..."
        exit 1
    }
    else {
        Write-Output "dotnet is installed at $(Get-Command dotnet)."
    }

    dotnet workload install maccatalyst

    if ($LASTEXITCODE -ne 0) {
        Write-Output "Failed to install workloads."
        exit 1
    }

    Write-Output "Workloads installed successfully."
    return 0
}

Install-Workloads
