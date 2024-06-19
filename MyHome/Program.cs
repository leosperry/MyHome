using System.Diagnostics;
using System.Text;
using System.Text.Unicode;
using HaKafkaNet;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.OpenTelemetry;
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

var activitySource = new ActivitySource("HaKafkaNet");

var traceProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("HaKafkaNet")
    .AddSource(KafkaFlowInstrumentation.ActivitySourceName)
    .Build();

otel.WithTracing(tracing =>{
    tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRedisInstrumentation(redis => {redis.SetVerboseDatabaseStatements = true;})
        .AddSource("HaKafkaNet")
        .AddSource(KafkaFlowInstrumentation.ActivitySourceName)
        .AddOtlpExporter(exporterOptions => {
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

builder.Logging.AddOpenTelemetry(logging => {
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;
    logging.ParseStateValues = true;
    logging.AddOtlpExporter((exporterOptions, logProcessOptions) =>{
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

services.AddHaKafkaNet(config, (kafka, cluset) =>{
    kafka
        .UseMicrosoftLog()
        .AddOpenTelemetryInstrumentation(opt => {
            opt.EnrichConsumer = ((activitySource, messageContext) => {
                activitySource.SetTag("entityId", Encoding.Default.GetString((byte[])messageContext.Message.Key));
            });
        });});

var app = builder.Build();

await app.StartHaKafkaNet();

app.MapGet("/", () => Results.Redirect("hakafkanet"));

app.Run();
