namespace Testcontainers.MiniStack;

public sealed class MiniStackBuilderTest
{
    [Fact]
    public void DefaultImageIsMiniStack()
    {
#pragma warning disable CS0618
        Assert.StartsWith("ministackorg/ministack", MiniStackBuilder.MiniStackImage);
#pragma warning restore CS0618
    }

    [Fact]
    public void AnyVersionWithoutAuthTokenDoesNotThrowArgumentException()
    {
        var exception = Xunit.Record.Exception(() => new MiniStackBuilder("ministackorg/ministack:latest").Build());
        Assert.Null(exception);
    }

    [Fact]
    public void HighVersionWithoutAuthTokenDoesNotThrowArgumentException()
    {
        var exception = Xunit.Record.Exception(() => new MiniStackBuilder("ministackorg/ministack:5.0.0").Build());
        Assert.Null(exception);
    }

    [Fact]
    public void CustomImageDoesNotThrowArgumentException()
    {
        var exception = Xunit.Record.Exception(() => new MiniStackBuilder("ministackorg/ministack:1.0.0").Build());
        Assert.Null(exception);
    }
}
