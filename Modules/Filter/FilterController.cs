using System.Collections.Generic;
using System.Threading.Tasks;
using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.Filter.Dtos.Requests;
using ExecutiveDashboard.Modules.Filter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExecutiveDashboard.Modules.Filter
{
    [ApiController]
    [Route("v1/filters")]
    public class FilterController : ControllerBase
    {
        private readonly IFilterService _service;

        public FilterController(IFilterService service)
        {
            _service = service;
        }

        [HttpGet("yearweeks", Name = "GetYearweeks")]
        public async Task<ActionResult<BaseResponse>> GetYearweeks()
        {
            var result = await _service.GetYearweeks();
            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );
            return Ok(response);
        }

        [HttpGet("locations", Name = "GetLocations")]
        public async Task<ActionResult<BaseResponse>> GetLocations(
            [FromQuery] FilterLocationRequest request
        )
        {
            var result = await _service.GetLocations(request);
            var response = BaseResponse.ToResponse(
                code: 200,
                success: true,
                data: result,
                errors: null
            );
            return Ok(response);
        }

        [HttpGet("cities-by-region", Name = "GetCitiesByRegion")]
        public async Task<ActionResult<BaseResponse>> GetCitiesByRegion(
            [FromQuery] FilterRegionRequest request
        )
        {
            var result = await _service.GetCitiesByRegion(request);
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
