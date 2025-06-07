using Appilcation.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistenceOracle.Repository;

namespace PersistenceOracle;

public static class Config
{
    public static async Task OracleDbConfig(
        this IServiceCollection service,
        IConfiguration configuration
        )
    {
        OracleDb.Init(
            configuration["OracleConfig:Username"] ?? "",
            configuration["OracleConfig:Password"] ?? "",
            configuration["OracleConfig:HostName"] ?? "",
            configuration["OracleConfig:ServiceName"] ?? "",
            configuration["OracleConfig:Port"] ?? "",
            configuration["OracleConfig:MaxPool"] ?? "",
            configuration["OracleConfig:Timeout"] ?? ""
        );
        await using OracleDb db = new();
        await db.CheckConnection();
        service.AddTransient<IUserRepository, UserRepository>();
        service.AddTransient<IDbCompileLog, DbCompileLog>();
    }
}