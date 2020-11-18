using FluentResults;

namespace Fetcher.Errors.ApiFetcher
{
    public class TimeOutError : Error
    {
        public TimeOutError(string error) : base(error)
        {

        }
    }
}
