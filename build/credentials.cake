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
  public string Key { get; private set; }
  public string Url { get; private set; }
  public string Token { get; private set; }

  private SonarQubeCredentials(string key, string url, string token)
  {
    Key = key;
    Url = url;
    Token = token;
  }

  public static SonarQubeCredentials GetSonarQubeCredentials(ICakeContext context)
  {
    return new SonarQubeCredentials
    (
      context.EnvironmentVariable("SONARQUBE_KEY"),
      context.EnvironmentVariable("SONARQUBE_URL"),
      context.EnvironmentVariable("SONARQUBE_TOKEN")
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
      context.EnvironmentVariable("NUGET_USERNAME"),
      context.EnvironmentVariable("NUGET_PASSWORD"),
      context.EnvironmentVariable("NUGET_SOURCE"),
      context.EnvironmentVariable("NUGET_API_KEY")
    );
  }
}
