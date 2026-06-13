FROM mcr.microsoft.com/dotnet/sdk:10.0.3@sha256:e362a8dbcd691522456da26a5198b8f3ca1d7641c95624fadc5e3e82678bd08a AS build-env
ARG CSPROJ_FILE_PATH="src/WeatherForecast/WeatherForecast.csproj"
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
WORKDIR /app

COPY . .

RUN dotnet restore $CSPROJ_FILE_PATH --locked-mode

RUN dotnet publish $CSPROJ_FILE_PATH --configuration Release --framework net10.0 --runtime linux-x64 --self-contained false --output out /p:DebugType=None /p:DebugSymbols=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0.3@sha256:aec87aa74ddf129da573fa69f42f229a23c953a1c6fdecedea1aa6b1fe147d76
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
WORKDIR /app

EXPOSE 443

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WeatherForecast.dll"]
