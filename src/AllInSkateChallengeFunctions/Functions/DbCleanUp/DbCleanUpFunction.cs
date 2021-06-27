using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AllInSkateChallengeFunctions.Functions.DbCleanUp
{
    public class DbCleanUpFunction
    {
        private readonly IDbCleanUpRepository repository;

        public DbCleanUpFunction(IDbCleanUpRepository repository)
        {
            this.repository = repository;
        }

        [FunctionName("DbCleanUpFunction")]
        public void Run([TimerTrigger("0 30 9 * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"Starting Clean of Strava Events at: {DateTime.Now}");
                repository.CleanStravaEvents();
                log.LogInformation($"Completed Clean of Strava Events at: {DateTime.Now}");

                log.LogInformation($"Starting Clean of Strava Integration Logs at: {DateTime.Now}");
                repository.CleanStravaIntegrationLogs();
                log.LogInformation($"Completed Clean of Strava Integration Logs at: {DateTime.Now}");
            }
            catch(Exception exception)
            {
                log.LogError("Error encountered executing Strava events and logs clean up.", exception);
            }
        }
    }
}
