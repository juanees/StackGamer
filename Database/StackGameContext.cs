using Database.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Database
{
    public class StackGameContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<Parameter> Parameters { get; set; }

        public StackGameContext(DbContextOptions<StackGameContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Parameter>().HasData(
                new Parameter()
                {
                    ParameterId = 1,
                    Key = "TIME_BETWEEN_QUERIES",
                    Description = "Tiempo en segundos a esperar entre consultas",
                    Value = "5"
                },
                new Parameter()
                {
                    ParameterId = 2,
                    Key = "MAX_QUERIES_PER_PRODUCT",
                    Description = "Máxima cantidad de veces que se consulta un producto",
                    Value = "5"
                },
                new Parameter()
                {
                    ParameterId = 3,
                    Key = "CATEGORIES_URL_VALIDATION_REGEX",
                    Description = "Regex para la url de categorías",
                    Value = @"\/index.php\?seccion=3&cate=([0-9]+)"
                },
                new Parameter()
                {
                    ParameterId = 4,
                    Key = "PRODUCT_ID_FROM_URL_REGEX",
                    Description = "Regex para la url de producto",
                    Value = @"\/producto\/[a-zA-Z0-9_]+_([0-9]+)\?"
                }
            );
        }

        public override int SaveChanges()
        {
            //Saves the information in memory before commiting the changes
            var auditEntries = OnBeforeSaveChanges();

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

            //Saves to the table Audits all the changes ocurred in this operation
            OnAfterSaveChanges(auditEntries);

            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            //Saves the information in memory before commiting the changes
            var auditEntries = OnBeforeSaveChanges();

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
            
            //Saves to the table Audits all the changes ocurred in this operation
            await OnAfterSaveChanges(auditEntries);

            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.GetTableName()
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    // The following condition is ok with EF Core 2.2 onwards.
                    // If you are using EF Core 2.1, you may need to change the following condition to support navigation properties: https://github.com/dotnet/efcore/issues/17700
                    // if (property.IsTemporary || (entry.State == EntityState.Added && property.Metadata.IsForeignKey()))
                    if (property.IsTemporary)
                    {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                Audits.Add(auditEntry.ToAudit());
            }

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                // Save the Audit entry
                Audits.Add(auditEntry.ToAudit());
            }
            return SaveChangesAsync();
        }
    }
}
/*
 * Add-Migration InitialCreate
 * Update-Database
 */