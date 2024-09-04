using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SharedDependencies.Services;

public class RabbitMQService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string? _exchangeName;
    private readonly string? _queueName;
    private readonly string? _routingKey;

    public RabbitMQService(IConfiguration configuration)
    {
        var rabbitMqConfig = configuration.GetSection("RabbitMq");

        var factory = new ConnectionFactory
        {
            Uri = new Uri(rabbitMqConfig["Uri"]!),
            ClientProvidedName = rabbitMqConfig["ClientProvidedName"]
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _exchangeName = rabbitMqConfig["ExchangeName"];
        _queueName = rabbitMqConfig["QueueName"];
        _routingKey = rabbitMqConfig["RoutingKey"];

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(_queueName, false, false, false, null);
        _channel.QueueBind(_queueName, _exchangeName, _routingKey, null);
        
    }
    public void PublishMessage(string message)
    {
        var messageBodyBytes = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(_exchangeName, _routingKey, null, messageBodyBytes);
    }
    
    public void StartListening()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var body = ea.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(_queueName, false, consumer);
    }
    
    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}