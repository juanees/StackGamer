using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fetcher.Model
{
    public class ProductDTO
    {
        public class Imagene
        {
            [JsonPropertyName("listaImagenes")]
            public List<int?> ListaImagenes { get; set; }

            [JsonPropertyName("nombre")]
            public string Nombre { get; set; }
        }

        public class Nivele
        {
            [JsonPropertyName("nivel_cpu")]
            public int? NivelCpu { get; set; }

            [JsonPropertyName("nivel_gpu")]
            public int? NivelGpu { get; set; }
        }
        [JsonPropertyName("coins")]
        public int? Coins { get; set; }

        [JsonPropertyName("coins_forzada")]
        public int? CoinsForzada { get; set; }

        [JsonPropertyName("coins_sale")]
        public int? CoinsSale { get; set; }

        [JsonPropertyName("destacado")]
        public int? Destacado { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("id_categoria")]
        public int IdCategoria { get; set; }

        [JsonPropertyName("id_subcategoria")]
        public int IdSubcategoria { get; set; }

        [JsonPropertyName("garantia")]
        public string Garantia { get; set; }

        [JsonPropertyName("id_marca")]
        public int IdMarca { get; set; }

        [JsonPropertyName("precioEspecial")]
        public int? PrecioEspecial { get; set; }

        [JsonPropertyName("precioLista")]
        public int? PrecioLista { get; set; }

        [JsonPropertyName("precioListaAnterior")]
        public int? PrecioListaAnterior { get; set; }

        [JsonPropertyName("precioEspecialAnterior")]
        public int? PrecioEspecialAnterior { get; set; }

        [JsonPropertyName("imagenes")]
        public List<Imagene> Imagenes { get; set; }

        [JsonPropertyName("vendible")]
        public int? Vendible { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("niveles")]
        public Nivele Niveles { get; set; }

        [JsonPropertyName("reviews")]
        public object Reviews { get; set; }

        [JsonPropertyName("tv")]
        public object Tv { get; set; }

        [JsonPropertyName("sale")]
        public object Sale { get; set; }

        [JsonPropertyName("incluidos")]
        public object Incluidos { get; set; }
    }
}
