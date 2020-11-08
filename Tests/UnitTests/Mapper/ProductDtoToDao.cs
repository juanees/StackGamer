using Core.Mapper;
using Fetcher;
using Fetcher.Model;
using NUnit.Framework;

namespace Tests.UnitTests.Mapper
{

    [TestFixture]
    public class ProductDtoToDao
    {
        private ProductDTO _productDTO;

        [SetUp]
        public void SetUp()
        {
            _productDTO = new ProductDTO()
            {
                Nombre = "  test TEST ñ 🤡 ",
                Vendible = null,
                Codigo = "COD 1",
                IdCategoria = 1,
                IdMarca = 2,
                IdSubcategoria = 3,
                PrecioEspecial = 100,
                PrecioEspecialAnterior = 200,
                PrecioLista = null,
                PrecioListaAnterior = 0
            };
        }

        [Test]
        public void MapDtoToDao_IsWorkingCorrectly()
        {
            int codeProd = int.MaxValue;
            var result = _productDTO.MapDtoToDao(codeProd);

            Assert.AreEqual(result.Nombre, _productDTO.Nombre);

            Assert.AreEqual(result.CodigoProducto, codeProd);

            Assert.IsFalse(result.Vendible);

            Assert.AreEqual(result.Codigo, _productDTO.Codigo);

            Assert.AreEqual(result.IdCategoria, _productDTO.IdCategoria);

            Assert.AreEqual(result.IdMarca, _productDTO.IdMarca);

            Assert.AreEqual(result.IdSubcategoria, _productDTO.IdSubcategoria);

            Assert.AreEqual(result.PrecioEspecial * 100, _productDTO.PrecioEspecial);

            Assert.AreEqual(result.PrecioEspecialAnterior * 100, _productDTO.PrecioEspecialAnterior);

            Assert.AreEqual(result.PrecioLista, 0);

            Assert.AreEqual(result.PrecioListaAnterior * 100, _productDTO.PrecioListaAnterior);
        }
    }
}
