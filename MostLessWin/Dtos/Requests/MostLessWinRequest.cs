using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutiveDashboard.Modules.MostLessWin.Dtos.Requests
{
    using System.ComponentModel.DataAnnotations;

    public record MostLessWinRequest
    {
        [Required(ErrorMessage = "yearweek_required")]
        public int? Yearweek { get; init; }

        [Required(ErrorMessage = "level_required")]
        [RegularExpression("nation|region|kabupaten", ErrorMessage = "level_allowed")]
        public string? Level { get; init; }

        [Required(ErrorMessage = "location_required")]
        public string? Location { get; init; }

        [Required(ErrorMessage = "source_required")]
        [RegularExpression("ookla|open_signal", ErrorMessage = "source_allowed")]
        public string? Source { get; set; }
    }
}
