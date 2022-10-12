FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ARG CSPROJ_FILE_PATH="src/WeatherForecast/WeatherForecast.csproj"
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
WORKDIR /app

COPY . .

RUN dotnet restore $CSPROJ_FILE_PATH

RUN dotnet publish $CSPROJ_FILE_PATH --configuration Release --output out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
WORKDIR /app

EXPOSE 443

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WeatherForecast.dll"]
