using Cursos.Application.Services;
using Cursos.Domain.Cursos;
using Cursos.Infrastructure.Repositories;
using Cursos.Infrastructure.Serializers;
using Estudiantes.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Cursos.Infrastructure;

public static class DepedencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    ){
        var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbsettings>() 
            ?? throw new ArgumentNullException("MongoDbSettings is not configured");  

        services.AddSingleton<IMongoClient,MongoClient>(sp =>
        {
            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton( sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        services.AddScoped(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<Curso>("Cursos");	
        });

        services.AddScoped<ICursoRepository, CursoRepository>();

        BsonSerializer.RegisterSerializer(new CapacidadCursoSerializer());
        BsonSerializer.RegisterSerializer(new NombreCursoSerializer());
        BsonSerializer.RegisterSerializer(new DescripcionCursoSerializer());
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddScoped<IEventBus, RabbitMQEventBus>();
        services.AddHostedService<RabbitMQEventListener>();

        return services;
    }
}