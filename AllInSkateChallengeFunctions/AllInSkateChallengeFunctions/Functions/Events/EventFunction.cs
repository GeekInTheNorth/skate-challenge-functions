namespace AllInSkateChallengeFunctions.Functions.Events
{
    using System;
    using System.IO;
    using System.Net;

    using Microsoft.Azure.EventGrid.Models;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.EventGrid;

    using Newtonsoft.Json;

    public class EventFunction
    {
        [FunctionName("EventGridTest")]
        public void EventGridTest([EventGridTrigger]EventGridEvent eventGridEvent)
        {
            var request = WebRequest.Create("https://allinskatechallenge.azurewebsites.net/api/strava/event");
            request.ContentType = "application/json";
            request.Method = "POST";

            var streamWriter = new StreamWriter(request.GetRequestStream());
            var body = JsonConvert.SerializeObject(eventGridEvent.Data);

            streamWriter.Write(body);
            streamWriter.Flush();
            streamWriter.Close();

            var response = request.GetResponse();
        }
    }
}
