# Testcontainers for .NET WeatherForecast example

This example builds and ships a Blazor application in a Docker image build, runs a Docker container and executes tests against a running instance of the application. Testcontainers for .NET takes care of the Docker image build and the Docker container that hosts the application. Spin up as much as containers as you like and run your tests heavily in parallel. Checkout and run the tests on your machine:

```console
git lfs version
git clone --branch develop git@github.com:testcontainers/testcontainers-dotnet.git
cd ./testcontainers-dotnet/examples/WeatherForecast/
dotnet test WeatherForecast.sln --configuration=Release
```

_*) One unit test depends on Selenium and requires Chrome in version 106._
