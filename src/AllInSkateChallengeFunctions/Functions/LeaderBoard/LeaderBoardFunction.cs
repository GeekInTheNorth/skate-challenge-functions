namespace AllInSkateChallengeFunctions.Functions.LeaderBoard
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    public class LeaderBoardFunction
    {
        private readonly ILeaderBoardRepository repository;

        public LeaderBoardFunction(ILeaderBoardRepository repository)
        {
            this.repository = repository;
        }

        [FunctionName("LeaderBoard")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "LeaderBoard/List/{target}")] HttpRequest req,
            string target,
            ILogger log)
        {
            var skateTarget = GetSkateTarget(target);
            var data = await repository.Get(skateTarget);

            if (req.Query.ContainsKey("limit"))
            {
                var limit = int.Parse(req.Query["limit"]);

                return new OkObjectResult(data.Take(limit));
            }
            
            return new OkObjectResult(data);
        }

        private static SkateTarget GetSkateTarget(string target)
        {
            if (!Enum.TryParse<SkateTarget>(target, true, out var skateTarget))
            {
                return SkateTarget.LeedsBradfordAirport;
            }

            return skateTarget;
        }
    }
}
