using ReallyUsefullApp.ServiceModel.Types;
using ServiceStack;
using System.Collections.Generic;

namespace ReallyUsefullApp.ServiceModel
{
    [Route("/products/stats", "GET")]
    public class GetProductsStatistics : IReturn<GetProductsStatisticsResponse>
    {
    }

    public class GetProductsStatisticsResponse
    {
        public ProductsStatistics ProductsStatistics { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    [Route("/products/", "GET")]
    [Route("/products/{Ids}", "GET")]
    public class GetProducts : IReturn<GetProductsResponse>
    {
        public int[] Ids { get; set; }
    }

    public class GetProductsResponse
    {
        public List<Product> Products { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    [Route("/products/", "POST")]
    [Route("/products/{CatalogNumber}", "POST")]
    public class CreateProduct : IReturn<ResponseStatus>
    {
        public int CatalogNumber { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
    }

    public class CreateProductResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    [Route("/products/", "PUT")]
    [Route("/products/{CatalogNumber}", "PUT")]
    public class UpdateProduct : IReturn<UpdateProductResponse>
    {
        public int CatalogNumber { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
    }

    public class UpdateProductResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    [Route("/products/", "DELETE")]
    [Route("/products/{CatalogNumber}", "DELETE")]
    public class DeleteProduct : IReturn<DeleteProductResponse>
    {
        public int CatalogNumber { get; set; }
    }

    public class DeleteProductResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
}
