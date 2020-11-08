using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fetcher.Model.Thief
{
    public class ProductDTO
    {
        public class Image
        {
            [JsonPropertyName("listaImagenes")]
            public List<int?> ImagesList { get; set; }

            [JsonPropertyName("nombre")]
            public string Name { get; set; }
        }

        public class Level
        {
            [JsonPropertyName("nivel_cpu")]
            public int? CpuLevel { get; set; }

            [JsonPropertyName("nivel_gpu")]
            public int? GpuLevel { get; set; }
        }
        [JsonPropertyName("coins")]
        public int? Coins { get; set; }

        [JsonPropertyName("coins_forzada")]
        public int? CoinsForzada { get; set; }

        [JsonPropertyName("coins_sale")]
        public int? CoinsSale { get; set; }

        [JsonPropertyName("destacado")]
        public int? Outstanding { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; }

        [JsonPropertyName("id_categoria")]
        public int IdCategory { get; set; }

        [JsonPropertyName("id_subcategoria")]
        public int IdSubCategory { get; set; }

        [JsonPropertyName("garantia")]
        public string Warranty { get; set; }

        [JsonPropertyName("id_marca")]
        public int IdBrand { get; set; }

        [JsonPropertyName("precioEspecial")]
        public int? SpecialPrice { get; set; }

        [JsonPropertyName("precioLista")]
        public int? ListPrice { get; set; }

        [JsonPropertyName("precioListaAnterior")]
        public int? PreviousListPrice { get; set; }

        [JsonPropertyName("precioEspecialAnterior")]
        public int? PreviousSpecialPrice { get; set; }

        [JsonPropertyName("imagenes")]
        public List<Image> Images { get; set; }

        [JsonPropertyName("vendible")]
        public int? Salable { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; }

        [JsonPropertyName("niveles")]
        public Level Levels { get; set; }

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
