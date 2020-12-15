namespace AzureFunctionTest.Functions.LeaderBoard
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILeaderBoardRepository
    {
        Task<IList<LeaderBoardEntry>> Get(SkateTarget target);
    }
}