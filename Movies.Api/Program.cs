using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Movies.Api;

var movie = new Movie
{
    ReleaseYear = 2014,
    AgeRestriction = 18,
    RottenTomatoesPercentage = 100,
    Title = "Vrijaru",
    Id = Guid.NewGuid(),
};

var asJson = JsonSerializer.Serialize(movie);
var attributeMap = Document.FromJson(asJson).ToAttributeMap();

var movieVip = new Movie
{
    ReleaseYear = 2014,
    AgeRestriction = 18,
    RottenTomatoesPercentage = 100,
    Title = "Vrijaru",
    Id = Guid.NewGuid(),
};

var asVipJson = JsonSerializer.Serialize(movieVip);
var attributeMapVip = Document.FromJson(asVipJson).ToAttributeMap();

var transactionRequest = new TransactWriteItemsRequest
{
    TransactItems = new()
    {
        new TransactWriteItem
        {
            Put = new()
            {
                TableName = "movies",
                Item = attributeMap
            }
        },
        new TransactWriteItem
        {
            Put = new()
            {
                TableName = "movies_vip",
                Item = attributeMapVip
            }
        }
    }
};

var db = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
//await db.TransactWriteItemsAsync(transactionRequest);