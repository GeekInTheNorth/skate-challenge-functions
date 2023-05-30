namespace AllInSkateChallengeFunctions.Functions.SkaterLogs
{
    using System;

    using AllInSkateChallenge.Features.Common;

    public class SkaterLogEntry
    {
        public string ProfileImage { get; set; }

        public string SkaterName { get; set; }

        public DateTime Logged { get; set; }

        public decimal Miles { get; set; }

        public decimal Kilometres => Conversion.MilesToKilometres(Miles);

        public string ActivityName { get; set; }
    }
}