using HaKafkaNet;
using MyHome;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseNLog(); // enable tracing

var services = builder.Services;


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
    options.InstanceName = "MyHome.Dev.";
});

//my services
services.AddHaKafkaNet(config);

services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("DefaultLogger"));

var app = builder.Build();

NLog.LogManager.Configuration = new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"));

await app.StartHaKafkaNet();

app.MapGet("/", () => Results.Redirect("hakafkanet"));

app.UseCors("hknDev");

app.Run();
