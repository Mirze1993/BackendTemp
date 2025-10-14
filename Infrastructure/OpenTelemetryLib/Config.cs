using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Instrumentation.StackExchangeRedis;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Oracle.ManagedDataAccess.OpenTelemetry;

namespace OpenTelemetryLib;

public static class Config
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="configureRedis">
    ///(sp, i) =>
    ///{
    ///    var cache = (RedisCache)sp.GetRequiredService<IDistributedCache>();
    ///    i.AddConnection(cache.GetConnection());
    ///})
    /// </param>
    public static void AddOpenTelemetryServices(this IServiceCollection services, IConfiguration configuration,
        Action<IServiceProvider, StackExchangeRedisInstrumentation> configureRedis = null)
    {
        
        
        #region openTelemetry

        var trac = configuration.GetValue("openTelemetry:IsTracing", defaultValue: false);
        var tracFilter = configuration.GetValue("openTelemetry:TraceFilter", defaultValue: "");
        var isTraceOracle = configuration.GetValue("openTelemetry:TraceOracle", defaultValue: false);
        var isTraceRedis = configuration.GetValue("openTelemetry:TraceRedis", defaultValue: false);


        var metr = configuration.GetValue("openTelemetry:isMetrics", defaultValue: false);
        var additionalMetrs = configuration.GetValue("openTelemetry:AdditionalMetrs", "");
        // Note: Switch between Zipkin/OTLP/Console by setting UseTracingExporter in appsettings.json.
        var tracingExporter = configuration.GetValue("openTelemetry:TracingExporter", defaultValue: "console")!
            .ToLowerInvariant();
        // Note: Switch between Prometheus/OTLP/Console by setting UseMetricsExporter in appsettings.json.
        var metricsExporter = configuration.GetValue("openTelemetry:MetricsExporter", defaultValue: "console")!
            .ToLowerInvariant();

        var profSpanId = configuration.GetValue("openTelemetry:Profiler:AddSpanId", defaultValue: false);
        var profCpuTracking = configuration.GetValue("openTelemetry:Profiler:SetCPUTracking", defaultValue: false);
        var profAllocationTracking =
            configuration.GetValue("openTelemetry:Profiler:SetAllocationTracking", defaultValue: false);
        var profContentionTracking =
            configuration.GetValue("openTelemetry:Profiler:SetAllocationTracking", defaultValue: false);
        var profExceptionTracking =
            configuration.GetValue("openTelemetry:Profiler:SetAllocationTracking", defaultValue: false);
        var profIsActive =
            configuration.GetValue("openTelemetry:Profiler:IsActive", defaultValue: false);
      
        var mkey = Environment.GetEnvironmentVariable("openTelemetry__OtlpMetricsEndpointKey")
                   ?? configuration.GetValue<string>("openTelemetry:OtlpMetricsEndpointKey", "");
        if (trac || metr)
        {
            var t = services.AddOpenTelemetry()
                .ConfigureResource(mm =>
                {
                    mm.AddService(
                        serviceName: Environment.GetEnvironmentVariable("SERVICE_NAME")??configuration.GetValue("openTelemetry:ServiceName", defaultValue: "Test")!,
                        serviceVersion: typeof(Config).Assembly.GetName().Version?.ToString() ?? "unknown",
                        serviceInstanceId: Environment.MachineName);
                });
            
            if (trac)
            {
                if (profIsActive)
                {
             
                    Pyroscope.Profiler.Instance.SetCPUTrackingEnabled(profCpuTracking);
                    Pyroscope.Profiler.Instance.SetAllocationTrackingEnabled(profAllocationTracking);
                    Pyroscope.Profiler.Instance.SetContentionTrackingEnabled(profContentionTracking);
                    Pyroscope.Profiler.Instance.SetExceptionTrackingEnabled(profExceptionTracking);
                    // Pyroscope.Profiler.Instance.SetBasicAuth(profUser,profToken);
            
                }
                t.WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .AddAspNetCoreInstrumentation(o =>
                        {
                            if (!string.IsNullOrEmpty(tracFilter))
                            {
                                var filter = tracFilter?.Split(";") ?? ["index.html", "swagger.json", "metrics"];
                                o.Filter = context => !filter.Any(mm =>
                                    context!.Request.Path.Value!.Contains(mm));
                            }

                            o.RecordException = true;
                        })
                        .AddHttpClientInstrumentation(o => { o.RecordException = true; });
                    if (isTraceOracle)
                        tracerProviderBuilder.AddOracleDataProviderInstrumentation(o =>
                        {
                            o.EnableConnectionLevelAttributes = true;
                            o.RecordException = true;
                            o.InstrumentOracleDataReaderRead = true;
                            o.SetDbStatementForText = true;
                        });
                    if (isTraceRedis)
                    {
                        tracerProviderBuilder.AddRedisInstrumentation(mm => { mm.SetVerboseDatabaseStatements = true; })
                            .ConfigureRedisInstrumentation(configureRedis);
                    }

                    
                    switch (configuration.GetValue("openTelemetry:TracingExporter", defaultValue: ""))
                    {
                        case "zipkin":
                            break;
                        case "otlp":
                            var otlpEndpoint =
                                configuration.GetValue("openTelemetry:OtlpTracingEndpoint", defaultValue: "");
                            var tuser = configuration.GetValue("openTelemetry:OtlpTracingEndpointUser",
                                defaultValue: "");
                            var tkey = Environment.GetEnvironmentVariable("openTelemetry__OtlpTracingEndpointKey")
                                       ?? configuration.GetValue<string>("openTelemetry:OtlpTracingEndpointKey", "");
                            var authHeader =
                                $"Authorization=Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{tuser}:{tkey}"))}";
                            if (!string.IsNullOrEmpty(otlpEndpoint))
                                tracerProviderBuilder.AddOtlpExporter(otlpOptions =>
                                {
                                    otlpOptions.Endpoint = new Uri(otlpEndpoint!);
                                    otlpOptions.Headers = authHeader;
                                    otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                                    otlpOptions.TimeoutMilliseconds = 60000;
                                    otlpOptions.BatchExportProcessorOptions = new BatchExportActivityProcessorOptions()
                                    {
                                        ScheduledDelayMilliseconds = 1000,  // default 5000
                                        ExporterTimeoutMilliseconds = 30000,  // default 30s
                                        MaxQueueSize = 2048,
                                        MaxExportBatchSize = 512
                                    };
                                });
                            if (profSpanId)
                                tracerProviderBuilder.AddProcessor(
                                    new Pyroscope.OpenTelemetry.PyroscopeSpanProcessor());
                            break;

                        default:
                            tracerProviderBuilder.AddConsoleExporter();
                            break;
                    }
                });
            }

            if (metr)
            {
                t.WithMetrics(providerBuilder =>
                {
                    providerBuilder.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddProcessInstrumentation()
                        .AddRuntimeInstrumentation();

                    if (!string.IsNullOrEmpty(additionalMetrs) && additionalMetrs.Split(";").Any())
                    {
                        providerBuilder.AddMeter(additionalMetrs.Split(";"));
                    }

                    switch (metricsExporter)
                    {
                        case "prometheus":
                            providerBuilder.AddPrometheusExporter();
                            break;
                        case "otlp":
                            var otlpEndpoint =
                                configuration.GetValue("openTelemetry:OtlpMetricsEndpoint", defaultValue: "");
                            var muser = configuration.GetValue("openTelemetry:OtlpMetricsEndpointUser",
                                defaultValue: "");
                            var mkey = Environment.GetEnvironmentVariable("openTelemetry__OtlpMetricsEndpointKey")
                                       ?? configuration.GetValue<string>("openTelemetry:OtlpMetricsEndpointKey", "");
                            var authHeader =
                                $"Authorization=Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{muser}:{mkey}"))}";
                            if (!string.IsNullOrEmpty(otlpEndpoint))
                                providerBuilder.AddOtlpExporter(otlpOptions =>
                                {
                                    otlpOptions.Endpoint = new Uri(otlpEndpoint!);
                                    otlpOptions.Headers = authHeader;
                                    otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                                });
                            break;
                        default:
                            providerBuilder.AddConsoleExporter();
                            break;
                    }
                });
            }
        }

       
        
        #endregion
    }


    public static void AddOpenTelemetryLogging(this ILoggingBuilder logging, IConfiguration configuration)
    {
        var otlpEndpoint =
            configuration.GetValue("openTelemetry:OtlpLogEndpoint", defaultValue: "");
        var luser = configuration.GetValue("openTelemetry:OtlpLogEndpointUser",
            defaultValue: "");
        var lkey = configuration.GetValue("openTelemetry:OtlpLogEndpointKey", defaultValue: "");
        var authHeader =
            $"Authorization=Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{luser}:{lkey}"))}";
        logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            
            
            options.AddOtlpExporter(o =>
            {
                o.Endpoint = new Uri(otlpEndpoint);
                o.Headers = authHeader;
                o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            });
        });
    }
    
    
    public static void UseOpenTelemetry(this IApplicationBuilder app, IConfiguration configuration)
    {
        var metr = configuration.GetValue("openTelemetry:isMetrics", defaultValue: false);
        if (metr)
        {
            app.UseOpenTelemetryPrometheusScrapingEndpoint();
        }
    }
}