
using backend_dtap.Modules.Qf.Executive.Domain.Entities;

namespace backend_dtap.Modules.Qf.Executive.Entites.Repositories
{
    public interface IExecRepo
    {
        Task<List<ExecProdWithStatus>> GetProdsSubsWithStatus(List<string> kpis, int time, string level, string location);
        Task<List<ExecServiceProdCat>> GetServiceProd(int yearweek, string category, string level, string location);
        Task<List<ExecCustExpPerceive>> GetCustExpPerceives(int yearweek, string level, string location);
        Task<List<ExecCustExpMeasureKqi>> GetCustExpMeasureKQI(int yearweek, string level, string location);
        Task<List<ExecCustExpMeasureCei>> GetCustExpMeasureCei(int yearweek, string level, string location);
        Task<ExecServiceExp?> GetServiceExp(int yearweek, string level, string location);
        Task<List<ExecBenchmark>> GetBenchmark(int time, string level, string location, string feature);
        Task<ExecNetworkWithStatusTarget?> GetNetworkWithStatusTarget(string kpi, int time, string level, string location);
        Task<List<ExecNetworkWithRegional>> GetNetworkTopLocations(string geographicLevel, string locationName, int timeFrame, string metricCategory);
        Task<List<ExecNetworkTopKpi>> GetGetNetworkTopKpis(string kpi, string level, string locationName, int yearweek);
        Task<List<ExecSeriesEntity>> GetNetworkSeries(string kpi, string level, string locationName, string startYearweek, string endYearweek);
    }
}