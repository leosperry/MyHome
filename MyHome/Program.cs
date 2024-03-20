using HaKafkaNet;
using MyHome;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseNLog();

var services = builder.Services;

HaKafkaNetConfig config = new HaKafkaNetConfig();
builder.Configuration.GetSection("HaKafkaNet").Bind(config);

// provide an IDistributedCache implementation
services.AddStackExchangeRedisCache(options => 
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConStr");
});

//my services
services.AddSingleton<IGarageService, GarageService>();
services.AddSingleton<Func<IDynamicLightAdjuster.DynamicLightModel, IDynamicLightAdjuster>>(model => new DynamicLightAdjuster(model));

services.AddHaKafkaNet(config);

var app = builder.Build();

await app.StartHaKafkaNet();

app.MapGet("/", () => Results.Redirect("dashboard.html"));

app.Run();
