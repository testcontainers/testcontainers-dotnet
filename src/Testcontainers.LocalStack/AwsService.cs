namespace Testcontainers.Minio;

public sealed class AwsService
{
    public string Name { get; }

    private AwsService(string name)
    {
        Name = name;
    }
    
    public static readonly AwsService ApiGateway = new("apigateway");
    public static readonly AwsService Ec2 = new AwsService("ec2");
    public static readonly AwsService Kinesis = new AwsService("kinesis");
    public static readonly AwsService DynamoDb = new("dynamodb");
    public static readonly AwsService DynamoDbStreams = new("dynamodbstreams");
    public static readonly AwsService S3 = new("s3");
    public static readonly AwsService Firehose = new("firehose");
    public static readonly AwsService Sqs = new("sqs");
}