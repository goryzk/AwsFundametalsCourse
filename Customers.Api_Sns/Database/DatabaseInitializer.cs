using Dapper;

namespace Customers.Api.Database;

public class DatabaseInitializer(IDbConnectionFactory connectionFactory)
{
    public async Task InitializeAsync()
    {
        using var connection = await connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS Customers (
        Id UUID PRIMARY KEY, 
        GitHubUsername TEXT NOT NULL,
        FullName TEXT NOT NULL,
        Email TEXT NOT NULL,
        DateOfBirth TEXT NOT NULL)");
    }
}
