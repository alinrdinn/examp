using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.Filter.Data.Entities
{
    [Keyless]
    public class RegionReference
    {
        public string? region { get; set; }
    }
}
