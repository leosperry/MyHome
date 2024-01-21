using HaKafkaNet;

var builder = WebApplication.CreateBuilder(args);


var services = builder.Services;

HaKafkaNetConfig config = new HaKafkaNetConfig();
builder.Configuration.GetSection("HaKafkaNet").Bind(config);

// provide an IDistributedCache implementation
services.AddStackExchangeRedisCache(options => 
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConStr");
});

//my services

services.AddHaKafkaNet(config);

var app = builder.Build();

await app.StartHaKafkaNet(config);

app.MapGet("/", () => "Hello World!");

app.Run();
