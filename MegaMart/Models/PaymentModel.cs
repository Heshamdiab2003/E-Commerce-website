using System.ComponentModel.DataAnnotations;

namespace MegaMart.Models
{
    public class PaymentModel
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; }
        [Required]
        public string Expiry { get; set; }
        [Required]
        public string CVC { get; set; }
    }
} 