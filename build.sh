#!/bin/sh

set -e

dotnet run --project ./build/Build.csproj -- "$@"
