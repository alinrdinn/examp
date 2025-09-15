using ExecutiveDashboard.Modules.ImproveDegrade.Data.Entities;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Repositories
{
    public interface IImproveDegradeRepository
    {
        Task<List<LocationWinLoseEntity>> GetAreaWinLose(int yearweek, string level, string location, string source);
    }
}
