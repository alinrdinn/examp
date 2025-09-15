using ExecutiveDashboard.Modules.MostLessWin.Data.Entities;

namespace ExecutiveDashboard.Modules.MostLessWin.Repositories
{
    public interface IMostLessWinRepository
    {
        Task<List<MostLessWinEntity>> GetMostWinForLatestWeek(int yearweek, string level, string location, string platform);
    }
}
