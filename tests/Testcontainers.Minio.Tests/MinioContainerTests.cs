using System;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace Testcontainers.Minio.Tests.Container;

public sealed class MinioContainerTests : IAsyncLifetime
{
    private readonly MinioContainer _minioTestcontainer;

    public MinioContainerTests()
    {
        _minioTestcontainer = new MinioBuilder().Build();
    }

    [Fact]
    public async Task TestMinio()
    {
        const string bucketName = "somebucket";
        await _minioTestcontainer.StartAsync();
        var config = new AmazonS3Config
        {
            AuthenticationRegion = "eu-west-1",
            ServiceURL = _minioTestcontainer.GetMinioUrl(),
            UseHttp = true,
            ForcePathStyle = true
        };
        var s3 = new AmazonS3Client(_minioTestcontainer.GetAccessId(), _minioTestcontainer.GetAccessKey(), config);

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
        const string fileName = "jp2137.jpg";
        await _minioTestcontainer.StartAsync();
        var config = new AmazonS3Config
        {
            AuthenticationRegion = "eu-west-1",
            ServiceURL = _minioTestcontainer.GetMinioUrl(),
            UseHttp = true,
            ForcePathStyle = true
        };
        var s3 = new AmazonS3Client(_minioTestcontainer.GetAccessId(), _minioTestcontainer.GetAccessKey(), config);

        await s3.PutBucketAsync(bucketName);

        await using var file = File.OpenRead($"./{fileName}");

        await s3.PutObjectAsync(new PutObjectRequest()
        {
            Key = fileName,
            BucketName = bucketName,
            InputStream = file,
        });

        var subject = await s3.GetObjectAsync(new GetObjectRequest() { Key = fileName, BucketName = bucketName });

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

    public async Task InitializeAsync()
    {
        await _minioTestcontainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _minioTestcontainer.StopAsync();
    }
}