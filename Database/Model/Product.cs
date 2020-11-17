using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public class Product: BaseEntity
    {
        public string Name { get; set; }

        public int ExternalProductId { get; set; }

        [Required]
        public int CategoryId { get; set; }
        
        public Category Category { get; set; }

        public int BrandId { get; set; }

        public ICollection<ProductPrice> Prices { get; set; }

        public bool Saleable { get; set; }

        public string Code { get; set; }

    }
}