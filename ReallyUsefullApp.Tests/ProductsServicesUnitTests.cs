using NUnit.Framework;
using ServiceStack;
using ReallyUsefullApp.ServiceInterface;
using ReallyUsefullApp.ServiceModel;
using Moq;
using ReallyUsefullApp.DataAccess.Core;
using ReallyUsefullApp.ServiceModel.Types;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReallyUsefullApp.Tests
{
    public class ProductsServicesUnitTests
    {
        [Test]
        public void GetAsync_ReturnsCorrespondingProducts_WhenIdsAreProvided()
        {
            var products = new List<Product>
            {
                new Product
                {
                    CatalogNumber = 1
                },
                new Product
                {
                    CatalogNumber = 2
                }
            };

            var productsRepositoryMock = new Mock<IProductsRepository>();
            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(products as IEnumerable<Product>));
            var service = new ProductsServices(productsRepositoryMock.Object);

            var request = new GetProducts
            {
                Ids = new int[] { 1, 2 }
            };
            var response = (GetProductsResponse)service.GetAsync(request).Result;

            Assert.AreEqual(2, response.Products.Count);
            Assert.AreEqual(1, response.Products[0].CatalogNumber);
            Assert.AreEqual(2, response.Products[1].CatalogNumber);
        }

        [Test]
        public void GetAsync_ReturnsAllProducts_WhenIdsAreNotProvided()
        {
            var products = new List<Product>
            {
                new Product
                {
                    CatalogNumber = 1
                },
                new Product
                {
                    CatalogNumber = 2
                }
            };

            var productsRepositoryMock = new Mock<IProductsRepository>();
            productsRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(products as IEnumerable<Product>));
            var service = new ProductsServices(productsRepositoryMock.Object);

            var request = new GetProducts();
            var response = (GetProductsResponse)service.GetAsync(request).Result;

            Assert.AreEqual(2, response.Products.Count);
            Assert.AreEqual(1, response.Products[0].CatalogNumber);
            Assert.AreEqual(2, response.Products[1].CatalogNumber);
        }

        [Test]
        public void PostAsync_DoesNotThrow_WhenProductDoesNotAlreadyExist()
        {
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);
            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product>() as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var createProduct = new CreateProduct
            {
                CatalogNumber = 1,
                Price = 10,
                Quantity = 100,
                Name = "Winter Jacket",
                Category = "Jackets",
                Vendor = "Nike"
            };

            Assert.DoesNotThrowAsync(() => service.PostAsync(createProduct));
        }

        [Test]
        public void PostAsync_ThrowsHttpError_WhenProductAlreadyExists()
        {
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);
            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product>() { new Product() } as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var createProduct = new CreateProduct
            {
                CatalogNumber = 1,
                Price = 10,
                Quantity = 100,
                Name = "Winter Jacket",
                Category = "Jackets",
                Vendor = "Nike"
            };

            Assert.ThrowsAsync<HttpError>(() => service.PostAsync(createProduct));
        }

        [Test]
        public void PutAsync_DoesNotThrow_WhenProductExists()
        {
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product> { new Product() } as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var updateProduct = new UpdateProduct
            {
                CatalogNumber = 1
            };

            Assert.DoesNotThrowAsync(() => service.PutAsync(updateProduct));
        }

        [Test]
        public void PutAsync_ThrowsHttpError_WhenProductDoesNotExist()
        {
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product> () as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var updateProduct = new UpdateProduct
            {
                CatalogNumber = 1
            };

            Assert.ThrowsAsync<HttpError>(() => service.PutAsync(updateProduct));
        }

        [Test]
        public void DeleteAsync_DoesNotThrow_WhenProductExists()
        {
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product> { new Product() } as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var deleteProduct = new DeleteProduct
            {
                CatalogNumber = 1
            };

            Assert.DoesNotThrowAsync(() => service.DeleteAsync(deleteProduct));
        }

        [Test]
        public void DeleteAsync_ThrowsHttpError_WhenProductDoesNotExist()
        {
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product>() as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var deleteProduct = new DeleteProduct
            {
                CatalogNumber = 1
            };

            Assert.ThrowsAsync<HttpError>(() => service.DeleteAsync(deleteProduct));
        }
    }
}
