using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database
{
    public class ParameterDAO : BaseEntity
    {
        [Key]
        public int IdParametro { get; set; }

        public string Key { get; set; }

        public string Nombre { get; set; }
        
        public string Descripcion { get; set; }

        public string Valor { get; set; }
    }
}
