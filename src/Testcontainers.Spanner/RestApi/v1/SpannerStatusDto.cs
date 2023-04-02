namespace Testcontainers.Spanner.RestApi.v1;

public class SpannerStatusDto
{
  public int Code { get; set; }
  public string? Message { get; set; }
  public object[]? Details { get; set; }
}
