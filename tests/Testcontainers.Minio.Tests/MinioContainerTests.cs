﻿using System;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace Testcontainers.Minio.Tests.Container;

public sealed class MinioContainerTests : IAsyncLifetime, IDisposable
{
    private const string TestFileContent = "👋";
    private readonly string TestFileContentFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
    
    private readonly MinioContainer _minioContainer;

    public MinioContainerTests()
    {
        _minioContainer = new MinioBuilder().Build();
    }

    [Fact]
    public async Task TestMinio()
    {
        const string bucketName = "somebucket";
        var config = new AmazonS3Config
        {
            AuthenticationRegion = "eu-west-1",
            ServiceURL = _minioContainer.GetMinioUrl(),
            UseHttp = true,
            ForcePathStyle = true,
        };
        var s3 = new AmazonS3Client(_minioContainer.GetAccessId(), _minioContainer.GetAccessKey(), config);

        await s3.PutBucketAsync(bucketName);

        var buckets = await s3.ListBucketsAsync();

        Assert.NotNull(buckets);
        Assert.NotNull(buckets.Buckets);
        Assert.NotEmpty(buckets.Buckets);
        Assert.Contains(buckets.Buckets, bucket => bucket.BucketName == bucketName);
    }
    
    [Fact]
    public async Task TestInsertAndGetDataFromMinio()
    {
        const string bucketName = "somebucket2";
        var config = new AmazonS3Config
        {
            AuthenticationRegion = "eu-west-1",
            ServiceURL = _minioContainer.GetMinioUrl(),
            UseHttp = true,
            ForcePathStyle = true,
        };
        var s3 = new AmazonS3Client(_minioContainer.GetAccessId(), _minioContainer.GetAccessKey(), config);

        await s3.PutBucketAsync(bucketName);
        await using var file = File.OpenRead(TestFileContentFilePath);

        await s3.PutObjectAsync(new PutObjectRequest()
        {
            Key = TestFileContentFilePath,
            BucketName = bucketName,
            InputStream = file,
        });

        var subject = await s3.GetObjectAsync(new GetObjectRequest() { Key = TestFileContentFilePath, BucketName = bucketName });

        Assert.NotNull(subject);
        Assert.NotEqual(0, subject.ContentLength);
    }
    
    
    [Fact]
    public void TestMinioWithEmptyUsername()
    {
        var ct = new MinioBuilder().WithUsername(string.Empty);

        Assert.Throws<ArgumentException>(() => ct.Build());
    }
    
    [Fact]
    public void TestMinioWithEmptyPassword()
    {
        var ct = new MinioBuilder().WithPassword(string.Empty);

        Assert.Throws<ArgumentException>(() => ct.Build());
    }

    public Task InitializeAsync()
    {
        File.WriteAllText(TestFileContentFilePath, TestFileContent); 
        return _minioContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _minioContainer.DisposeAsync().AsTask();
    }

    public void Dispose()
    {
        if (File.Exists(TestFileContentFilePath))
        {
            File.Delete(TestFileContentFilePath);
        }
    }
}