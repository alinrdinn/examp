using ExecutiveDashboard.Modules.ImproveDegrade.Data.Entities;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Repositories
{
    public interface IImproveDegradeRepository
    {
        Task<List<LocationWinLoseEntity>> GetAreaWinLose(
            int yearweek,
            string source
        );
        Task<List<ImproveMaintainDegradeEntity>> GetRegionCityMappings(int yearweek, string source);
    }
}
