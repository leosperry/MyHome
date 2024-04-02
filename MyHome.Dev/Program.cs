using HaKafkaNet;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseNLog(); // enable tracing

var services = builder.Services;

//builder.Logging.ClearProviders();

services.AddCors(options => {
    options.AddPolicy("hknDev", policy => {
        policy.WithOrigins("*");
        policy.AllowAnyHeader();
    });
});

HaKafkaNetConfig config = new HaKafkaNetConfig();
builder.Configuration.GetSection("HaKafkaNet").Bind(config);

// provide an IDistributedCache implementation
services.AddStackExchangeRedisCache(options => 
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConStr");
});

//my services

services.AddHaKafkaNet(config);


NLog.LogManager.Setup()
    .LoadConfigurationFromAppSettings();

services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("DefaultLogger"));

var app = builder.Build();

await app.StartHaKafkaNet();

app.MapGet("/", () => Results.Redirect("hakafkanet"));

app.UseCors("hknDev");

app.Run();
