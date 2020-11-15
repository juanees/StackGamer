using FluentResults;
using System;
using System.Collections.Generic;

namespace Services.Errors.ParametersService
{
    public class ParameterNotFoundError : Error
    {
        public string Key { get; }
        public ParameterNotFoundError(string message,string key) : base(message)
        {
            Key = key;
        }
    }
}
