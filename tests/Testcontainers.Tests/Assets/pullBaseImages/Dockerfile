ARG REPO=mcr.microsoft.com/dotnet/aspnet
FROM $REPO:6.0.21-jammy-amd64
FROM ${REPO}:6.0.21-jammy-amd64
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
FROM build
FROM build AS publish
FROM mcr.microsoft.com/dotnet/aspnet:6.0.21-jammy-amd64
