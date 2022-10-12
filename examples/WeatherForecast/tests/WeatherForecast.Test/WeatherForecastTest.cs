using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WeatherForecast.Entities;
using Xunit;

namespace WeatherForecast.Test;

public static class WeatherForecastTest
{
  public sealed class Api : IClassFixture<WeatherForecastContainer>
  {
    private readonly WeatherForecastContainer _weatherForecastContainer;

    public Api(WeatherForecastContainer weatherForecastContainer)
    {
      _weatherForecastContainer = weatherForecastContainer;
      _weatherForecastContainer.SetBaseAddress();
    }

    [Fact]
    [Trait("Category", nameof(Api))]
    public async Task Get_WeatherForecast_ReturnsSevenDays()
    {
      // Given
      const string path = "api/WeatherForecast";

      // When
      var response = await _weatherForecastContainer.GetAsync(path)
        .ConfigureAwait(false);

      var weatherForecastStream = await response.Content.ReadAsStreamAsync()
        .ConfigureAwait(false);

      var weatherForecast = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherData>>(weatherForecastStream)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      Assert.Equal(7, weatherForecast!.Count());
    }
  }

  public sealed class Web : IClassFixture<WeatherForecastContainer>
  {
    private static readonly ChromeOptions ChromeOptions = new();

    private readonly WeatherForecastContainer _weatherForecastContainer;

    static Web()
    {
      ChromeOptions.AddArgument("headless");
      ChromeOptions.AddArgument("ignore-certificate-errors");
    }

    public Web(WeatherForecastContainer weatherForecastContainer)
    {
      _weatherForecastContainer = weatherForecastContainer;
      _weatherForecastContainer.SetBaseAddress();
    }

    [Fact]
    [Trait("Category", nameof(Web))]
    public async Task Get_WeatherForecast_ReturnsSevenDays()
    {
      // Given
      string ScreenshotFileName() => $"{nameof(Get_WeatherForecast_ReturnsSevenDays)}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.png";

      using var chrome = new ChromeDriver(ChromeOptions);

      // When
      chrome.Navigate().GoToUrl(_weatherForecastContainer.BaseAddress);

      chrome.GetScreenshot().SaveAsFile(Path.Combine(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath, ScreenshotFileName()));

      chrome.FindElement(By.TagName("fluent-button")).Click();

      await Task.Delay(TimeSpan.FromSeconds(1))
        .ConfigureAwait(false);

      chrome.GetScreenshot().SaveAsFile(Path.Combine(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath, ScreenshotFileName()));

      // Then
      Assert.Equal(7, int.Parse(chrome.FindElement(By.TagName("span")).Text, NumberStyles.Integer, CultureInfo.InvariantCulture));
    }
  }
}
