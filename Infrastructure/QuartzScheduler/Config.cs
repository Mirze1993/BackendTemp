using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using QuartzScheduler.CustomJobs;

namespace QuartzScheduler;

public static class Config
{
    public static void QuartzConfig(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // base configuration from appsettings.json
       // services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));
        
        // if you are using persistent job store, you might want to alter some options
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });
        
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.AddQuartz(q =>
        {
            q.SchedulerId = "Scheduler-Core";
            q.UsePersistentStore(store =>
            {
                
                store.UseProperties = true;
                store.UseClustering(c =>
                {
                    c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                    c.CheckinInterval = TimeSpan.FromSeconds(10);
                });
                
                store.PerformSchemaValidation = true;
                store.UseProperties = true;
                store.UseOracle(oracleConfig =>
                {
                    var user = configuration["OracleConfig:Username"] ?? "";
                    var pass = configuration["OracleConfig:Password"] ?? "";
                    var host = configuration["OracleConfig:HostName"] ?? "";
                    var serviceName = configuration["OracleConfig:ServiceName"] ?? "";
                    var port = configuration["OracleConfig:Port"] ?? "1521";
                    var maxPoolSize = configuration["OracleConfig:MaxPool"] ?? "10";
                    var timeout = configuration["OracleConfig:Timeout"] ?? "15";

                    oracleConfig.ConnectionString =
                        $@"Data Source=
                                        (DESCRIPTION=
                                            (ADDRESS_LIST=
                                                (ADDRESS=
                                                    (PROTOCOL=TCP)
                                                    (HOST={host})
                                                    (PORT={port})
                                                )
                                            )
                                            (CONNECT_DATA=
                                                (SERVER=DEDICATED)
                                                (SERVICE_NAME={serviceName})
                                            )
                                        );
                                        User Id={user};
                                        Password={pass};
                                        Min Pool Size=1;
                                        Max Pool Size={maxPoolSize};
                                        Pooling=True;
                                        Validate Connection=True;
                                        Connection Lifetime=3600;
                                        Self Tuning=False;
                                        Connection Timeout={timeout};
                                        ";
                    oracleConfig.TablePrefix = "QRTZ_";
                });
                store.UseNewtonsoftJsonSerializer();
            });

            var jobKey=FileToArchiveJob.Key;
            q.AddJob<FileToArchiveJob>(options =>
            {
                options.WithIdentity(jobKey);
                options.StoreDurably();
                options.UsingJobData(new JobDataMap()
                {
                    { FileToArchiveJob.DirKey, configuration.GetValue<string>($"Quartz:{ FileToArchiveJob.DirKey}") },
                    { FileToArchiveJob.ArchiveDirKey, configuration.GetValue<string>($"Quartz:{ FileToArchiveJob.ArchiveDirKey}") },
                    { FileToArchiveJob.ExtensionKey, configuration.GetValue<string>($"Quartz:{ FileToArchiveJob.ExtensionKey}") },
                    { FileToArchiveJob.ToKeepMinuteKey, configuration.GetValue<string>($"Quartz:{ FileToArchiveJob.ToKeepMinuteKey}") },
                });
            });
            
            q.AddTrigger(options =>options
                .WithIdentity("FileToArchiveTrg")
                .ForJob(jobKey)
                .StartNow()
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever())
                .WithDescription("FileToArchiveTrigger for gz")
            );
        });
    }
}