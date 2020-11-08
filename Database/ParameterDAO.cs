using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database
{
    public class ParameterDAO : BaseEntity
    {
        [Key]
        public int ParameterId { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }        
    }
}
