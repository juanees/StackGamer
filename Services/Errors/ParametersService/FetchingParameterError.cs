using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Errors.ParametersService
{
    class FetchingParameterError : Error
    {
        public FetchingParameterError(string message) : base(message)
        {
        }
    }
}
