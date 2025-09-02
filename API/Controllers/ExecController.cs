using Microsoft.AspNetCore.Mvc;
using backend_dtap.Modules.Base.Application.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using backend_dtap.Modules.Qf.Executive.Application.Interfaces;
using backend_dtap.Modules.Qf.Executive.Application.DTOs.Requests;

namespace backend_dtap.Modules.Qf.Executive.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class ExecController : ControllerBase
    {

        private readonly ILogger<ExecController> _logger;
        private readonly IExecService _service;

        public ExecController(ILogger<ExecController> logger, IExecService service)
        {
            _logger = logger;
            _service = service;

        }


        [Authorize(Policy = "BasicRole")]
        [HttpPost("v1/executive-v2/productivity", Name = "GetProdsWithStatus")]
        public async Task<ActionResult<BaseResponse>> GetProdsWithStatus([FromBody] ExecRequest request)
        {
            var result = await _service.GetProdsWithStatus(request);

            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );

            return Ok(response);
        }



        [Authorize(Policy = "BasicRole")]
        [HttpPost("v1/executive-v2/subscriber", Name = "GetSubsWithStatus")]
        public async Task<ActionResult<BaseResponse>> GetSubsWithStatus([FromBody] ExecRequest request)
        {
            var result = await _service.GetSubsWithStatus(request);

            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );

            return Ok(response);
        }

        [Authorize(Policy = "BasicRole")]
        [HttpPost("v1/executive/service-productivity", Name = "GetServiceProdCat")]
        public async Task<ActionResult<BaseResponse>> GetServiceProdCat([FromBody] ExecRequest request)
        {
            var result = await _service.GetServiceProdCat(request);

            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );

            return Ok(response);
        }

        [Authorize(Policy = "BasicRole")]
        [HttpPost("v1/executive/top-application", Name = "GetServiceProdApps")]
        public async Task<ActionResult<BaseResponse>> GetServiceProdApps([FromBody] ExecRequest request)
        {
            var result = await _service.GetServiceProdApps(request);

            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );

            return Ok(response);
        }

        [Authorize(Policy = "BasicRole")]
        [HttpPost("v1/executive-v2/customer-experience-perceive", Name = "GetCustomerPerspective")]
        public async Task<ActionResult<BaseResponse>> GetCustExpPerceive([FromBody] ExecRequest request)
        {
            var result = await _service.GetCustExpPerceive(request);

            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );

            return Ok(response);
        }
        [Authorize(Policy = "BasicRole")]
        [HttpPost("v1/executive-v2/customer-experience-measure", Name = "GetExecCustExpMeasure")]
        public async Task<ActionResult<BaseResponse>> GetExecCustExpMeasure([FromBody] ExecRequest request)
        {
            var result = await _service.GetCustExpMeasure(request);

            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );

            return Ok(response);
        }


        [Authorize(Policy = "BasicRole")]
        [HttpPost("v1/executive-v2/service-experience", Name = "GetExecutiveServiceExp")]
        public async Task<ActionResult<BaseResponse>> GetExecutiveServiceExp([FromBody] ExecRequest request)
        {
            var result = await _service.GetExecServiceExp(request);

            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );

            return Ok(response);
        }


        [Authorize(Policy = "BasicRole")]
        [HttpPost("v1/executive-v2/benchmark", Name = "GetExecBenchmark")]
        public async Task<ActionResult<BaseResponse>> GetExecBenchmark([FromBody] ExecFeatureRequest request)
        {
            var result = await _service.GetExecBenchmark(request);

            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );

            return Ok(response);
        }
        [Authorize(Policy = "BasicRole")]
        [HttpPost("v1/executive-v2/network", Name = "GetExecNetwork")]
        public async Task<ActionResult<BaseResponse>> GetExecNetwork([FromBody] ExecFeatureRequest request)
        {
            var result = await _service.GetExecNetwork(request);

            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );

            return Ok(response);
        }
    }
}
