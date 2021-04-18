using Funq;
using ServiceStack;
using NUnit.Framework;
using ReallyUsefullApp.ServiceInterface;
using ReallyUsefullApp.ServiceModel;
using ReallyUsefullApp.DataAccess.Core;
using Moq;
using ServiceStack.Validation;

namespace ReallyUsefullApp.Tests
{
    public class ProductsServicesIntegrationTests
    {
        const string BaseUri = "http://localhost:2000/";
        private readonly ServiceStackHost appHost;

        class AppHost : AppSelfHostBase
        {
            public AppHost() : base(nameof(ProductsServicesIntegrationTests), typeof(ProductsServices).Assembly) { }

            public override void Configure(Container container)
            {
                Plugins.Add(new ValidationFeature());
                container.RegisterValidators(typeof(ProductsServices).Assembly);
                container.Register(new Mock<IProductsRepository>().Object);
            }
        }

        public ProductsServicesIntegrationTests()
        {
            appHost = new AppHost()
                .Init()
                .Start(BaseUri);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();

        public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);

        [Test]
        public void PostAsync_ThrowsWebServiceException_WhenCatalogNumberIsNotProvided()
        {
            var createProduct = new CreateProduct
            {
                Price = 10,
                Quantity = 100,
                Name = "Winter Jacket",
                Category = "Jackets",
                Vendor = "Nike"
            };

            var client = CreateClient();

            Assert.ThrowsAsync<WebServiceException>(() => client.PostAsync(createProduct));
        }

        [Test]
        public void PutAsync_ThrowsWebServiceException_WhenCatalogNumberIsNotProvided()
        {
            var updateProduct = new UpdateProduct
            {
                Price = 10,
                Quantity = 100,
                Name = "Winter Jacket",
                Category = "Jackets",
                Vendor = "Nike"
            };

            var client = CreateClient();

            Assert.ThrowsAsync<WebServiceException>(() => client.PutAsync(updateProduct));
        }

        [Test]
        public void DeleteAsync_ThrowsWebServiceException_WhenCatalogNumberIsNotProvided()
        {
            var deleteProduct = new DeleteProduct();

            var client = CreateClient();

            Assert.ThrowsAsync<WebServiceException>(() => client.DeleteAsync(deleteProduct));
        }
    }
}