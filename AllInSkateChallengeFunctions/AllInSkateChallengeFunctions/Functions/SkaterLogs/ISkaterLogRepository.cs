namespace AllInSkateChallengeFunctions.Functions.SkaterLogs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISkaterLogRepository
    {
        Task<IList<SkaterLogEntry>> Get(int skip, int take);
    }
}