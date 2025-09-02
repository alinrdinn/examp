using backend_dtap.Modules.Base.Domain.Entities;

namespace backend_dtap.Modules.Qf.Executive.Application.DTOs.Requests
{
    public class ExecRequest
    {
        public required string Level { get; set; }
        public required string Location { get; set; }
        public required string Period { get; set; }
        public required string Time { get; set; }
    }
    public class ExecFeatureRequest : ExecRequest
    {
        public required string Feature { get; set; }
    }
    
}