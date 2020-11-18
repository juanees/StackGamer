using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Shared.Options
{
    public class StackGamerOption
    {     
        public UrlsOption Urls { get; set; }

        public class UrlsOption
        {
            public string BaseUrl { get; set; }

            public string GetProductByIdUrl { get; set; }

            public string CategoriesUrl { get; set; }
        }
    }
   


}
