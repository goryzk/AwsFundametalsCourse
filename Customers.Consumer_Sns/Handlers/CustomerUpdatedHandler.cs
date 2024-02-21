using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerUpdatedHandler(ILogger<CustomerCreatedHandler> logger) : IRequestHandler<CustomerUpdated>
{
    public async Task Handle(CustomerUpdated request, CancellationToken cancellationToken)
    {
        logger.LogInformation(request.GitHubUserName);
        await Task.CompletedTask;
    }
}