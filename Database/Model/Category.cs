using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public class Category : BaseEntity
    {
        [Key]
        public int IdCategory { get; set; }

        public string Name { get; set; }

        public int ExternalIdCategory { get; set; }

        public ICollection<Product> Products { get; set; }

    }
}