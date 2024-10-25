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
    dotnet workload install maccatalyst --version 9.0.100-rc.1.24453.3 --source https://api.nuget.org/v3/index.json

    if [[ $? != 0 ]]; then
        Write-PipelineTelemetryError -category 'InitializeToolset' "Failed to install workloads."
        ExitWithExitCode 1
    fi

    echo "Workloads installed successfully."

    return 0
}

InstallWorkloads
