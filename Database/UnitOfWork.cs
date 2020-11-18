using Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class UnitOfWork : IDisposable
    {
        private readonly StackGameContext context;
        private GenericRepository<Category> categoryRepository;
        //private GenericRepository<Product> productRepository;

        public UnitOfWork(StackGameContext _context)
        {
            context = _context;
        }
        public GenericRepository<Category> CategoryRepository
        {
            get
            {

                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new GenericRepository<Category>(context);
                }
                return categoryRepository;
            }
        }

        //public GenericRepository<Product> ProductRepository
        //{
        //    get
        //    {

        //        if (this.productRepository == null)
        //        {
        //            this.productRepository = new GenericRepository<Product>(context);
        //        }
        //        return productRepository;
        //    }
        //}

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

