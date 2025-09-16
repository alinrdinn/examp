namespace ExecutiveDashboard.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public string LocalizationKey { get; }

        public NotFoundException(string localizationKey) : base()
        {
            LocalizationKey = localizationKey;
        }

        public NotFoundException(string localizationKey, string message) : base(message)
        {
            LocalizationKey = localizationKey;
        }

        public NotFoundException(string localizationKey, string message, Exception innerException) : base(message, innerException)
        {
            LocalizationKey = localizationKey;
        }
    }
}