ARG REPO=mcr.microsoft.com/dotnet/aspnet
FROM $REPO:6.0.21-jammy-amd64
FROM ${REPO}:6.0.21-jammy-amd64
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
FROM build
FROM build AS publish
FROM mcr.microsoft.com/dotnet/aspnet:6.0.22-jammy-amd64

# https://github.com/testcontainers/testcontainers-dotnet/issues/993.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:6.0.23-jammy-amd64

# https://github.com/testcontainers/testcontainers-dotnet/issues/1030.
FROM mcr.microsoft.com/dotnet/sdk:$SDK_VERSION_6_0 AS build_sdk_6_0
FROM build_sdk_6_0 AS publish_sdk_6_0
