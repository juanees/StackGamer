using FluentResults;

namespace Fetcher.Errors.ApiFetcher
{
    public class ContentTypeNotValidError : Error
    {
        public ContentTypeNotValidError(string message) : base(message)
        {
        }
    }
}
