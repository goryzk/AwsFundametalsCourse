using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

var cts = new CancellationTokenSource();
var sqsClient = new AmazonSQSClient(RegionEndpoint.USEast1);

var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var receiveMessageRequest = new ReceiveMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    AttributeNames = new List<string>{ "ALL" },
    MessageAttributeNames = new List<string>{ "ALL" },
};

while (!cts.IsCancellationRequested)
{
    var receiveMessageResponse = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cts.Token);
    foreach (var message in receiveMessageResponse.Messages)
    {
        Console.WriteLine($"Message Id: {message.MessageId}");
        Console.WriteLine($"Message Body: {message.Body}");

        await sqsClient.DeleteMessageAsync(receiveMessageRequest.QueueUrl, message.ReceiptHandle, cts.Token);
    }
}