using System.Reflection;
using Amazon;
using Amazon.SQS;
using Customers.Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(QueueSettings.Key));

builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>(_ => new AmazonSQSClient(RegionEndpoint.USEast1));
builder.Services.AddHostedService<QueueConsumerService>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(Assembly.GetAssembly(typeof(Program)));
});

var app = builder.Build();

app.Run();