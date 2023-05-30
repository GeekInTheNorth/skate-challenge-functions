using AllInSkateChallenge.Features.Common;

namespace AllInSkateChallengeFunctions.Functions.LeaderBoard
{
    public class LeaderBoardEntry
    {
        public int Position { get; set; }

        public string ProfileImage { get; set; }

        public string SkaterName { get; set; }

        public decimal TotalMiles { get; set; }

        public decimal TotalKilometres => Conversion.MilesToKilometres(TotalMiles);
    }
}