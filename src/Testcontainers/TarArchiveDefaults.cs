namespace DotNet.Testcontainers
{
  internal static class TarArchiveDefaults
  {
    // A block factor of 1 keeps the record size equal to the block size (512 B),
    // so SharpZipLib writes no extra zero padding beyond the two standard EOF
    // blocks. The default factor of 20 produces up to 9 KiB of trailing zero
    // padding, which can trigger a race in Podman's archive handler. The tar
    // subprocess exits after the EOF blocks while the HTTP sender is still
    // flushing the padding, resulting in an EPIPE (HTTP 500 "broken pipe").
    // See: https://github.com/testcontainers/testcontainers-dotnet/issues/1683.
    //
    // The race only affects PUT /containers/{id}/archive (resource mappings via
    // a MemoryStream). Buildah's copierHandlerPut returned immediately after
    // tar.Reader reached io.EOF without draining the pipe. POST /build is immune
    // because containers/storage's chrootarchive.Untar always drains stdin after
    // extraction. The server-side fix landed in buildah.
    // See: https://github.com/podman-container-tools/buildah/pull/6678.
    // Keeping the block factor at 1 stays correct for older Podman versions.
    //
    // Do NOT use this for the Dockerfile build context, which is written to a
    // FileStream on disk: at 512 B per record, SharpZipLib flushes on every
    // block, which regresses image build performance. That path intentionally
    // keeps SharpZipLib's default block factor.
    internal const int TarBlockFactor = 1;
  }
}
