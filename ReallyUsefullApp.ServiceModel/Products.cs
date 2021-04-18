using ServiceStack;

namespace ReallyUsefullApp.ServiceModel
{
    [Route("/product", "POST")]
    public class CreateProduct : IReturn<CreateProductResponse>
    {
        public double Price { get; set; }

        public int Quantity { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Vendor { get; set; }
    }

    public class CreateProductResponse
    {
        public int Id { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
