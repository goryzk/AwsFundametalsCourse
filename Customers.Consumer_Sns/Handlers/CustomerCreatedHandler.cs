using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger) : IRequestHandler<CustomerCreated>
{
    public async Task Handle(CustomerCreated request, CancellationToken cancellationToken)
    {
        logger.LogInformation(request.FullName);
        await Task.CompletedTask;
    }
}