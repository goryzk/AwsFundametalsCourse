using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerDeletedHandler(ILogger<CustomerCreatedHandler> logger) : IRequestHandler<CustomerDeleted>
{
    public async Task Handle(CustomerDeleted request, CancellationToken cancellationToken)
    {
        logger.LogInformation(request.Id.ToString());
        await Task.CompletedTask;
    }
}