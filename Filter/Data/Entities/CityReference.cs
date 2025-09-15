using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.Filter.Data.Entities
{
    [Keyless]
    public class CityReference
    {
        public string? city { get; set; }
    }
}
