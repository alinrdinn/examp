namespace ExecutiveDashboard.Common.Constant
{
    public static class Constant
    {
        // This is just a sample constant class for corporate dashboard, so it is easy to call variables.
        // Add more constants for data that never change, so they can be easily accessed.

        // Response
        public const string GET_RESPONSE_SUCCESS = "Get Data Successfully";
        public const string WORST = "worst";
        public const string BEST = "best";

        // Constants for byte size conversions
        public const double OneKB = 1024;
        public const double OneMB = OneKB * 1024;
        public const double OneGB = OneMB * 1024;
        public const double OneTB = OneGB * 1024;
        public const double OnePB = OneTB * 1024;

        // Format date
        public const string DEFAULT_FORMAT_DATE = "yyyy-MM-dd";

        // Increment / decrement
        public const string INCREMENT = "increment";
        public const string DECREMENT = "decrement";

        // Repository
        public const string ETKABUPATEN = "@Kabupaten";
        public const string ETDATE = "@DATE";

        // Scatter
        public static readonly string[] QUADRANTS = { "Q1", "Q2", "Q3", "Q4" };
        public static readonly string[] SCATTER_LABELS =
        {
            "Attack",
            "Disturb",
            "Defend",
            "Defend",
        };

        // Title
        public const string UTILIZATION = "Utilization";
        public const string PREPAID_USAGE = "Prepaid Usage";
        public const string NEW_INFLOW = "New Inflow (1-6) Prepaid";
        public const string EXISTING = "Existing (>6) Prepaid";
        public const string PAYING_USER_PREPAID = "Paying User Prepaid";
        public const string PAYLOAD = "Payload";

        // Granularity
        public const string MTD = "MTD";

        public class RankingResult
        {
            public int Rank { get; set; }
            public string? BestMobileNetworkProvider { get; set; }
            public double BestMobileNetwork { get; set; }
            public string? FastestMobileNetworkProvider { get; set; }
            public double FastestMobileNetwork { get; set; }
            public string? BestMobileVideoExperienceProvider { get; set; }
            public double BestMobileVideoExperience { get; set; }
            public string? BestMobileGamingExperienceProvider { get; set; }
            public double BestMobileGamingExperience { get; set; }
            public string? BestMobileCoverageProvider { get; set; }
            public double BestMobileCoverage { get; set; }
            public string? TopRatedMobileNetworkProvider { get; set; }
            public double TopRatedMobileNetwork { get; set; }
            public string? Best5gNetworkProvider { get; set; }
            public double Best5gNetwork { get; set; }
            public string? Fastest5gNetworkProvider { get; set; }
            public double Fastest5gNetwork { get; set; }
            public string? Best5gMobileVideoExperienceProvider { get; set; }
            public double Best5gMobileVideoExperience { get; set; }
            public string? Best5gMobileGamingExperienceProvider { get; set; }
            public double Best5gMobileGamingExperience { get; set; }
        }

        public class GapResult
        {
            public string? GapStatus { get; set; }
            public double BestMobileNetworkGap { get; set; }
            public double FastestMobileNetworkGap { get; set; }
            public double BestMobileVideoExperienceGap { get; set; }
            public double BestMobileGamingExperienceGap { get; set; }
            public double BestMobileCoverageGap { get; set; }
            public double TopRatedMobileNetworkGap { get; set; }
            public double Best5gNetworkGap { get; set; }
            public double Fastest5gNetworkGap { get; set; }
            public double Best5gMobileVideoExperienceGap { get; set; }
            public double Best5gMobileGamingExperienceGap { get; set; }
        }
    }
}
