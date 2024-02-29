using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

var secretsManagerClient = new AmazonSecretsManagerClient(RegionEndpoint.USEast1);

var secretsRequest = new GetSecretValueRequest
{
    SecretId = "ApiKey",
    VersionStage = "AWSPREVIOUS", //AWSCURRENT
};

var response = await secretsManagerClient.GetSecretValueAsync(secretsRequest);

Console.WriteLine(response.SecretString);
Console.ReadKey();