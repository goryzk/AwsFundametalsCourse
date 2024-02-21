using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;

namespace Customers.Api.Messaging;

public class SnsMessenger : ISnsMessenger
{
    private readonly IAmazonSimpleNotificationService _amazonSns;
    private readonly TopicSettings _options;
    private string _topicArn;
    
    public SnsMessenger(IAmazonSimpleNotificationService amazonSns, IOptions<TopicSettings> options)
    {
        _amazonSns = amazonSns;
        _options = options.Value;
    }

    public async Task<PublishResponse> PublishMessageAsync<T>(T message)
    {
        var topicArn = await GetTopicArnAsync();
        var sendMessageRequest = new PublishRequest
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(message),
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

        var response = await _amazonSns.PublishAsync(sendMessageRequest);
        return response;
    }

    private async ValueTask<string> GetTopicArnAsync()
    {
        if (!string.IsNullOrEmpty(_topicArn))
        {
            return _topicArn;
        }
        
        var queueUrlResponse = await _amazonSns.FindTopicAsync(_options.Name);
        _topicArn = queueUrlResponse.TopicArn;
        return _topicArn;
    }
}