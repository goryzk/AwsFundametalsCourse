using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;

namespace Customers.Consumer;

public class QueueConsumerService : BackgroundService
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly QueueSettings _queueSettings;
    private string _queueUrl;
    private readonly IMediator _mediator;
    private readonly ILogger<QueueConsumerService> _logger;

    public QueueConsumerService(IAmazonSQS amazonSqs, IOptions<QueueSettings> queueSettings, IMediator mediator, ILogger<QueueConsumerService> logger)
    {
        _amazonSqs = amazonSqs;
        _mediator = mediator;
        _logger = logger;
        _queueSettings = queueSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = await GetQueueUrl(stoppingToken);

        var receiveResponse = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            AttributeNames = new List<string>{"All"},
            MessageAttributeNames = new List<string>{"All"},
            MaxNumberOfMessages = 1,
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _amazonSqs.ReceiveMessageAsync(receiveResponse, stoppingToken);

            foreach (var message in response.Messages)
            {
                var messageType = message.MessageAttributes["MessageType"].StringValue;
                var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");
                if (type is null)
                {
                    _logger.LogWarning("oops.");
                }

                var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type);
                try
                {
                    await _mediator.Send(typedMessage, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError("Unable to send to mediator");
                }

                await _amazonSqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle, stoppingToken);
            }
            
            await Task.Delay(1000, stoppingToken);

        }

    }

    private async Task<string> GetQueueUrl(CancellationToken stoppingToken)
    {
        if (!string.IsNullOrEmpty(_queueUrl))
        {
            return _queueUrl;
        }

        var queueUrlResponse = await _amazonSqs.GetQueueUrlAsync(_queueSettings.Name, stoppingToken);
        _queueUrl = queueUrlResponse.QueueUrl;
        return _queueUrl;
    }
}