using System;
using System.ComponentModel.DataAnnotations;

namespace Database
{
    public class ProductDAO: BaseEntity
    {
        [Key]
        public int IdProducto { get; set; }
        public string Nombre { get; set; }

        public int IdCategoria { get; set; }

        public int IdSubcategoria { get; set; }

        public int IdMarca { get; set; }

        public double PrecioEspecial { get; set; }

        public double PrecioLista { get; set; }

        public double PrecioListaAnterior { get; set; }

        public double PrecioEspecialAnterior { get; set; }

        public bool Vendible { get; set; }

        public string Codigo { get; set; }

    }
}