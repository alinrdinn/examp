using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutiveDashboard.Modules.OLOPerformance.Dtos.Requests
{
    using System.ComponentModel.DataAnnotations;

    public record OLOPerformanceRequest
    {
        [Required(ErrorMessage = "yearweek_required")]
        public int? Yearweek { get; init; }

        [Required(ErrorMessage = "level_required")]
        [RegularExpression("nation|region|kabupaten", ErrorMessage = "level_allowed")]
        public string? Level { get; init; }

        [Required(ErrorMessage = "location_required")]
        public string? Location { get; init; }
    }
}
