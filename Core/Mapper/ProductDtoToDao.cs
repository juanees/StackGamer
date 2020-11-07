using Database;
using Fetcher;

namespace Core.Mapper
{
    public static class ProductDtoToDao
    {
        public static ProductDAO MapDtoToDao(this ProductDTO productDTO)
        {
            var prod = new ProductDAO()
            {
                Nombre = productDTO.Nombre,
                Vendible = productDTO.Vendible == 1,
                Codigo = productDTO.Codigo,
                IdCategoria = productDTO.IdCategoria,
                IdMarca = productDTO.IdMarca,
                IdSubcategoria = productDTO.IdSubcategoria,
                PrecioEspecial = (productDTO.PrecioEspecial ?? 0) / 100,
                PrecioEspecialAnterior = (productDTO.PrecioEspecialAnterior ?? 0) / 100,
                PrecioLista = (productDTO.PrecioLista ?? 0) / 100,
                PrecioListaAnterior = (productDTO.PrecioListaAnterior ?? 0) / 100
            };
            return prod;
        }
    }
}
