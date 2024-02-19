using System.Text.Json;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;

var client = new AmazonSQSClient(RegionEndpoint.USEast1);

var queueUrlResponse = await client.GetQueueUrlAsync("customers");

var sendMessageRequest = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(new CustomerCreated
    {
        Email = "test@gmail.com",
        Id = Guid.NewGuid(),
        Name = "Gagooo"
    })
};

var response = await client.SendMessageAsync(sendMessageRequest);

Console.ReadKey();