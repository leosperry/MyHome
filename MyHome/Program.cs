using HaKafkaNet;
using Microsoft.AspNetCore.DataProtection;
using MyHome;
using NLog.Web;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseNLog();

var services = builder.Services;


HaKafkaNetConfig config = new HaKafkaNetConfig();
builder.Configuration.GetSection("HaKafkaNet").Bind(config);

var redisUri = builder.Configuration.GetConnectionString("RedisConStr");
// provide an IDistributedCache implementation
services.AddStackExchangeRedisCache(options => 
{
    options.Configuration = redisUri;
    options.InstanceName = "MyHome.Prod.";
});

var redis = ConnectionMultiplexer.Connect(redisUri!);
services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

//my services
services.AddSingleton<IGarageService, GarageService>();
services.AddSingleton<ILivingRoomService, LivingRoomService>();
services.AddSingleton<Func<IDynamicLightAdjuster.DynamicLightModel, IDynamicLightAdjuster>>(model => new DynamicLightAdjuster(model));
services.AddSingleton<INotificationService, NotificationService>();
services.AddSingleton<LightAlertModule>();

services.AddHaKafkaNet(config);

var app = builder.Build();

await app.StartHaKafkaNet();

app.MapGet("/", () => Results.Redirect("hakafkanet"));

app.Run();
