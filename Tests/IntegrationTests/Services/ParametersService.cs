using Database;
using Database.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using System;
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
                ParameterId = 988,
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

            var parameterRetrive = parametersService.GetParameter(parameter.Key);
            
            Assert.IsNotNull(parameterRetrive);
            Assert.AreEqual(parameter.ParameterId, parameterRetrive.ParameterId);
            Assert.AreEqual(parameter.Description, parameterRetrive.Description);
            Assert.AreEqual(parameter.Value, parameterRetrive.Value);

            await dbContext2.DisposeAsync();
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
