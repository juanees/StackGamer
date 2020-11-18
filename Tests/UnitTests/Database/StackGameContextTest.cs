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
    public class StackGameContextTest
    {
        [Test]
        public async Task Database_StackGameContext_Category_Should_Be_Added()
        {
            // Setup
            DbContextOptions<global::Database.StackGameContext> options = GenerateDbContextOptions();

            // Seed
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                Category cat = new()
                {
                    Id = 1,
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
                    //SpecialPrice = 1000,
                    //PreviousSpecialPrice = 900,
                    //ListPrice = 1200,
                    //PreviousListPrice = 1100,
                    Id = 1,
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
                var category = await dbContext.Categories.Include(c => c.Products).SingleAsync(x => x.Id == 1);
                Assert.IsNotNull(category);
                Assert.AreEqual(1, category.Id);
                Assert.AreEqual(123, category.ExternalCategoryId);
                Assert.AreEqual("CategoryName", category.Name);
                Assert.AreEqual(1, category.Products.Count);
                Assert.IsNotNull(category.CreatedDate);
                Assert.IsNotNull(category.UpdatedDate);

                var product = category.Products.ToList().First();
                Assert.IsNotNull(product);
                Assert.AreEqual(1, product.Id);
                Assert.AreEqual("ProductName", product.Name);
                Assert.AreEqual(100, product.ExternalProductId);
                Assert.AreEqual(true, product.Saleable);
                Assert.AreEqual("ProductCode", product.Code);
                Assert.AreEqual(1, product.BrandId);
                //Assert.AreEqual(1000, product.SpecialPrice);
                //Assert.AreEqual(900, product.PreviousSpecialPrice);
                //Assert.AreEqual(1200, product.ListPrice);
                //Assert.AreEqual(1100, product.PreviousListPrice);
                Assert.AreEqual(1, product.Id);
                Assert.AreEqual(1, product.CategoryId);
                Assert.AreEqual(category, product.Category);
                Assert.AreSame(category, product.Category);
                Assert.IsNotNull(product.CreatedDate);
                Assert.IsNotNull(product.UpdatedDate);
            }
        }

        [Test]
        public async Task Database_StackGameContext_Product_Should_Be_Added()
        {
            // Setup
            DbContextOptions<global::Database.StackGameContext> options = GenerateDbContextOptions();

            // Seed
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                Category cat = new()
                {
                    Id = 1,
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
                    Id = 1,
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
                var category = await dbContext.Categories.Include(c => c.Products).SingleAsync(x => x.Id == 1);
                Assert.IsNotNull(category);
                Assert.AreEqual(1, category.Id);
                Assert.AreEqual(123, category.ExternalCategoryId);
                Assert.AreEqual("CategoryName", category.Name);
                Assert.AreEqual(1, category.Products.Count);
                Assert.IsNotNull(category.CreatedDate);
                Assert.IsNotNull(category.UpdatedDate);

                var product = category.Products.ToList().First();
                Assert.IsNotNull(product);
                Assert.AreEqual(1, product.Id);
                Assert.AreEqual("ProductName", product.Name);
                Assert.AreEqual(100, product.ExternalProductId);
                Assert.AreEqual(true, product.Saleable);
                Assert.AreEqual("ProductCode", product.Code);
                Assert.AreEqual(1, product.BrandId);
                Assert.AreEqual(1, product.Id);
                Assert.AreEqual(1, product.CategoryId);
                Assert.AreEqual(category, product.Category);
                Assert.AreSame(category, product.Category);
                Assert.IsNotNull(product.CreatedDate);
                Assert.IsNotNull(product.UpdatedDate);
            }
        }

        [Test]
        public async Task Database_StackGameContext_Product_And_ProductPrice_Should_Be_Added()
        {
            // Setup
            DbContextOptions<global::Database.StackGameContext> options = GenerateDbContextOptions();
            ProductPrice productPrice;
            // Seed
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                Category cat = new()
                {
                    Id = 1,
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
                    Prices = new List<ProductPrice>(),
                    Id = 1,
                    Category = cat
                };
                productPrice = new()
                {
                    ListPrice = 100.00,
                    PreviousListPrice = 90.00,
                    PreviousSpecialPrice = 80.00,
                    SpecialPrice = 90.00,
                    Product = prod
                };
                prod.Prices.Add(productPrice);
                cat.Products.Add(prod);

                await dbContext.Categories.AddAsync(cat);
                await dbContext.Products.AddAsync(prod);
                await dbContext.SaveChangesAsync();
            }

            //Test
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                var category = await dbContext.Categories.Include(cat => cat.Products).ThenInclude(prod => prod.Prices).SingleAsync(x => x.Id == 1);
                Assert.IsNotNull(category);

                var product = category.Products.ToList().First();
                Assert.IsNotNull(product);

                var productPrice_ = product.Prices.First();
                Assert.IsNotNull(productPrice_);

                Assert.AreEqual(productPrice.ListPrice, productPrice_.ListPrice);
                Assert.AreEqual(productPrice.PreviousListPrice, productPrice_.PreviousListPrice);
                Assert.AreEqual(productPrice.PreviousSpecialPrice, productPrice_.PreviousSpecialPrice);
                Assert.AreEqual(productPrice.SpecialPrice, productPrice_.SpecialPrice);
            }
        }

        [Test]
        public async Task Database_StackGameContext_Product_Should_Not_Be_Added_If_Category_Is_Not_Set()
        {
            // Setup
            DbContextOptions<global::Database.StackGameContext> options = GenerateDbContextOptions();

            // Seed
            Product prod = new()
            {
                Name = "ProductName",
                ExternalProductId = 100,
                Saleable = true,
                Code = "ProductCode",
                BrandId = 1,
                Prices = new List<ProductPrice>(),
                Id = 1
            };


            //Test
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                try
                {
                    await dbContext.Products.AddAsync(prod);
                    await dbContext.SaveChangesAsync();
                    Assert.Fail("Should fail if Category is null");
                }
                catch (Exception)
                {
                    Assert.Pass("Category is null");
                }
            }
        }       

        [Test]
        public async Task Database_StackGameContext_Parameter_Should_Be_Added()
        {
            // Setup
            DbContextOptions<global::Database.StackGameContext> options = GenerateDbContextOptions();
            Parameter parameter = new()
            {
                Id = 988,
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
                Assert.AreEqual(parameter.Id, parameterRetrive.Id);
                Assert.AreEqual(parameter.Id, parameterRetrive.Id);
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
                    Id = 1,
                    ExternalCategoryId = 123,
                    Name = "CategoryName",
                    Products = new List<Product>()
                };
                await dbContext.Categories.AddAsync(cat);
                await dbContext.SaveChangesAsync();
            }
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                var category = await dbContext.Categories.SingleAsync(x => x.Id == 1);
                category.Name = "UpdatedName";
                dbContext.Categories.Update(category);
                await dbContext.SaveChangesAsync();
            }

            //Test
            using (global::Database.StackGameContext dbContext = new global::Database.StackGameContext(options))
            {
                var category = await dbContext.Categories.Include(c => c.Products).SingleAsync(x => x.Id == 1);

                Assert.IsNotNull(category);
                Assert.AreEqual(1, category.Id);
                Assert.AreEqual(123, category.ExternalCategoryId);
                Assert.AreEqual("UpdatedName", category.Name);
                Assert.AreEqual(0, category.Products.Count);
                Assert.IsNotNull(category.CreatedDate);
                Assert.IsNotNull(category.UpdatedDate);
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
