using ReallyUsefullApp.ServiceModel.Types;
using System.Threading.Tasks;

namespace ReallyUsefullApp.DataAccess.Core
{
    public interface IProductsRepository
    {
        Task<int> AddProductAsync(Product product);
    }
}
