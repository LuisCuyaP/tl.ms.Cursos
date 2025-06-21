using Cursos.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text.Json;

namespace Estudiantes.Infrastructure.Services;

public class RabbitMQEventBus : IEventBus, IDisposable
{
    private IConnection _connection;
    private IModel _channel;
    private readonly ILogger<RabbitMQEventBus> _logger;

    public RabbitMQEventBus(
        IConfiguration configuration,
        ILogger<RabbitMQEventBus> logger)
    {
        _logger = logger;
        _logger.LogInformation("Establishing RabbitMQ connection...");
        var factory = new ConnectionFactory
        {
            Uri = new Uri(configuration["UrlRabbit"]!)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "CursosEstudiantesExchange", type: ExchangeType.Fanout, durable: true);
        _logger.LogInformation("RabbitMQ connection established.");
    }

    public void Publish<T>(T @event) where T : class
    {
        _logger.LogInformation("Publishing event: {EventName}", typeof(T).Name);
        var eventName = typeof(T).Name;
        var message = JsonSerializer.Serialize(@event);
        var body = System.Text.Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: "CursosEstudiantesExchange",
            routingKey: string.Empty,
            basicProperties: CreateBasicProperties(eventName),
            body: body);
        _logger.LogInformation("Event published: {EventName}", eventName);
    }

    private IBasicProperties CreateBasicProperties(string Type)
    {
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true; // Ensure messages are durable
        properties.Type = Type; // Set the type of the message
        return properties;
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}