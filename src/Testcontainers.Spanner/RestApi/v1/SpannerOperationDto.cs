namespace Testcontainers.Spanner.RestApi.v1;

public class SpannerOperationDto
{
  public string Name { get; set; } = string.Empty;
  public bool Done { get; set; }
  public SpannerStatusDto? Error { get; set; } 
  public object? Response { get; set; }
  public object? MetaData { get; set; }
}
