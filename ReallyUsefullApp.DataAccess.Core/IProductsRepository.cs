using ReallyUsefullApp.ServiceModel.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReallyUsefullApp.DataAccess.Core
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByIdsAsync(int[] ids);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<ProductsStatistics> GetStatistics();
    }
}
