using HaKafkaNet;
using MyHome;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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

var otlpEndpoint = "http://172.17.1.3:4317";

services.AddOpenTelemetry()
    .ConfigureResource(resource => {
        resource.AddService(serviceName: "home-automations-dev");
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



//my services
services.AddHaKafkaNet(config);

services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("DefaultLogger"));

var app = builder.Build();

NLog.LogManager.Configuration = new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"));

await app.StartHaKafkaNet();

app.MapGet("/", () => Results.Redirect("hakafkanet"));

app.UseCors("hknDev");

app.Run();
