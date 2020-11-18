using FluentResults;
using Microsoft.Extensions.Logging;

namespace Core.ResultConfig
{
    /// <summary>
    /// Adds ability to log in all <see cref="Result"/> calling .Log()
    /// </summary>
    public class Logger : FluentResults.IResultLogger
    {
        private readonly ILogger logger;

        /// <summary>
        /// Adds ability to log in all <see cref="Result"/> calling .Log()
        /// </summary>
        public Logger(ILogger logger) 
        {
            this.logger = logger;
        }
        public void Log(string context, ResultBase result)
        {
            if (result.IsFailed)
                logger.LogError("{0}", result);            
            else
                logger.LogInformation("{0}", result);

        }
    }
}
