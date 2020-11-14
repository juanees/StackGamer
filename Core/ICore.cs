using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Core
{
    public interface ICore : IDisposable
    {        
        public ILogger<T> GetLogger<T>();
        public ServiceProvider SetUp(params string[] args);
    }
}
