using Customers.Api.Domain;
using Customers.Api.Mapping;
using Customers.Api.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Customers.Api.Services;

public class CustomerService(
    ICustomerRepository customerRepository,
    IGitHubService gitHubService)
    : ICustomerService
{
    public async Task<bool> CreateAsync(Customer customer)
    {
        var existingUser = await customerRepository.GetAsync(customer.Id);
        if (existingUser is not null)
        {
            var message = $"A user with id {customer.Id} already exists";
            throw new ValidationException(message, GenerateValidationError(nameof(Customer), message));
        }

        var isValidGitHubUser = await gitHubService.IsValidGitHubUser(customer.GitHubUsername);
        if (!isValidGitHubUser)
        {
            var message = $"There is no GitHub user with username {customer.GitHubUsername}";
            throw new ValidationException(message, GenerateValidationError(nameof(customer.GitHubUsername), message));
        }
        
        var customerDto = customer.ToCustomerDto();
        return await customerRepository.CreateAsync(customerDto);
    }

    public async Task<Customer?> GetAsync(Guid id)
    {
        var customerDto = await customerRepository.GetAsync(id);
        return customerDto?.ToCustomer();
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customerDtos = await customerRepository.GetAllAsync();
        return customerDtos.Select(x => x.ToCustomer());
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        var customerDto = customer.ToCustomerDto();
        
        var isValidGitHubUser = await gitHubService.IsValidGitHubUser(customer.GitHubUsername);
        if (!isValidGitHubUser)
        {
            var message = $"There is no GitHub user with username {customer.GitHubUsername}";
            throw new ValidationException(message, GenerateValidationError(nameof(customer.GitHubUsername), message));
        }
        
        return await customerRepository.UpdateAsync(customerDto);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await customerRepository.DeleteAsync(id);
    }

    public async Task<Customer?> GetByEmailAsync(string idOrEmail)
    {
        var customerDto = await customerRepository.GetByEmailAsync(idOrEmail);
        return customerDto.ToCustomer();
    }

    private static ValidationFailure[] GenerateValidationError(string paramName, string message)
    {
        return new []
        {
            new ValidationFailure(paramName, message)
        };
    }
}