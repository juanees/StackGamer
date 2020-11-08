using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
