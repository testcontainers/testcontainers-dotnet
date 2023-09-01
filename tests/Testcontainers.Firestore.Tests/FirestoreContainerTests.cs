using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Commons;
using Google.Api.Gax;
using Google.Apis.Http;
using Google.Cloud.Firestore;

namespace Testcontainers.Firestore.Tests;

public class FirestoreContainerTests : IAsyncLifetime
{
  private readonly FirestoreContainer _firestoreContainer = new FirestoreBuilder().Build();

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task SaveAndReadDataTest()
  {
    const string collectionName = "test";
    _firestoreContainer.SetEmulatorHost();
    var builder = new FirestoreDbBuilder()
    {
      ProjectId = "test",
      EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
    };

    var firestore = builder.Build();

    var testObject = new Dictionary<string, object>() {
        { "name", "John" },
        { "id", Guid.NewGuid().ToString() }
      };

    await firestore.Collection(collectionName).Document().SetAsync(testObject);

    var data = await firestore.Collection(collectionName).GetSnapshotAsync();
    var receivedObject = data.Documents.Select(x => x.ToDictionary()).FirstOrDefault();

    foreach (var (k, v) in testObject)
    {
      Assert.Equal(v, receivedObject[k]);
    }

  }

  public Task InitializeAsync()
  {
    return _firestoreContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _firestoreContainer.DisposeAsync().AsTask();
  }
}

