namespace AllInSkateChallengeFunctions.Functions.SkaterLogs
{
    using System.Collections.Generic;

    public class SkaterLogResult
    {
        public int Total { get; set; }

        public int Remaining { get; set; }

        public List<SkaterLogEntry> Log { get; set; }
    }
}