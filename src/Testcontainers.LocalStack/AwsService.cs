namespace Testcontainers.Minio;

/// <summary>
/// AwsService
/// </summary>
[PublicAPI]
public sealed class AwsService
{
    /// <summary>
    /// The name of the AWS service.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the `AwsService` class.
    /// </summary>
    /// <param name="name">The name of the AWS service.</param>
    private AwsService(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Represents the AWS API Gateway service. <a href="https://aws.amazon.com/api-gateway/">See docs</a>
    /// </summary>
    public static readonly AwsService ApiGateway = new AwsService("apigateway");

    /// <summary>
    /// Represents the AWS Elastic Compute Cloud (EC2) service. <a href="https://aws.amazon.com/ec2/">See docs</a>
    /// </summary>
    public static readonly AwsService Ec2 = new AwsService("ec2");

    /// <summary>
    /// Represents the AWS Kinesis service. <a href="https://aws.amazon.com/kinesis/">See docs</a>
    /// </summary>
    public static readonly AwsService Kinesis = new AwsService("kinesis");

    /// <summary>
    /// Represents the AWS DynamoDB service. <a href="https://aws.amazon.com/dynamodb/">See docs</a>
    /// </summary>
    public static readonly AwsService DynamoDb = new AwsService("dynamodb");

    /// <summary>
    /// Represents the AWS DynamoDB Streams service. <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Streams.html">See docs</a>
    /// </summary>
    public static readonly AwsService DynamoDbStreams = new AwsService("dynamodbstreams");

    /// <summary>
    /// Represents the AWS Simple Storage Service (S3) service. <a href="https://aws.amazon.com/s3/">See docs</a>
    /// </summary>
    public static readonly AwsService S3 = new AwsService("s3");

    /// <summary>
    /// Represents the AWS Kinesis Firehose service. <a href="https://aws.amazon.com/firehose/">See docs</a>
    /// </summary>
    public static readonly AwsService Firehose = new AwsService("firehose");

    /// <summary>
    /// Represents the AWS Lambda service. <a href="https://aws.amazon.com/lambda/">See docs</a>
    /// </summary>
    public static readonly AwsService Lambda = new AwsService("lambda");

    /// <summary>
    /// Represents the AWS Simple Notification Service (SNS) service. <a href="https://aws.amazon.com/sns/">See docs</a>
    /// </summary>
    public static readonly AwsService Sns = new AwsService("sns");

    /// <summary>
    /// Represents the AWS Redshift service. <a href="https://aws.amazon.com/redshift/">See docs</a>
    /// </summary>
    public static readonly AwsService Redshift = new AwsService("redshift");

    /// <summary>
    /// Represents the AWS Simple Email Service (SES) service. <a href="https://aws.amazon.com/ses/">See docs</a>
    /// </summary>
    public static readonly AwsService Ses = new AwsService("ses");

    /// <summary>
    /// Represents the AWS Simple Queue Service (SQS) service. <a href="https://aws.amazon.com/sqs/">See docs</a>
    /// </summary>
    public static readonly AwsService Sqs = new AwsService("sqs");

    /// <summary>
    /// Represents the AWS Route53 service. <a href="https://aws.amazon.com/route53/">See docs</a>
    /// </summary>
    public static readonly AwsService Route53 = new AwsService("route53");

    /// <summary>
    /// Enables AWS CloudFormation Service. <a href="https://aws.amazon.com/cloudformation/">See docs</a>
    /// </summary>
    public static readonly AwsService CloudFormation = new AwsService("cloudformation");

    /// <summary>
    /// Enables AWS CloudWatch Service. <a href="https://aws.amazon.com/cloudwatch/">See docs</a>
    /// </summary>
    public static readonly AwsService CloudWatch = new AwsService("cloudwatch");

    /// <summary>
    /// Enables AWS Simple System Manager (SSM) Service. <a href="https://aws.amazon.com/ssm/">See docs</a>
    /// </summary>
    public static readonly AwsService Ssm = new AwsService("ssm");

    /// <summary>
    /// Enables AWS Secrets Manager Service. <a href="https://aws.amazon.com/secrets-manager/">See docs</a>
    /// </summary>
    public static readonly AwsService SecretsManager = new AwsService("secretsmanager");

    /// <summary>
    /// Enables AWS Step Functions Service. <a href="https://aws.amazon.com/step-functions/">See docs</a>
    /// </summary>
    public static readonly AwsService StepFunctions = new AwsService("stepfunctions");

    /// <summary>
    /// Enables AWS CloudWatch Logs Service. <a href="https://aws.amazon.com/cloudwatch/">See docs</a>
    /// </summary>
    public static readonly AwsService CloudWatchLogs = new AwsService("logs");

    /// <summary>
    /// Enables AWS Security Token Service (STS) Service. <a href="https://aws.amazon.com/sts/">See docs</a>
    /// </summary>
    public static readonly AwsService Sts = new AwsService("sts");

    /// <summary>
    /// Enables AWS Identity and Access Management (IAM) Service. <a href="https://aws.amazon.com/iam/">See docs</a>
    /// </summary>
    public static readonly AwsService Iam = new AwsService("iam");

    /// <summary>
    /// Enables AWS Key Management Service (KMS) Service. <a href="https://aws.amazon.com/kms/">See docs</a>
    /// </summary>
    public static readonly AwsService Kms = new AwsService("kms");
}