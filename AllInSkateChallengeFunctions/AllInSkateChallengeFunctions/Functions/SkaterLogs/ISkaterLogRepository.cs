namespace AllInSkateChallengeFunctions.Functions.SkaterLogs
{
    using System.Threading.Tasks;

    public interface ISkaterLogRepository
    {
        Task<SkaterLogResult> Get(int skip, int take);
    }
}