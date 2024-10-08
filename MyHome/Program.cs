using HaKafkaNet;
using KafkaFlow;
using Microsoft.AspNetCore.DataProtection;
using MyHome;
using MyHome.Areas.Office;
using MyHome.Services;
using NLog.Extensions.Logging;
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

var otlpEndpoint = "http://172.17.1.3:4317";

services.AddOpenTelemetry()
    .ConfigureResource(resource => {
        resource.AddService(serviceName: "home-automations");
    }).WithTracing(tracing =>{ 
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddHaKafkaNetInstrumentation()
            //.AddSource(KafkaFlowInstrumentation.ActivitySourceName)
            .AddOtlpExporter(exporterOptions => {
                exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                exporterOptions.Endpoint = new Uri(otlpEndpoint);
                exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
            });
    }).WithMetrics(metrics => {
        metrics.AddAspNetCoreInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddHaKafkaNetInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter((exporterOptions, metricReaderOptions) =>{
                exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                exporterOptions.Endpoint = new Uri("http://localhost:24317");
                exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
                metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 15000;
            })
            .AddOtlpExporter((exporterOptions, metricReaderOptions) =>{
                exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                exporterOptions.Endpoint = new Uri(otlpEndpoint);
                exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
                metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 15000;
            });
    });

builder.Logging.AddOpenTelemetry(logging => {
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;
    logging.ParseStateValues = true;
    logging.AddOtlpExporter((exporterOptions, logProcessOptions) =>{
            exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            exporterOptions.Endpoint = new Uri(otlpEndpoint);
            exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
        });
});

// for local development of dashboard only
services.AddCors(options => {
    options.AddPolicy("hknDev", policy =>{
        policy.WithOrigins("*");
        policy.AllowAnyHeader();
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
services
    .AddSingleton<IGarageService, GarageService>()
    .AddSingleton<LivingRoomService>()
    .AddSingleton<Func<IDynamicLightAdjuster.DynamicLightModel, IDynamicLightAdjuster>>(sp => 
        model => new DynamicLightAdjuster(model, sp.GetRequiredService<ILogger<DynamicLightAdjuster>>()))
    .AddSingleton<INotificationService, NotificationService>()
    .AddSingleton<LightAlertModule>()
    .AddSingleton<HelpersService>()
    .AddSingleton<OfficeService>();

services.AddHaKafkaNet(config, (kafka, cluster) =>{ });


var app = builder.Build();

NLog.LogManager.Configuration = new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"));

// for local development of dashboard only
app.UseCors("hknDev");

await app.StartHaKafkaNet();

app.MapGet("/", () => Results.Redirect("hakafkanet"));

app.Run();
