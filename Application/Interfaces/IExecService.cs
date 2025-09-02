
using backend_dtap.Modules.Qf.Executive.Application.DTOs.Requests;
using backend_dtap.Modules.Qf.Executive.Application.DTOs.Responses;

namespace backend_dtap.Modules.Qf.Executive.Application.Interfaces
{
    public interface IExecService
    {
        Task<ExecProdResponse?> GetProdsWithStatus(ExecRequest request);
        Task<ExecSubsResponse?> GetSubsWithStatus(ExecRequest request);
        Task<List<ExecServiceProdCatResponse>?> GetServiceProdCat(ExecRequest request);
        Task<List<ExecServiceProdAppResponse>?> GetServiceProdApps(ExecRequest request);
        Task<ExecCustExpPerceiveResponse?> GetCustExpPerceive(ExecRequest request);
        Task<ExecCustExpMeasureResponse?> GetCustExpMeasure(ExecRequest request);
        Task<ExecServiceExpResponse?> GetExecServiceExp(ExecRequest request);
        Task<IExecBenchmarkResponse?> GetExecBenchmark(ExecFeatureRequest request);
        Task<ExecNetResponse?> GetExecNetwork(ExecFeatureRequest request);
    }
}