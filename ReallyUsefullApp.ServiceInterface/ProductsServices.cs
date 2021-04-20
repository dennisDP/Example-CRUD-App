using ServiceStack;
using ReallyUsefullApp.ServiceModel;
using System.Threading.Tasks;
using ReallyUsefullApp.DataAccess.Core;
using ReallyUsefullApp.ServiceModel.Types;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.FluentValidation;

namespace ReallyUsefullApp.ServiceInterface
{
    public class CreateProductValidator : AbstractValidator<CreateProduct>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.CatalogNumber).NotEmpty();
        }
    }

    public class UpdateProductValidator : AbstractValidator<UpdateProduct>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.CatalogNumber).NotEmpty();
        }
    }

    public class DeleteProductValidator : AbstractValidator<DeleteProduct>
    {
        public DeleteProductValidator()
        {
            RuleFor(x => x.CatalogNumber).NotEmpty();
        }
    }

    public class ProductsServices : Service
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsServices(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<object> GetAsync(GetProductsStatistics request)
        {
            var statistics = await _productsRepository.GetStatistics();
            return new GetProductsStatisticsResponse
            {
                ProductsStatistics = statistics
            };
        }

        public async Task<object> GetAsync(GetProducts request)
        {
            IEnumerable<Product> products;
            if(request.Ids == null || request.Ids.Length == 0)
            {
                products = await _productsRepository.GetAllAsync();
            }
            else
            {
                products = await _productsRepository.GetByIdsAsync(request.Ids);
            }

            return new GetProductsResponse { Products = products.ToList() };
        }
       
        public async Task<object> PostAsync(CreateProduct request)
        {
            var existingProduct = await _productsRepository.GetByIdsAsync(new int[] { request.CatalogNumber });
            if(existingProduct.FirstOrDefault() != null)
            {
                throw HttpError.Conflict($"Product with Catalog Number: {request.CatalogNumber} already exists.");
            }

            var product = request.ConvertTo<Product>();
            await _productsRepository.AddAsync(product);

            return new CreateProductResponse();
        }

        public async Task<object> PutAsync(UpdateProduct request)
        {
            var product = (await _productsRepository.GetByIdsAsync(new int[] { request.CatalogNumber })).FirstOrDefault();
            if (product == null)
            {
                throw HttpError.NotFound($"Product with Catalog Number:{request.CatalogNumber} does not exist.");
            }

            var updatedProduct = request.ConvertTo<Product>();
            await _productsRepository.UpdateAsync(updatedProduct);

            return new UpdateProductResponse();
        }

        public async Task<object> DeleteAsync(DeleteProduct request)
        {
            var product = (await _productsRepository.GetByIdsAsync(new int[] { request.CatalogNumber })).FirstOrDefault();
            if (product == null)
            {
                throw HttpError.NotFound($"Product with Catalog Number:{request.CatalogNumber} does not exist.");
            }

            await _productsRepository.DeleteAsync(product.CatalogNumber);

            return new DeleteProductResponse();
        }
    }
}
