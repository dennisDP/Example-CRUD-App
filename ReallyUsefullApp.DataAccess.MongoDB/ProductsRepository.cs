using ReallyUsefullApp.DataAccess.Core;
using ReallyUsefullApp.ServiceModel.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReallyUsefullApp.DataAccess.MongoDB
{
    public class ProductsRepository : IProductsRepository
    {
        public Task AddAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetByIdsAsync(int[] ids)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
