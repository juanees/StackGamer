using Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService
    {
        private readonly StackGameContext stackGameContext;
        public ProductService(StackGameContext stackGameContext)
        {
            this.stackGameContext = stackGameContext;
        }

        //public async Task<List<CategoryDTO>> GetCategoriesAndProducts()
        //{
        //    return 
        //}
    }
}
