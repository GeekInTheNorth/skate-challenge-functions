namespace AllInSkateChallengeFunctions.Functions.LeaderBoard
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILeaderBoardRepository
    {
        Task<IList<LeaderBoardEntry>> Get(SkateTarget target);
    }
}