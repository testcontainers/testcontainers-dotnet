namespace DotNet.Testcontainers.Configurations
{
  using System.Text.Json;

  public static class DefaultJsonSerializerOptions
  {
    static DefaultJsonSerializerOptions()
    {
      Instance.Converters.Add(new JsonOrderedKeysConverter());
    }

    public static JsonSerializerOptions Instance { get; } = new JsonSerializerOptions();
  }
}
