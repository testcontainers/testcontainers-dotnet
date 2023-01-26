internal sealed class BuildCredentials
{
  public string Username { get; private set; }
  public string Password { get; private set; }

  public BuildCredentials(string username, string password)
  {
    Username = username;
    Password = password;
  }

  public static BuildCredentials GetCodeSigningCertificateCredentials(ICakeContext context)
  {
    return new BuildCredentials
    (
      null,
      context.EnvironmentVariable("CODE_SIGNING_CERTIFICATE_PASSWORD")
    );
  }
}

internal sealed class SonarQubeCredentials
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

internal sealed class NuGetCredentials
{
  public string Source { get; private set; }
  public string ApiKey { get; private set; }

  private NuGetCredentials(string source, string apiKey)
  {
    Source = source;
    ApiKey = apiKey;
  }

  public static NuGetCredentials GetNuGetCredentials(ICakeContext context)
  {
    return new NuGetCredentials
    (
      context.EnvironmentVariable("FEED_SOURCE"),
      context.EnvironmentVariable("FEED_API_KEY")
    );
  }
}
