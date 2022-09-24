using CarvedRock.OrderProcessor.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text.Json;

namespace CarvedRock.OrderProcessor;

public class Worker : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private string queueName = "quickorder.received";
    private EventingBasicConsumer _consumer;

    public Worker(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration.GetValue<string>("RabbitMqHost")
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queueName,false, false, false, null);
        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += ProcessQuickOrderReceived;
    }

    private void ProcessQuickOrderReceived(object? sender, BasicDeliverEventArgs e)
    {
        var orderInfo = JsonSerializer.Deserialize<QuickOrderReceivedMessage>(e.Body.ToArray());

        Log.ForContext("OrderReceived", orderInfo, true)
            .Information("Received message from queue for processing.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _channel.BasicConsume(queueName, true, _consumer);
        }
    }
}