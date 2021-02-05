namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.IO;
  using System.IO.MemoryMappedFiles;
  using System.Threading;
  using DotNet.Testcontainers.Services;
  using Microsoft.Extensions.Logging;
  using Serilog;

  internal sealed class TestcontainersSession : IDisposable
  {
    private static readonly ILogger<TestcontainersSession> Logger = TestcontainersHostService.GetLogger<TestcontainersSession>();

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
            Logger.LogTrace("Read Guid.");
            accessor.ReadArray(0, guidBytes, 0, guidBytes.Length);
          }
        }
        catch (FileNotFoundException)
        {
          // Otherwise, create new memory mapped file and session id.
          this.sharedMemoryMappedFile = MemoryMappedFile.CreateNew(mapName, guidBytes.Length);
          using (var accessor = this.sharedMemoryMappedFile.CreateViewAccessor())
          {
            Logger.LogTrace("Write Guid.");
            accessor.WriteArray(0, guidBytes, 0, guidBytes.Length);
          }
        }
        catch (Exception e)
        {
          Logger.LogTrace(e.Message);
        }
        finally
        {
          Logger.LogTrace($"Id: {this.Id.ToString()}");
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
      if (this.sharedMemoryMappedFile != null)
      {
        this.sharedMemoryMappedFile.Dispose();
      }

      GC.SuppressFinalize(this);
    }
  }
}
