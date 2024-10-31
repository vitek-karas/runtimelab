#!/usr/bin/env pwsh

function Install-Workloads {
    if (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        Write-Host "No sudo privileges detected. Skipping dotnet workload install..."
    } else {
        # Check if dotnet is installed
        if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
            Write-Output "dotnet could not be found. Aborting..."
            exit 1
        }
        else {
            Write-Output "dotnet is installed at $(Get-Command dotnet)."
        }

        # This workload requires Xcode 15.4
        dotnet workload install maccatalyst --version 9.0.100-rc.1.24453.3 --source https://api.nuget.org/v3/index.json

        if ($LASTEXITCODE -ne 0) {
            Write-Output "Failed to install workloads."
            exit 1
        }

        Write-Output "Workloads installed successfully."
    }

    return 0
}

Install-Workloads
