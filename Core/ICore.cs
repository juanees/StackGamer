using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Core
{
    /// <summary>
    /// This interface is resposible to setup all the dependencies and offers an <see cref="ILogger[T]"/> for differents types
    /// All the Public classes in this project should use this interface
    /// </summary>
    public interface ICore : IDisposable
    {
        /// <summary>
        /// Returns an <see cref="ILogger[T]"/> for the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ILogger<T> GetLogger<T>();

        /// <summary>
        /// The implementation of SetUp is in charge of configuring all the dependencies
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public void SetUp(params string[] args);
    }
}
