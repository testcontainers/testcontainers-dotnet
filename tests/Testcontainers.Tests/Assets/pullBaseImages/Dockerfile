ARG REPO=mcr.microsoft.com/dotnet/aspnet
ARG PLATFORM=linux/arm64

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
FROM build
FROM build AS publish
FROM $REPO:8.0-jammy
FROM ${REPO}:8.0-noble
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

# https://github.com/testcontainers/testcontainers-dotnet/issues/993.
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0
FROM --platform=$PLATFORM mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0
FROM --platform="linux/arm/v6" mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0
FROM --platform='linux/arm/v7' mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0

# https://github.com/testcontainers/testcontainers-dotnet/issues/1030.
FROM mcr.microsoft.com/dotnet/sdk:$SDK_VERSION_8_0 AS build_sdk_8_0
FROM build_sdk_8_0 AS publish_sdk_8_0
