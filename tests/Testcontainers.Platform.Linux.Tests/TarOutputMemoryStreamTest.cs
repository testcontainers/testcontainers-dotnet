namespace Testcontainers.Tests;

public sealed class TarOutputMemoryStreamTest
{
    [Theory]
    [InlineData(Unix.FileMode644, "644")]
    [InlineData(Unix.FileMode666, "666")]
    [InlineData(Unix.FileMode700, "700")]
    [InlineData(Unix.FileMode755, "755")]
    [InlineData(Unix.FileMode777, "777")]
    public void UnixFileModeResolvesToPosixFilePermission(UnixFileMode fileMode, string posixFilePermission)
    {
        Assert.Equal(Convert.ToInt32(posixFilePermission, 8), Convert.ToInt32(fileMode));
    }
}