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
      context.EnvironmentVariable("SONARQUBE_URL"),
      context.EnvironmentVariable("SONARQUBE_KEY"),
      context.EnvironmentVariable("SONARQUBE_TOKEN"),
      context.EnvironmentVariable("SONARQUBE_ORGANIZATION")
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
      context.EnvironmentVariable("NUGET_USERNAME") ?? "",
      context.EnvironmentVariable("NUGET_PASSWORD") ?? "",
      context.EnvironmentVariable("NUGET_SOURCE"),
      context.EnvironmentVariable("NUGET_APIKEY")
    );
  }
}
