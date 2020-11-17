using Database.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Model
{
    public class Parameter : BaseEntity
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }
}
