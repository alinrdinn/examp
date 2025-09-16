using System.ComponentModel.DataAnnotations;

namespace ExecutiveDashboard.Modules.Summary.Dtos.Requests
{
    public record SummaryRequest
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
