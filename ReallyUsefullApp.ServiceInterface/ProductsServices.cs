using System;
using ServiceStack;
using ReallyUsefullApp.ServiceModel;
using System.Threading.Tasks;
using ReallyUsefullApp.DataAccess.Core;
using ReallyUsefullApp.ServiceModel.Types;

namespace ReallyUsefullApp.ServiceInterface
{
    public class ProductsServices : Service
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsServices(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<object> Post(CreateProduct request)
        {
            var product = request.ConvertTo<Product>();
            int productId;

            try
            {
                productId = await _productsRepository.AddProductAsync(product);
            }
            catch
            {
                throw new Exception("Something went wrong.");
            }

            return new CreateProductResponse { Id = productId  };
        }
    }
}
