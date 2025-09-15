using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutiveDashboard.Modules.MostLessWin.Dtos.Responses
{
    public class MostLessWinResponse
    {
        public string? MostWinRegion { get; set; }
        public int? MostWinCount { get; set; }
        public int? MostWinOutOf { get; set; }
        public string? LessWinRegion { get; set; }
        public int? LessWinCount { get; set; }
        public int? LessWinOutOf { get; set; }
    }
}
