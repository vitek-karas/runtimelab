#!/usr/bin/env bash

function InstallWorkloads {
    # Check if dotnet is installed
    if ! command -v dotnet &> /dev/null
    then
        echo "dotnet could not be found. Aborting..."
        exit 1
    else
        echo "dotnet is installed at $(command -v dotnet)."
    fi

    # This workload requires Xcode 15.4
    if ! dotnet workload install maccatalyst --version 9.0.100-rc.1.24453.3 --source https://api.nuget.org/v3/index.json; then
        echo "Failed to install workloads. Skipping..."
    else
        echo "Workloads installed successfully."
    fi

    return 0
}

InstallWorkloads
