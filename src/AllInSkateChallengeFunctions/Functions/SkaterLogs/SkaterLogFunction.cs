namespace AllInSkateChallengeFunctions.Functions.SkaterLogs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    public class SkaterLogFunction
    {
        private readonly ISkaterLogRepository repository;

        public SkaterLogFunction(ISkaterLogRepository repository)
        {
            this.repository = repository;
        }

        [FunctionName("SkaterLog")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SkaterLog/List/")]
            HttpRequest req,
            ILogger log)
        {
            var take = GetTakeValue(req);
            var skip = GetSkipValue(req);
            var data = await repository.Get(skip, take);

            return new OkObjectResult(data);
        }

        private int GetTakeValue(HttpRequest req)
        {
            var takeValue = 10;
            if (int.TryParse(req.Query["take"], out var parsedValue))
            {
                takeValue = parsedValue;
            }

            return takeValue;
        }

        private int GetSkipValue(HttpRequest req)
        {
            var skipValue = 0;
            if (int.TryParse(req.Query["skip"], out var parsedValue))
            {
                skipValue = parsedValue;
            }

            return skipValue;
        }
    }
}
