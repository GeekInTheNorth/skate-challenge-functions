namespace AllInSkateChallengeFunctions.Functions.SkaterLogs
{
    using System;

    public class SkaterLogEntry
    {
        public string ProfileImage { get; set; }

        public string SkaterName { get; set; }

        public DateTime Logged { get; set; }

        public decimal Miles { get; set; }

        public string ActivityName { get; set; }
    }
}