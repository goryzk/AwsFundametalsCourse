﻿using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Customers.Api.Contracts.Data;

namespace Customers.Api.Repositories;

public class CustomerRepository(IAmazonDynamoDB amazonDynamoDb) : ICustomerRepository
{
    private readonly string _tableItem = "customers";
    public async Task<bool> CreateAsync(CustomerDto customer)
    {
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsDocument = Document.FromJson(customerAsJson).ToAttributeMap();
        
        var createItemRequest = new PutItemRequest
        {
            TableName = _tableItem,
            Item = customerAsDocument
        };

        var response = await amazonDynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<CustomerDto?> GetAsync(Guid id)
    {
        var getItemRequest = new GetItemRequest
        {
            TableName = _tableItem,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } },
            }
        };

        var response = await amazonDynamoDb.GetItemAsync(getItemRequest);
        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }
    
    public async Task<CustomerDto> GetByEmailAsync(string email)
    {
        var queryRequest = new QueryRequest
        {
            TableName = _tableItem,
            IndexName = "Email-Id-index",
            KeyConditionExpression = "Email = :v_Email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {
                    ":v_Email", new AttributeValue
                    {
                        S = email
                    }
                }
            }
        };
        
        var response = await amazonDynamoDb.QueryAsync(queryRequest);
        if (response.Items.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Items.FirstOrDefault());
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson())!;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var scanRequest = new ScanRequest
        {
            TableName = _tableItem
        };

        var response = await amazonDynamoDb.ScanAsync(scanRequest);
        return response.Items.Select(x =>
        {
            var json = Document.FromAttributeMap(x).ToJson();
            return JsonSerializer.Deserialize<CustomerDto>(json);
        })!;
    }

    public async Task<bool> UpdateAsync(CustomerDto customer)
    {
        customer.UpdatedAt = DateTime.Now;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsDocument = Document.FromJson(customerAsJson).ToAttributeMap();
        
        var updateItemRequest = new PutItemRequest
        {
            TableName = _tableItem,
            Item = customerAsDocument
        };

        var response = await amazonDynamoDb.PutItemAsync(updateItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleteItemRequest = new DeleteItemRequest
        {
            TableName = _tableItem,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } },
            }
        };

        var response = await amazonDynamoDb.DeleteItemAsync(deleteItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}
