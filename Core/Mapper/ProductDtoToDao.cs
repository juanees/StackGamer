using Database;
using Fetcher.Model;

namespace Core.Mapper
{
    public static class ProductDtoToDao
    {
        public static ProductDAO MapDtoToDao(this ProductDTO productDTO, int prodCode)
        {
            var prod = new ProductDAO()
            {
                Nombre = productDTO.Nombre,
                CodigoProducto = prodCode,
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
