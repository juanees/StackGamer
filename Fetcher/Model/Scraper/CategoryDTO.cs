using System;
using System.Collections.Generic;
using System.Text;

namespace Fetcher.Model.Scraper
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
    }
}
