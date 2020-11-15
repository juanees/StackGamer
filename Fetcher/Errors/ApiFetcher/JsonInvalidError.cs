using FluentResults;

namespace Fetcher.Errors.ApiFetcher
{
    public class JsonInvalidError : Error
    {
        public JsonInvalidError(string error) : base(error)
        {

        }
    }
}
