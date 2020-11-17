using Database;
using Database.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Services.Errors.ParametersService;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.IntegrationTests.Services
{
    [TestFixture]
    public class ParametersService
    {
        [Test]
        public async Task Services_ParametersService_Should_Return_Parameter()
        {
            // Setup
            DbContextOptions<StackGameContext> options = GenerateDbContextOptions();
            Parameter parameter = new()
            {
                Id = 988,
                Key = "test_parameter",
                Description = "Test parameter in memory database",
                Value = "Working!!!"
            };
            // Seed
            using (StackGameContext dbContext = new StackGameContext(options))
            {
                await dbContext.Parameters.AddAsync(parameter);
                await dbContext.SaveChangesAsync();
            }
            var _memoryCache =new MemoryCache(new MemoryCacheOptions());
            var _logger = Mocks.MocksCreator.CreateLoggerMock<global::Services.ParametersService>();
            using var dbContext2 = new StackGameContext(options);
            var parametersService = new global::Services.ParametersService(_memoryCache, _logger.Object, dbContext2);

            var result = await parametersService.GetParameterAsync(parameter.Key);
            var parameterRetrived = result.Value;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsFailed);
            Assert.IsNotNull(parameterRetrived);

            Assert.AreEqual(parameter.Id, parameterRetrived.Id);
            Assert.AreEqual(parameter.Description, parameterRetrived.Description);
            Assert.AreEqual(parameter.Value, parameterRetrived.Value);
        } 

        [Test]
        public async Task Services_ParametersService_Should_Return_Error_Result_When_Parameter_Key_Does_Not_Exists()
        {
            // Setup
            DbContextOptions<StackGameContext> options = GenerateDbContextOptions();
            var _memoryCache =new MemoryCache(new MemoryCacheOptions());
            var _logger = Mocks.MocksCreator.CreateLoggerMock<global::Services.ParametersService>();
            using var dbContext = new StackGameContext(options);
            var parametersService = new global::Services.ParametersService(_memoryCache, _logger.Object, dbContext);
            var key = "THIS_KEY_DOES_NOT_EXISTS";
            var result = await parametersService.GetParameterAsync(key);
            

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsFailed);
            Assert.IsFalse(result.IsSuccess);
            Assert.Throws<InvalidOperationException>(delegate { var _ = result.Value; });
            Assert.IsTrue(result.HasError<ParameterNotFoundError>());
            ParameterNotFoundError error = (ParameterNotFoundError) result.Errors.Find(x => x.GetType() == typeof(ParameterNotFoundError));
            Assert.IsNotNull(error);
            Assert.AreEqual(key, error.Key);
        }

        private static DbContextOptions<Database.StackGameContext> GenerateDbContextOptions()
        {
            string dbName = Guid.NewGuid().ToString();
            DbContextOptions<Database.StackGameContext> options = new DbContextOptionsBuilder<Database.StackGameContext>()
                            .UseInMemoryDatabase(dbName).Options;
            return options;
        }
    }
}
