using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Model
{
    public class BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
