using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    [Table("ProductsPrices")]
    public class ProductPrice : BaseEntity
    {
        public double SpecialPrice { get; set; }

        public double ListPrice { get; set; }

        public double PreviousListPrice { get; set; }

        public double PreviousSpecialPrice { get; set; }
        
        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}