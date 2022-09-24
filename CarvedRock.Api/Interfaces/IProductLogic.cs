using CarvedRock.Api.ApiModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarvedRock.Api.Interfaces
{
    public interface IProductLogic
    {
        Task<IEnumerable<Product>> GetProductsForCategory(string category);
    }
}
