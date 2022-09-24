using CarvedRock.Api.ApiModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarvedRock.Api.Repository
{
    public interface ICarvedRockRepository
    {
        Task<List<Product>> GetProducts(string category);
        Task SubmitNewQuickOrder(QuickOrder order, int customerId, Guid orderId);
    }
}