namespace Digital_Products.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }

        public Order Order { get; set; }
    }

}
