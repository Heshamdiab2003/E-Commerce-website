using System.ComponentModel.DataAnnotations;

namespace MegaMart.Models
{
    public class ShippingInfoModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
    }
} 