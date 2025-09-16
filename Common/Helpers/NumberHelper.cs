namespace ExecutiveDashboard.Common.Helpers
{
    public class NumberHelper
    {
        public static string FormatNumber(double value)
        {
            if (value >= 1000000000)
            {
                return "Bn";
            }
            if (value >= 1000000)
            {
                return "Mn";
            }
            else if (value >= 1000)
            {
                return "K";
            }

            return "";
        }

        public static double GetDividedValue(double value, string divisor)
        {
            if (divisor == "Tn")
            {
                return value / 1000000000000;
            }
            if (divisor == "Bn")
            {
                return value / 1000000000;
            }
            if (divisor == "Mn")
            {
                return value / 1000000;
            }
            if (divisor == "K")
            {
                return value / 1000;
            }

            return value;
        }

        public static string FormatNumberData(double value)
        {
            if (value >= (1024 * 1024 * 1024))
            {
                return "PB";
            }
            if (value >= (1024 * 1024))
            {
                return "TB";
            }
            else if (value >= 1024)
            {
                return "GB";
            }

            return "MB";
        }

        public static double GetDividedValueData(double value, string divisor)
        {
            if (divisor == "PB")
            {
                return value / (1024 * 1024 * 1024); // From MB to PB
            }
            if (divisor == "TB")
            {
                return value / (1024 * 1024); // From MB to TB
            }
            if (divisor == "GB")
            {
                return value / 1024; // From MB to GB
            }

            return value; // MB remains as is
        }
    }
}
