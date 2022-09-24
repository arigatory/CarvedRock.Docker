using CarvedRock.Api.ApiModels;
using System;

namespace CarvedRock.Api.Integrations
{
    public interface IOrderProcessingNotification
    {
        void QuickOrderReceived(QuickOrder order, int customerId, Guid orderId);
    }
}
