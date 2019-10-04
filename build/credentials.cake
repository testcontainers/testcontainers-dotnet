internal class BuildCredentials
{
  public string Username { get; private set; }
  public string Password { get; private set; }

  public BuildCredentials(string username, string password)
  {
    Username = username;
    Password = password;
  }
}

internal class SonarQubeCredentials
{
  public string Url { get; private set; }
  public string Key { get; private set; }
  public string Token { get; private set; }
  public string Organization { get; private set; }

  private SonarQubeCredentials(string url, string key, string token, string organization)
  {
    Url = url;
    Key = key;
    Token = token;
    Organization = organization;
  }

  public static SonarQubeCredentials GetSonarQubeCredentials(ICakeContext context)
  {
    return new SonarQubeCredentials
    (
      context.EnvironmentVariable("SONARCLOUD_URL"),
      context.EnvironmentVariable("SONARCLOUD_KEY"),
      context.EnvironmentVariable("SONARCLOUD_TOKEN"),
      context.EnvironmentVariable("SONARCLOUD_ORGANIZATION")
    );
  }
}

internal class NuGetCredentials : BuildCredentials
{
  public string Source { get; private set; }
  public string ApiKey { get; private set; }

  private NuGetCredentials(string username, string password, string source, string apiKey) : base(username, password)
  {
    Source = source;
    ApiKey = apiKey;
  }

  public static NuGetCredentials GetNuGetCredentials(ICakeContext context)
  {
    return new NuGetCredentials
    (
      context.EnvironmentVariable("FEED_USERNAME") ?? "",
      context.EnvironmentVariable("FEED_PASSWORD") ?? "",
      context.EnvironmentVariable("FEED_SOURCE"),
      context.EnvironmentVariable("FEED_APIKEY")
    );
  }
}
