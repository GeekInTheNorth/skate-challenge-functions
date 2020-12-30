namespace AllInSkateChallengeFunctions.Functions.Events
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Azure.EventGrid.Models;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.EventGrid;

    public class SkateLogUpdatedEvent
    {
        private readonly IEventStatisticRepository repository;

        public SkateLogUpdatedEvent(IEventStatisticRepository repository)
        {
            this.repository = repository;
        }

        [FunctionName("SkateLogUpdatedEventFunction")]
        public async Task SkateLogUpdatedEventFunction([EventGridTrigger]EventGridEvent eventGridEvent)
        {
            if (!string.Equals(eventGridEvent.EventType, EventTypes.SkateLogUpdate, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("The triggering event has an incorrect Event Type", nameof(eventGridEvent));
            }

            await repository.Update();
        }
    }
}
