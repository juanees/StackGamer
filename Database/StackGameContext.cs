using Database.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Database
{
    public class StackGameContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Parameter> Parameters { get; set; }

        /*
         * * Add-Migration InitialCreate
         * * Update-Database
        */

        public StackGameContext(DbContextOptions<StackGameContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Parameter>().HasData(
                new Parameter()
                {
                    Id = 1,
                    Key = "TIME_BETWEEN_QUERIES",
                    Description = "Tiempo en segundos a esperar entre consultas",
                    Value = "5",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                },
                new Parameter()
                {
                    Id = 2,
                    Key = "MAX_QUERIES_PER_PRODUCT",
                    Description = "Máxima cantidad de veces que se consulta un producto",
                    Value = "5",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                },
                new Parameter()
                {
                    Id = 3,
                    Key = "CATEGORIES_URL_VALIDATION_REGEX",
                    Description = "Regex para la url de categorías",
                    Value = @"\/index.php\?seccion=3&cate=([0-9]+)",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                },
                new Parameter()
                {
                    Id = 4,
                    Key = "PRODUCT_ID_FROM_URL_REGEX",
                    Description = "Regex para la url de producto",
                    Value = @"\/producto\/[a-zA-Z0-9_]+_([0-9]+)\?",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                }
            );
        }

        public override int SaveChanges()
        {
            //Set the properties UpdatedDate and CreatedDate 
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));
            var date = DateTime.Now;
            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedDate = date;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = date;
                }
            }
            //Saves the changes, adding UpdatedDate and CreatedDate
            var result = base.SaveChanges();

            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            //Set the properties UpdatedDate and CreatedDate 
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }
            //Saves the changes, adding UpdatedDate and CreatedDate
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            return result;
        }
    }
}
