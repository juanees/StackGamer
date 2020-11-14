using Database.Model;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Tests.UnitTests.Database
{
    [TestFixture]
    public class StackGameContext
    {
        [Test]
        public async Task Database_StackGameContext_Category_And_Product_Should_Be_Added()
        {
            // Setup
            DbContextOptions<global::Database.StackGameContext> options = GenerateDbContextOptions();

            // Seed
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                Category cat = new()
                {
                    CategoryId = 1,
                    ExternalCategoryId = 123,
                    Name = "CategoryName",
                    Products = new List<Product>()
                };
                Product prod = new()
                {
                    Name = "ProductName",
                    ExternalProductId = 100,
                    Saleable = true,
                    Code = "ProductCode",
                    BrandId = 1,
                    SpecialPrice = 1000,
                    PreviousSpecialPrice = 900,
                    ListPrice = 1200,
                    PreviousListPrice = 1100,
                    ProductId = 1,
                    CategoryId = 1,
                    Category = cat
                };

                cat.Products.Add(prod);

                await dbContext.Categories.AddAsync(cat);
                await dbContext.Products.AddAsync(prod);
                await dbContext.SaveChangesAsync();
            }

            //Test
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                var category = await dbContext.Categories.Include(c => c.Products).SingleAsync(x => x.CategoryId == 1);
                Assert.IsNotNull(category);
                Assert.AreEqual(1, category.CategoryId);
                Assert.AreEqual(123, category.ExternalCategoryId);
                Assert.AreEqual("CategoryName", category.Name);
                Assert.AreEqual(1, category.Products.Count);
                Assert.IsNotNull(category.CreatedDate);
                Assert.IsNotNull(category.UpdatedDate);

                var product = category.Products.ToList().First();
                Assert.IsNotNull(product);
                Assert.AreEqual(1, product.ProductId);
                Assert.AreEqual("ProductName", product.Name);
                Assert.AreEqual(100, product.ExternalProductId);
                Assert.AreEqual(true, product.Saleable);
                Assert.AreEqual("ProductCode", product.Code);
                Assert.AreEqual(1, product.BrandId);
                Assert.AreEqual(1000, product.SpecialPrice);
                Assert.AreEqual(900, product.PreviousSpecialPrice);
                Assert.AreEqual(1200, product.ListPrice);
                Assert.AreEqual(1100, product.PreviousListPrice);
                Assert.AreEqual(1, product.ProductId);
                Assert.AreEqual(1, product.CategoryId);
                Assert.AreEqual(category, product.Category);
                Assert.AreSame(category, product.Category);
                Assert.IsNotNull(product.CreatedDate);
                Assert.IsNotNull(product.UpdatedDate);
            }
        }

        [Test]
        public async Task Database_StackGameContext_Parameter_Should_Be_Added()
        {
            // Setup
            DbContextOptions<global::Database.StackGameContext> options = GenerateDbContextOptions();
            Parameter parameter = new()
            {
                ParameterId = 988,
                Key = "test_parameter",
                Description = "Test parameter in memory database",
                Value = "Working!!!"
            };

            // Seed
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {

                await dbContext.Parameters.AddAsync(parameter);
                await dbContext.SaveChangesAsync();
            }

            //Test
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                var parameterRetrive = await dbContext.Parameters.FirstAsync();
                Assert.IsNotNull(parameterRetrive);
                Assert.AreEqual(parameter.ParameterId, parameterRetrive.ParameterId);
                Assert.AreEqual(parameter.ParameterId, parameterRetrive.ParameterId);
                Assert.AreEqual(parameter.Description, parameterRetrive.Description);
                Assert.AreEqual(parameter.Description, parameterRetrive.Description);
                Assert.IsNotNull(parameterRetrive.CreatedDate);
                Assert.IsNotNull(parameterRetrive.UpdatedDate);
            }
        }

        [Test]
        public async Task Database_StackGameContext_Category_Should_Be_Updated()
        {
            // Setup
            DbContextOptions<global::Database.StackGameContext> options = GenerateDbContextOptions();

            // Seed
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                Category cat = new()
                {
                    CategoryId = 1,
                    ExternalCategoryId = 123,
                    Name = "CategoryName",
                    Products = new List<Product>()
                };
                await dbContext.Categories.AddAsync(cat);
                await dbContext.SaveChangesAsync();
            }
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                var category = await dbContext.Categories.SingleAsync(x => x.CategoryId == 1);
                category.Name = "UpdatedName";
                dbContext.Categories.Update(category);
                await dbContext.SaveChangesAsync();
            }

            //Test
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                var category = await dbContext.Categories.Include(c => c.Products).SingleAsync(x => x.CategoryId == 1);

                Assert.IsNotNull(category);
                Assert.AreEqual(1, category.CategoryId);
                Assert.AreEqual(123, category.ExternalCategoryId);
                Assert.AreEqual("UpdatedName", category.Name);
                Assert.AreEqual(0, category.Products.Count);
                Assert.IsNotNull(category.CreatedDate);
                Assert.IsNotNull(category.UpdatedDate);
            }
        }

        [Test]
        public async Task Database_StackGameContext_Audits_Should_Be_Added_When_Entity_Change()
        {
            // Setup
            DbContextOptions<global::Database.StackGameContext> options = GenerateDbContextOptions();

            // Seed
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                Category cat = new()
                {
                    CategoryId = 1,
                    ExternalCategoryId = 123,
                    Name = "OriginalCategoryName",
                    Products = new List<Product>()
                };
                await dbContext.Categories.AddAsync(cat);
                await dbContext.SaveChangesAsync();
            }
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                var category = await dbContext.Categories.SingleAsync(x => x.CategoryId == 1);
                category.Name = "UpdatedCategoryName";
                dbContext.Categories.Update(category);
                await dbContext.SaveChangesAsync();
            }

            //Test
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                var category = await dbContext.Categories.SingleAsync(x => x.CategoryId == 1);
                var audits = dbContext.Audits.Where<Audit>(a => a.TableName == nameof(global::Database.Model.Category)).ToList<Audit>();
                var updatedAudit = audits.Where(x => x.NewValues.Contains("UpdatedCategoryName") && x.OldValues.Contains("OriginalCategoryName"));

                Assert.IsNotNull(category);
                Assert.AreEqual("UpdatedCategoryName", category.Name);
                Assert.AreEqual(2, audits.Count());
                Assert.AreEqual(1, updatedAudit.Count());
            }
        }

        private static DbContextOptions<global::Database.StackGameContext> GenerateDbContextOptions()
        {
            string dbName = Guid.NewGuid().ToString();
            DbContextOptions<global::Database.StackGameContext> options = new DbContextOptionsBuilder<global::Database.StackGameContext>()
                            .UseInMemoryDatabase(dbName).Options;
            return options;
        }
    }
}
