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
    //
    // This only applies to archives we stream to the daemon over a pipe (resource
    // mappings via a MemoryStream), where the surplus padding is both unnecessary
    // and harmful. Do NOT reuse it for the Dockerfile build context, which is
    // written to a FileStream on disk: a 512 B record forces a write and flush per
    // block and regresses image builds. That path keeps SharpZipLib's default block
    // factor on purpose.
    internal const int TarBlockFactor = 1;
  }
}
