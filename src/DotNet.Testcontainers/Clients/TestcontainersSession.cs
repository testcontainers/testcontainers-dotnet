namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.IO;
  using System.IO.MemoryMappedFiles;
  using System.Threading;

  internal sealed class TestcontainersSession : IDisposable
  {
    // TODO: Use proper name.
    private const string mutexName = "FOO";

    private const string mapName = "BAR";

    private readonly MemoryMappedFile sharedMemoryMappedFile;

    public TestcontainersSession()
    {
      using (var mutex = new Mutex(false, mutexName))
      {
        bool isMutexAcquired;

        try
        {
          isMutexAcquired = mutex.WaitOne(TimeSpan.FromSeconds(30));
        }
        catch (AbandonedMutexException)
        {
          isMutexAcquired = true;
        }

        if (!isMutexAcquired)
        {
          throw new TimeoutException(string.Empty);
        }

        var guidBytes = Guid.NewGuid().ToByteArray();

        try
        {
          // If the memory mapped file exists read the shared session id.
          this.sharedMemoryMappedFile = MemoryMappedFile.OpenExisting(mapName);
          using (var accessor = this.sharedMemoryMappedFile.CreateViewAccessor())
          {
            accessor.ReadArray(0, guidBytes, 0, guidBytes.Length);
          }
        }
        catch (FileNotFoundException)
        {
          // Otherwise, create new memory mapped file and session id.
          this.sharedMemoryMappedFile = MemoryMappedFile.CreateNew(mapName, guidBytes.Length);
          using (var accessor = this.sharedMemoryMappedFile.CreateViewAccessor())
          {
            accessor.WriteArray(0, guidBytes, 0, guidBytes.Length);
          }
        }
        finally
        {
          this.Id = new Guid(guidBytes);
          mutex.ReleaseMutex();
        }
      }
    }

    ~TestcontainersSession()
    {
      this.Dispose();
    }

    public Guid Id { get; }

    public void Dispose()
    {
      this.sharedMemoryMappedFile.Dispose();
      GC.SuppressFinalize(this);
    }
  }
}
