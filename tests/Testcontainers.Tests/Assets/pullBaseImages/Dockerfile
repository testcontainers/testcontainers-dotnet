ARG REPO=mcr.microsoft.com/dotnet/aspnet
ARG PLATFORM=linux/arm64

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Stage-local build arguments are out of scope for FROM instructions.
ARG REPO=stage-local
ARG TARGETPLATFORM=linux/stage-local

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

# Built-in build arguments are not declared with ARG.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build_platform
FROM --platform=${TARGETPLATFORM} mcr.microsoft.com/dotnet/runtime:8.0-alpine AS target_platform

# https://github.com/testcontainers/testcontainers-dotnet/issues/1030.
FROM mcr.microsoft.com/dotnet/sdk:$SDK_VERSION_8_0 AS build_sdk_8_0
FROM build_sdk_8_0 AS publish_sdk_8_0
