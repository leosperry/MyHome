using HaKafkaNet;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Diagnostics.Metrics;
using MyHome;
using NLog.Web;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseNLog();

var services = builder.Services;

var otel = services.AddOpenTelemetry();

otel.ConfigureResource(resource => {
    resource.AddService(serviceName: "home-automations");
});


otel.WithTracing(tracing =>{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddRedisInstrumentation(redis => {redis.SetVerboseDatabaseStatements = true;});
    tracing.AddOtlpExporter(exporterOptions => {
            exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            exporterOptions.Endpoint = new Uri("http://192.168.1.3:4317");
            exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
        });
});


otel.WithMetrics(metrics => {
    metrics.AddAspNetCoreInstrumentation()
        .AddMeter("Microsoft.AspNetCore.Hosting")
        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter((exporterOptions, metricReaderOptions) =>{
            exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            exporterOptions.Endpoint = new Uri("http://192.168.1.3:4317");
            exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 15000;
        })
        ;
});

builder.Logging.AddOpenTelemetry(config => {
    config.IncludeScopes = true;
    config.IncludeFormattedMessage = true;
    config.ParseStateValues = true;
    config.AddOtlpExporter((exporterOptions, metricReaderOptions) =>{
            exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            exporterOptions.Endpoint = new Uri("http://192.168.1.3:4317");
            exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
        });
});
HaKafkaNetConfig config = new HaKafkaNetConfig();
builder.Configuration.GetSection("HaKafkaNet").Bind(config);

var redisUri = builder.Configuration.GetConnectionString("RedisConStr");
// provide an IDistributedCache implementation
services.AddStackExchangeRedisCache(options => 
{
    options.Configuration = redisUri;
    options.InstanceName = "MyHome.Prod.";
});;

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
