using CarvedRock.Api.ApiModels;
using Microsoft.Extensions.Configuration;
using System;
using RabbitMQ.Client;
using System.Text.Json;
using Serilog;

namespace CarvedRock.Api.Integrations
{
    public class OrderProcessingNotification : IOrderProcessingNotification
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string queueName = "quickorder.received";

        public OrderProcessingNotification(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration.GetValue<string>("RabbitMqHost")
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queueName, false, false, false, null);
        }

        public void QuickOrderReceived(QuickOrder order, int customerId, Guid orderId)
        {
            var message = new QuickOrderReceivedMessage { Order = order, CustomerId = customerId, OrderId = orderId};
            var messsgeBytes = JsonSerializer.SerializeToUtf8Bytes(message);
            _channel.BasicPublish("", queueName, null, messsgeBytes);
            Log.ForContext("Body", message, true).Information("Published quickorder notification");
        }
    }
}
