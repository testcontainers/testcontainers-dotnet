using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBStreams;
using Amazon.Runtime;
using Xunit;
using Ddb = Amazon.DynamoDBv2;
using StreamsModel = Amazon.DynamoDBStreams.Model;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerDynamoDbStreamsTest(FlociContainerFixture fixture)
{
    private (AmazonDynamoDBClient db, AmazonDynamoDBStreamsClient streams) CreateClients()
    {
        var creds = new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey());
        var url = fixture.Container.GetConnectionString();
        var db = new AmazonDynamoDBClient(creds, new AmazonDynamoDBConfig { ServiceURL = url, AuthenticationRegion = FlociContainer.Region });
        var streams = new AmazonDynamoDBStreamsClient(creds, new AmazonDynamoDBStreamsConfig { ServiceURL = url, AuthenticationRegion = FlociContainer.Region });
        return (db, streams);
    }

    [Fact]
    public async Task DynamoDbStreams_ListStreamsForTable_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        var (db, streams) = CreateClients();
        using var _ = db;
        using var __ = streams;
        var tableName = $"table-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await db.CreateTableAsync(new CreateTableRequest
        {
            TableName = tableName,
            KeySchema = [new KeySchemaElement("Id", Ddb.KeyType.HASH)],
            AttributeDefinitions = [new AttributeDefinition("Id", ScalarAttributeType.S)],
            BillingMode = BillingMode.PAY_PER_REQUEST,
            StreamSpecification = new StreamSpecification { StreamEnabled = true, StreamViewType = Ddb.StreamViewType.NEW_AND_OLD_IMAGES },
        }, ct);
        var response = await streams.ListStreamsAsync(new StreamsModel.ListStreamsRequest { TableName = tableName }, ct);

        Assert.Contains(response.Streams, s => s.TableName == tableName);
    }

    [Fact]
    public async Task DynamoDbStreams_ReadRecords_ContainsPutItem()
    {
        var ct = TestContext.Current.CancellationToken;
        var (db, streamsClient) = CreateClients();
        using var _ = db;
        using var __ = streamsClient;
        var tableName = $"table-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await db.CreateTableAsync(new CreateTableRequest
        {
            TableName = tableName,
            KeySchema = [new KeySchemaElement("Id", Ddb.KeyType.HASH)],
            AttributeDefinitions = [new AttributeDefinition("Id", ScalarAttributeType.S)],
            BillingMode = BillingMode.PAY_PER_REQUEST,
            StreamSpecification = new StreamSpecification { StreamEnabled = true, StreamViewType = Ddb.StreamViewType.NEW_IMAGE },
        }, ct);
        await db.PutItemAsync(new PutItemRequest
        {
            TableName = tableName,
            Item = new Dictionary<string, AttributeValue> { ["Id"] = new AttributeValue { S = "item-1" } },
        }, ct);

        var streamsResponse = await streamsClient.ListStreamsAsync(new StreamsModel.ListStreamsRequest { TableName = tableName }, ct);
        var streamArn = streamsResponse.Streams[0].StreamArn;
        var descResponse = await streamsClient.DescribeStreamAsync(new StreamsModel.DescribeStreamRequest { StreamArn = streamArn }, ct);
        var shardId = descResponse.StreamDescription.Shards[0].ShardId;
        var iteratorResponse = await streamsClient.GetShardIteratorAsync(new StreamsModel.GetShardIteratorRequest
        {
            StreamArn = streamArn,
            ShardId = shardId,
            ShardIteratorType = ShardIteratorType.TRIM_HORIZON,
        }, ct);
        var recordsResponse = await streamsClient.GetRecordsAsync(new StreamsModel.GetRecordsRequest { ShardIterator = iteratorResponse.ShardIterator }, ct);

        Assert.Contains(recordsResponse.Records, r => r.Dynamodb.Keys.ContainsKey("Id"));
    }
}
