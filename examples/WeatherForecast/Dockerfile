FROM mcr.microsoft.com/dotnet/sdk:11.0.100@sha256:2794448a07118a6eaf00f29ab5d2bf44928517ce5ff86505a63a69230374f599 AS build-env
ARG CSPROJ_FILE_PATH="src/WeatherForecast/WeatherForecast.csproj"
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
WORKDIR /app

COPY . .

RUN dotnet restore $CSPROJ_FILE_PATH --locked-mode

RUN dotnet publish $CSPROJ_FILE_PATH --configuration Release --framework net11.0 --runtime linux-x64 --self-contained false --output out /p:DebugType=None /p:DebugSymbols=false

FROM mcr.microsoft.com/dotnet/aspnet:11.0.0@sha256:ca396ece3f2a2e62c7a03f1d7d710b8bc7e8c9dea5597d091178d9f5bfca13f0
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
WORKDIR /app

EXPOSE 443

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WeatherForecast.dll"]
