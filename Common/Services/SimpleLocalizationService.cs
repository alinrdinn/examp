using System.Globalization;
using Microsoft.Extensions.Localization;

namespace ExecutiveDashboard.Common.Services
{
    public class SimpleLocalizationService : IStringLocalizer<SharedResources>
    {
        private static readonly Dictionary<string, Dictionary<string, string>> LocalizedStrings =
            new()
            {
                ["en"] = new Dictionary<string, string>
                {
                    ["status_required"] = "status is required.",
                    ["level_required"] = "level is required.",
                    ["location_required"] = "location is required.",
                    ["source_required"] = "source is required.",
                    ["yearweek_required"] = "yearweek is required.",
                    ["region_required"] = "region is required.",
                    ["level_allowed"] = "level must be one of: nation, area, region, kabupaten.",
                    ["source_allowed"] = "source must be one of: ookla, open_signal.",
                    ["status_allowed"] = "status must be one of: win, lose.",
                    ["not_found"] = "The requested resource was not found.",
                    ["bad_request"] = "Your request is invalid.",
                    ["invalid_request"] = "Invalid request.",
                    ["unexpected_error"] = "An unexpected error occurred.",
                },
            };

        public LocalizedString this[string name]
        {
            get
            {
                var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                if (
                    LocalizedStrings.TryGetValue(culture, out var languageStrings)
                    && languageStrings.TryGetValue(name, out var localizedString)
                )
                {
                    return new LocalizedString(name, localizedString, false);
                }

                if (
                    LocalizedStrings.TryGetValue("en", out var englishStrings)
                    && englishStrings.TryGetValue(name, out var englishString)
                )
                {
                    return new LocalizedString(name, englishString, true);
                }

                return new LocalizedString(name, name, true);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var localizedString = this[name];
                return new LocalizedString(
                    name,
                    string.Format(localizedString.Value, arguments),
                    localizedString.ResourceNotFound
                );
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            if (LocalizedStrings.TryGetValue(culture, out var languageStrings))
            {
                return languageStrings.Select(kvp => new LocalizedString(
                    kvp.Key,
                    kvp.Value,
                    false
                ));
            }

            if (LocalizedStrings.TryGetValue("en", out var englishStrings))
            {
                return englishStrings.Select(kvp => new LocalizedString(kvp.Key, kvp.Value, true));
            }

            return Enumerable.Empty<LocalizedString>();
        }
    }
}
