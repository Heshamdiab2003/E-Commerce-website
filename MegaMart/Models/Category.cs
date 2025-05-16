using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace MegaMart.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الفئة مطلوب")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        // Navigation property for products (optional)
        public virtual ICollection<Product> Products { get; set; }
    }
} 