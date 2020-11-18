using Database;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductsService
    {        
        private readonly UnitOfWork unitOfWork;

        public ProductsService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //public async Task<List<CategoryDTO>> GetCategoriesAndProducts()
        //{
        //    return 
        //}
    }
}
