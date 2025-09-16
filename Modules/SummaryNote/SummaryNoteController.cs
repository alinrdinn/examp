using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.SummaryNote.Dtos.Requests;
using ExecutiveDashboard.Modules.SummaryNote.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExecutiveDashboard.Modules.SummaryNote
{
    [ApiController]
    [Route("v1/summary-notes")]
    public class SummaryNoteController : ControllerBase
    {
        private readonly ISummaryNoteService _service;

        public SummaryNoteController(ISummaryNoteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse>> GetSummaryNotes(
            [FromQuery] SummaryNoteQueryRequest request
        )
        {
            var result = await _service.GetSummaryNotes(request);

            var response = BaseResponse.ToResponse(200, true, result, null);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> CreateSummaryNote(
            [FromBody] CreateSummaryNoteRequest request
        )
        {
            await _service.CreateSummaryNote(request);

            var response = BaseResponse.ToResponse(201, true, null, null);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseResponse>> UpdateSummaryNote(
            int id,
            [FromBody] UpdateSummaryNoteRequest request
        )
        {
            await _service.UpdateSummaryNote(id, request);

            var response = BaseResponse.ToResponse(200, true, null, null);
            return Ok(response);
        }
    }
}
