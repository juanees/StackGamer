using System;
using System.Collections.Generic;
using System.Text;

namespace Fetcher.Model.Scraper
{
    public class ScraperCategory
    {
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
        public List<ScraperProduct> Products { get; set; } = new List<ScraperProduct>();
    }
}
