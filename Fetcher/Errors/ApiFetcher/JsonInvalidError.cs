using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fetcher.Errors.ApiFetcher
{
    public class JsonInvalidError : Error
    {
        public JsonInvalidError(string error) : base(error)
        {

        }
    }
}
