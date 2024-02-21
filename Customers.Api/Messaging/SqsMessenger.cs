using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace Customers.Api.Messaging;

public class SqsMessenger : ISqsMessenger
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly QueueSettings _options;
    private string _queueUrl;
    
    public SqsMessenger(IAmazonSQS amazonSqs, IOptions<QueueSettings> options)
    {
        _amazonSqs = amazonSqs;
        _options = options.Value;
    }

    public async Task<SendMessageResponse> SendMessageAsync<T>(T message)
    {
        var queueUrl = await GetQueueUrl();
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType", new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    }
                }
            }
        };

        var response = await _amazonSqs.SendMessageAsync(sendMessageRequest);
        return response;
    }

    private async Task<string> GetQueueUrl()
    {
        if (!string.IsNullOrEmpty(_queueUrl))
        {
            return _queueUrl;
        }
        
        var queueUrlResponse = await _amazonSqs.GetQueueUrlAsync(_options.Name);
        _queueUrl = queueUrlResponse.QueueUrl;
        return _queueUrl;
    }
}