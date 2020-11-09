using System;
using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public class ProductDAO: BaseEntity
    {
        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; }

        public int ExternalProductId { get; set; }

        public int CategoryId { get; set; }

        public CategoryDAO Category { get; set; }

        public int BrandId { get; set; }

        public double SpecialPrice { get; set; }

        public double ListPrice { get; set; }

        public double PreviousListPrice { get; set; }

        public double PreviousSpecialPrice { get; set; }

        public bool Saleable { get; set; }

        public string Code { get; set; }

    }
}