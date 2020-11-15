using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fetcher.Errors.ApiFetcher
{
    public class ContentTypeNotValidError : Error
    {
        public ContentTypeNotValidError(string message) : base(message)
        {
        }
    }
}
