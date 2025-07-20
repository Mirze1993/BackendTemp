using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace PersistenceMongo;

public static class Config
{
    public static void AddMongoClient(this IServiceCollection serviceCollection,IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IMongoClient>(sp =>
        {
            var connectionUri=configuration.GetValue("MongoDb","");
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            return new MongoClient(settings);
        });
        serviceCollection.AddSingleton<ChatHistoryMdb>();
    }
}