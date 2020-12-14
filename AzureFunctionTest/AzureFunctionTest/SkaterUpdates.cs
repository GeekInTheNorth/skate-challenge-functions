namespace AzureFunctionTest
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    public class SkaterUpdates
    {
        [FunctionName("SkaterUpdates")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SkaterUpdates/List/")]
            HttpRequest req,
            ILogger log)
        {
            var responseMessage = "This will be a list of activities by different skaters";

            return new OkObjectResult(responseMessage);
        }
    }
}
