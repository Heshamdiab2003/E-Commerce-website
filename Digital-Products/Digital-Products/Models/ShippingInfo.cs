namespace Digital_Products.Models
{
    public class ShippingInfo
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public Order Order { get; set; }
    }

}
