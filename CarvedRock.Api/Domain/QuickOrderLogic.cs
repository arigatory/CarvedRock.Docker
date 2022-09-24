using System;
using CarvedRock.Api.ApiModels;
using CarvedRock.Api.Integrations;
using CarvedRock.Api.Interfaces;
using Microsoft.Extensions.Logging;

namespace CarvedRock.Api.Domain
{
    public class QuickOrderLogic : IQuickOrderLogic
    {
        private readonly ILogger<QuickOrderLogic> _logger;
        private readonly IOrderProcessingNotification _orderProcessingNotification;

        public QuickOrderLogic(ILogger<QuickOrderLogic> logger, IOrderProcessingNotification orderProcessingNotification)
        {
            _logger = logger;
            _orderProcessingNotification = orderProcessingNotification;
        }
        public Guid PlaceQuickOrder(QuickOrder order, int customerId)
        {
            _logger.LogInformation("Placing order and sending update for inventory...");
            // persist order to database or wherever
            var orderId = Guid.NewGuid();
            // post "orderplaced" event to rabbitmq
            _orderProcessingNotification.QuickOrderReceived(order, customerId, orderId);
            return Guid.NewGuid();
        }
    }
}
