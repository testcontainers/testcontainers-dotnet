namespace DotNet.Testcontainers
{
  internal static class TarArchiveDefaults
  {
    // Keep the record size equal to the block size (512 B) so SharpZipLib does
    // not write extra zero padding beyond the two standard EOF blocks. The
    // default factor of 20 produces ~8 KB of zeros after EOF, which can trigger
    // a race in Podman's archive handler. The tar subprocess exits after the EOF
    // blocks while the HTTP sender is still flushing the padding, causing EPIPE
    // (HTTP 500 "broken pipe"). See:
    // https://github.com/testcontainers/testcontainers-dotnet/issues/1683.
    internal const int TarBlockFactor = 1;
  }
}
