using HaKafkaNet;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

HaKafkaNetConfig config = new HaKafkaNetConfig();
builder.Configuration.GetSection("HaKafkaNet").Bind(config);

services.AddHaKafkaNet(config);

var app = builder.Build();

await app.StartHaKafkaNet();

app.MapGet("/", () => "Transformere is running");

app.Run();
