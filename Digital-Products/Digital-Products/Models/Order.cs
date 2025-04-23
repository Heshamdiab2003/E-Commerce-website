namespace Digital_Products.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }

}
