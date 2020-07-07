using HealthNerd.iOS.Utility;
using LanguageExt;
using NodaTime;
using NodaTime.Text;
using Xamarin.Essentials;

namespace HealthNerd.iOS.Services
{
    public class SettingsStore : ISettingsStore
    {
        public Option<LocalDate> SinceDate
        {
            get
            {
                return LanguageExtExtensions.Flatten(ToOpt(Preferences.Get(PreferenceKeys.FetchDataSinceDate, (string)null))
                   .Select(value => LocalDatePattern.Iso.Parse(value).ToOption()));
            }
        }

        public void SetSinceDate(LocalDate date)
        {
            Preferences.Set(PreferenceKeys.FetchDataSinceDate, LocalDatePattern.Iso.Format(date));
        }

        public void SetHealthKitAuthorized(Instant timestamp)
        {
            Preferences.Set(PreferenceKeys.HealthKitAuthorized, InstantPattern.ExtendedIso.Format(timestamp));
        }

        public bool IsHealthKitAuthorized => Preferences.ContainsKey(PreferenceKeys.HealthKitAuthorized);

        static Option<T> ToOpt<T>(T target)
        {
            return target == null ? Prelude.None : Prelude.Some(target);
        }
    }
}