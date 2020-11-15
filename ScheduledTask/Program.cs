namespace ScheduledTask
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.ScheduledTask;
    using Microsoft.Extensions.Logging;
    public class Program
    {
        
        public static async Task Main(string[] args)
        {
            using var fsup = new FetchAndSaveOrUpdateProducts();
            var res = await fsup.FetchAllCategoriesAndProducts();
            
            var a = 0;
        }

       
    }
}
