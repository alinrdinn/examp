using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Requests
{
    using System.ComponentModel.DataAnnotations;

    public record ImproveDegradeRequest
    {
        [Required(ErrorMessage = "yearweek_required")]
        public int? Yearweek { get; init; }

        [Required(ErrorMessage = "source_required")]
        [RegularExpression("ookla|open_signal", ErrorMessage = "source_allowed")]
        public string? Source { get; set; }
    }
}
