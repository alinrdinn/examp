using System.ComponentModel.DataAnnotations;

namespace ExecutiveDashboard.Modules.Filter.Dtos.Requests
{
    public class FilterLocationRequest
    {
        [Required(ErrorMessage = "level_required")]
        [RegularExpression("nation|region|kabupaten", ErrorMessage = "level_allowed")]
        public string? Level { get; init; }
    }

    public class FilterRegionRequest
    {
        [Required(ErrorMessage = "region_required")]
        public string? Region { get; init; }
    }
}
