namespace AzureFunctionTest
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    public static class LeaderBoard
    {
        [FunctionName("LeaderBoard")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{target}")] HttpRequest req,
            string target,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var skateTarget = GetSkateTarget(target);
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        private static SkateTarget GetSkateTarget(string target)
        {
            if (!Enum.TryParse<SkateTarget>(target, out var skateTarget))
            {
                return SkateTarget.LiverpoolCanningDock;
            }

            return skateTarget;
        }
    }
}
