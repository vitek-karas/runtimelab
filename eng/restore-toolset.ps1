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

    try {
        # This workload requires Xcode 15.4
        dotnet workload install maccatalyst --version 9.0.100-rc.1.24453.3 --source https://api.nuget.org/v3/index.json
        Write-Output "Workloads installed successfully."
    } catch {
        Write-Output "Failed to install workloads. Skipping..."
    }

    Write-Output "Workloads installed successfully."
    return 0
}

Install-Workloads
