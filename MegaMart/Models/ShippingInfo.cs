using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMart.Models
{
    public class ShippingInfo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        public string City { get; set; }

        [Required(ErrorMessage = "الرمز البريدي مطلوب")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "يرجى إدخال رقم هاتف صحيح")]
        public string PhoneNumber { get; set; }

        public string Notes { get; set; }

        // Navigation property
        public virtual Order Order { get; set; }
    }
} 