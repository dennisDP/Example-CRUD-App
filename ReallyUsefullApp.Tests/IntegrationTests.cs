using Funq;
using ServiceStack;
using NUnit.Framework;
using ReallyUsefullApp.ServiceInterface;
using ReallyUsefullApp.ServiceModel;
using ReallyUsefullApp.DataAccess.Core;
using ServiceStack.Validation;
using ReallyUsefullApp.DataAccess.MongoDB;
using ReallyUsefullApp.ServiceModel.Types;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ReallyUsefullApp.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        const string BaseUri = "http://localhost:2000/";
        private readonly ServiceStackHost appHost;

        class AppHost : AppSelfHostBase
        {
            public AppHost() : base(nameof(IntegrationTests), typeof(ProductsServices).Assembly) { }

            public override void Configure(Container container)
            {
                Plugins.Add(new ValidationFeature());
                container.RegisterValidators(typeof(ProductsServices).Assembly);
                container.RegisterAutoWiredAs<ProductsRepositoryMongoDB, IProductsRepository>();
            }
        }

        public IntegrationTests()
        {
            appHost = new AppHost()
                .Init()
                .Start(BaseUri);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();

        public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);
        
        [Test]
        public void CreateProduct_ThrowsWebServiceException_WhenCatalogNumberIsNotProvided()
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
        public void UpdateProduct_ThrowsWebServiceException_WhenCatalogNumberIsNotProvided()
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
        public void DeleteProduct_ThrowsWebServiceException_WhenCatalogNumberIsNotProvided()
        {
            var deleteProduct = new DeleteProduct();

            var client = CreateClient();

            Assert.ThrowsAsync<WebServiceException>(() => client.DeleteAsync(deleteProduct));
        }
        
        [Test]
        public void CanDoAllAvailableActionsInMeaningfulSequence_WhenRequestsAreValid()
        {
            int[] allProductsIds = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            CreateProduct_DoesNotThrow_WhenCatalogNumberIsProvided(allProductsIds);

            Func<Product, bool> productValuesMatchId = product =>
            {
                return
                (
                product.CatalogNumber != 0 &&
                product.Price == product.CatalogNumber &&
                product.Quantity == product.CatalogNumber &&
                product.Name == $"Name{product.CatalogNumber}" &&
                product.Category == $"Category{product.CatalogNumber}" &&
                product.Vendor == $"Vendor{product.CatalogNumber}");
            };
            GetProducts_ReturnsResponseContainingSpecifiedObjects_WhenIdsAreProvided(allProductsIds.Take(5).ToArray(), 5, productValuesMatchId);
            GetProducts_ReturnsResponseContainingAllObjects_WhenIdsAreNotProvided(allProductsIds.Length, productValuesMatchId);


            UpdateProducts_DoesNotThrow_WhenCatalogNumberIsProvided(allProductsIds.First());

            Func<Product, bool> productValuesMatchIdUpdated = product =>
            {
                return
                (
                product.CatalogNumber != 0 &&
                product.Price == product.CatalogNumber + 10 &&
                product.Quantity == product.CatalogNumber + 10 &&
                product.Name == $"Name{product.CatalogNumber}{product.CatalogNumber}" &&
                product.Category == $"Category{product.CatalogNumber}{product.CatalogNumber}" &&
                product.Vendor == $"Vendor{product.CatalogNumber}{product.CatalogNumber}");
            };
            GetProducts_ReturnsResponseContainingSpecifiedObjects_WhenIdsAreProvided(allProductsIds.Take(1).ToArray(), 1, productValuesMatchIdUpdated);

            DeleteProduct_DoesNotThrow_WhenCatalogNumberIsProvided(allProductsIds.First());
            GetProducts_ReturnsResponseContainingAllObjects_WhenIdsAreNotProvided(allProductsIds.Length - 1, productValuesMatchId);

            var productsIdsLeft = allProductsIds.TakeLast(9);
            var expectedProductsStatistics = new ProductsStatistics
            {
                NumberOfAllProducts = productsIdsLeft.Aggregate((x, y) => x + y),
                NumberOfDifferentProducts = 9,
                AverageProductPrice = productsIdsLeft.Average()
            };
            GetProductsStatistics_ReturnsResponseContainingValidStatistics(expectedProductsStatistics);
        }

        public void CreateProduct_DoesNotThrow_WhenCatalogNumberIsProvided(int[] allProductsIds)
        {
            //Arrange
            var client = CreateClient();
           
            var createProducts = allProductsIds.Select(id => {
                return new CreateProduct
                {
                    CatalogNumber = id,
                    Price = id,
                    Quantity = id,
                    Name = $"Name{id}",
                    Category = $"Category{id}",
                    Vendor = $"Vendor{id}"
                };
            });

            //Act
            var createProductsTasks = Task.WhenAll(createProducts.Select(cp => client.PostAsync(cp)));

            //Assert
            Assert.DoesNotThrowAsync(async () => await createProductsTasks);
        }

        public void GetProducts_ReturnsResponseContainingSpecifiedObjects_WhenIdsAreProvided(int[] productsIds, int expectedNumberOfProducts, Func<Product, bool> match)
        {
            //Arrange
            var client = CreateClient();
            var getProducts = new GetProducts
            {
                Ids = productsIds
            };

            //Act
            var getProductsResponse = client.Get(getProducts);

            //Assert
            Assert.AreEqual(expectedNumberOfProducts, getProductsResponse.Products.Count); ;
            Assert.That(getProductsResponse.Products.All(p => match(p)));
        }

        public void GetProducts_ReturnsResponseContainingAllObjects_WhenIdsAreNotProvided(int expectedNumberOfProducts, Func<Product, bool> match)
        {
            //Arrange
            var client = CreateClient();
            var getProducts = new GetProducts();

            //Act
            var getProductsResponse = client.Get(getProducts);

            //Assert
            Assert.AreEqual(expectedNumberOfProducts, getProductsResponse.Products.Count);
            Assert.That(getProductsResponse.Products.All( p => match(p)));
        }

        public void UpdateProducts_DoesNotThrow_WhenCatalogNumberIsProvided(int productId)
        {
            //Arrange
            var client = CreateClient();
            var updateProduct = new UpdateProduct
            {
                CatalogNumber = productId,
                Price = productId + 10,
                Quantity = productId + 10,
                Name = $"Name{productId}{productId}",
                Category = $"Category{productId}{productId}",
                Vendor = $"Vendor{productId}{productId}"
            };

            //Act
            var updateProductTask = client.PutAsync(updateProduct);

            //Assert
            Assert.DoesNotThrowAsync(async () => await updateProductTask);
        }

        public void DeleteProduct_DoesNotThrow_WhenCatalogNumberIsProvided(int productId)
        {
            //Arrange
            var client = CreateClient();
            var deleteProduct = new DeleteProduct
            {
                CatalogNumber = productId
            };

            //Act
            var deleteProductTask = client.DeleteAsync(deleteProduct);

            //Assert
            Assert.DoesNotThrowAsync(async () => await deleteProductTask);
        }

        public void GetProductsStatistics_ReturnsResponseContainingValidStatistics(ProductsStatistics expectedProductsStatistics)
        {
            //Arrange
            var client = CreateClient();
            var getProductsStatistic = new GetProductsStatistics();

            //Act
            var getProductsStatisticsResponse = client.Get(getProductsStatistic);

            //Assert
            var productsStatistics = getProductsStatisticsResponse.ProductsStatistics;
            Assert.AreEqual(expectedProductsStatistics.NumberOfAllProducts, productsStatistics.NumberOfAllProducts);
        }
    }
}