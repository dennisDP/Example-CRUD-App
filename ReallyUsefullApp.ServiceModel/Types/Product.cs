using ServiceStack.DataAnnotations;

namespace ReallyUsefullApp.ServiceModel.Types
{
    public class Product
    {
        [AutoIncrement]
        public int Id { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
    }
}
