using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using ReallyUsefullApp.ServiceInterface;
using ReallyUsefullApp.ServiceModel;
using Moq;
using ReallyUsefullApp.DataAccess.Core;
using ReallyUsefullApp.ServiceModel.Types;
using System.Threading.Tasks;

namespace ReallyUsefullApp.Tests
{
    public class ProductsServicesUnitTests
    {
        [Test]
        public void CreateProduct_ReturnsValidResponse_WhenRequestIsValid()
        {
            var productsRepositoryMock = new Mock<IProductsRepository>();
            productsRepositoryMock
                .Setup(x => x.AddProductAsync(It.IsAny<Product>()))
                .Returns(Task.FromResult(1));

            var service = new ProductsServices(productsRepositoryMock.Object);
            var product = new CreateProduct
            {
                Price = 10,
                Quantity = 100,
                Name = "Winter Jacket",
                Category = "Jackets",
                Vendor = "Nike"
            };

            var response = (CreateProductResponse)service.Post(product).Result;

            Assert.That(response.Id, Is.EqualTo(1));
        }
    }
}
