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
    [TestFixture]
    public class ProductsServicesUnitTests
    {
        [Test]
        public void GetProducts_ReturnsCorrespondingProducts_WhenIdsAreProvided()
        {
            //Arrange
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

            //Act
            var response = (GetProductsResponse)service.GetAsync(request).Result;

            //Assert
            Assert.AreEqual(2, response.Products.Count);
            Assert.AreEqual(1, response.Products[0].CatalogNumber);
            Assert.AreEqual(2, response.Products[1].CatalogNumber);
        }

        [Test]
        public void GetProducts_ReturnsAllProducts_WhenIdsAreNotProvided()
        {
            //Arrange
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

            //Act
            var response = (GetProductsResponse)service.GetAsync(request).Result;

            //Assert
            Assert.AreEqual(2, response.Products.Count);
            Assert.AreEqual(1, response.Products[0].CatalogNumber);
            Assert.AreEqual(2, response.Products[1].CatalogNumber);
        }

        [Test]
        public void CreateProduct_DoesNotThrow_WhenProductDoesNotAlreadyExist()
        {
            //Arrange
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

            //Act
            var createProductTask = service.PostAsync(createProduct);

            //Assert
            Assert.DoesNotThrowAsync(() => createProductTask);
        }

        [Test]
        public void CreateProduct_ThrowsHttpError_WhenProductAlreadyExists()
        {
            //Arrange
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

            //Act
            var createProductTask = service.PostAsync(createProduct);

            //Assert
            Assert.ThrowsAsync<HttpError>(() => createProductTask);
        }

        [Test]
        public void UpdateProduct_DoesNotThrow_WhenProductExists()
        {
            //Arrange
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product> { new Product() } as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var updateProduct = new UpdateProduct
            {
                CatalogNumber = 1
            };

            //Act
            var updateProductTask = service.PutAsync(updateProduct);

            //Assert
            Assert.DoesNotThrowAsync(() => updateProductTask);
        }

        [Test]
        public void UpdateProduct_ThrowsHttpError_WhenProductDoesNotExist()
        {
            //Arrange
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product> () as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var updateProduct = new UpdateProduct
            {
                CatalogNumber = 1
            };

            //Act
            var updateProductTask = service.PutAsync(updateProduct);

            //Assert
            Assert.ThrowsAsync<HttpError>(() => updateProductTask);
        }

        [Test]
        public void DeleteProduct_DoesNotThrow_WhenProductExists()
        {
            //Arrange
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product> { new Product() } as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var deleteProduct = new DeleteProduct
            {
                CatalogNumber = 1
            };

            //Act
            var deleteProductTask = service.DeleteAsync(deleteProduct);

            //Assert
            Assert.DoesNotThrowAsync(() => deleteProductTask);
        }

        [Test]
        public void DeleteProduct_ThrowsHttpError_WhenProductDoesNotExist()
        {
            //Arrange
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<int[]>()))
                .Returns(Task.FromResult(new List<Product>() as IEnumerable<Product>));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var deleteProduct = new DeleteProduct
            {
                CatalogNumber = 1
            };

            //Act
            var deleteProductTask = service.DeleteAsync(deleteProduct);

            //Assert
            Assert.ThrowsAsync<HttpError>(() => deleteProductTask);
        }
    }
}
