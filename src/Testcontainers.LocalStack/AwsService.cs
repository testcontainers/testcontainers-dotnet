namespace Testcontainers.Minio;

[PublicAPI]
public sealed class AwsService
{
    public string Name { get; }

    private AwsService(string name)
    {
        Name = name;
    }
    
    public static readonly AwsService ApiGateway = new("apigateway");
    public static readonly AwsService Ec2 = new ("ec2");
    public static readonly AwsService Kinesis = new ("kinesis");
    public static readonly AwsService DynamoDb = new("dynamodb");
    public static readonly AwsService DynamoDbStreams = new("dynamodbstreams");
    public static readonly AwsService S3 = new("s3");
    public static readonly AwsService Firehose = new("firehose");
    public static readonly AwsService Lambda = new("lambda");
    public static readonly AwsService Sns = new("sns");
    public static readonly AwsService Redshift = new("redshift");
    public static readonly AwsService Ses = new("ses");
    public static readonly AwsService Sqs = new("sqs");
    public static readonly AwsService Route53 = new("route53");
    public static readonly AwsService CloudFormation = new("cloudformation");
    public static readonly AwsService CloudWatch = new("cloudwatch");
    public static readonly AwsService Ssm = new("ssm");
    public static readonly AwsService SecretsManager = new("secretsmanager");
    public static readonly AwsService StepFunctions = new("stepfunctions");
    public static readonly AwsService CloudWatchLogs = new("logs");
    public static readonly AwsService Sts = new("sts");
    public static readonly AwsService Iam = new("iam");
    public static readonly AwsService Kms = new("kms");
}