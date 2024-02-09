namespace WeatherForecast.Tests;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

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
        .ConfigureAwait(true);

      var weatherForecastStream = await response.Content.ReadAsStreamAsync()
        .ConfigureAwait(true);

      var weatherForecast = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherData>>(weatherForecastStream)
        .ConfigureAwait(true);

      // Then
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      Assert.Equal(7, weatherForecast!.Count());
    }
  }

  public sealed class Web : IClassFixture<WeatherForecastContainer>
  {
    private static readonly ChromeOptions ChromeOptions = new ChromeOptions();

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
    public void Get_WeatherForecast_ReturnsSevenDays()
    {
      // Given
      string ScreenshotFileName() => $"{nameof(Get_WeatherForecast_ReturnsSevenDays)}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.png";

      using var chrome = new ChromeDriver(ChromeOptions);

      // When
      chrome.Navigate().GoToUrl(_weatherForecastContainer.BaseAddress);

      chrome.GetScreenshot().SaveAsFile(Path.Combine(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath, ScreenshotFileName()));

      chrome.FindElement(By.TagName("fluent-button")).Click();

      var wait = new WebDriverWait(chrome, TimeSpan.FromSeconds(10));
      wait.Until(webDriver => 1.Equals(webDriver.FindElements(By.TagName("span")).Count));

      chrome.GetScreenshot().SaveAsFile(Path.Combine(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath, ScreenshotFileName()));

      // Then
      Assert.Equal(7, int.Parse(chrome.FindElement(By.TagName("span")).Text, NumberStyles.Integer, CultureInfo.InvariantCulture));
    }
  }
}
