import boto3

boto3.client("sqs").create_queue(QueueName="seeded-queue")
