namespace Testcontainers.Tests
{
    public abstract class FileTestBase : IDisposable
    {
        protected readonly FileInfo _testFile = new FileInfo(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"), Path.GetRandomFileName()));

        private bool _disposed;

        protected FileTestBase()
        {
            _ = Directory.CreateDirectory(_testFile.Directory!.FullName);

            using var fileStream = _testFile.Open(FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            fileStream.WriteByte(13);
        }

        ~FileTestBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeManagedResources()
        {
            _testFile.Directory!.Delete(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                DisposeManagedResources();
            }

            _disposed = true;
        }
    }
}
