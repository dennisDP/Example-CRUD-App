using ServiceStack.DataAnnotations;

namespace ReallyUsefullApp.ServiceModel.Types
{
    public class Product
    {
        public int CatalogNumber { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
    }
}
