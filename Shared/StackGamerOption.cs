using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Shared
{
    
    public class StackGamerOption
    {
        public Urls Urls { get; set; }
        
    }

    public class Urls
    {
        public string BaseUrl { get; set; }
        public string GetProductByIdUrl { get; set; }
    }
}
