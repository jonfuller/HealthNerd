using LanguageExt;
using NodaTime;
using NodaTime.Text;
using Xamarin.Essentials;

namespace HealthNerd.iOS.Utility
{
    public static class PreferencesEx
    {
        public static Option<string> GetString(string key)
        {
            return ToOpt(Preferences.Get(key, (string) null));
        }

        public static Option<LocalDate> GetLocalDate(string key)
        {
            return OptionExtensions.Flatten(GetString(key)
                   .Select(value => LocalDatePattern.Iso.Parse(value).ToOption()));
        }

        public static void SetLocalDate(string key, LocalDate date)
        {
            Preferences.Set(key, LocalDatePattern.Iso.Format(date));
        }

        static Option<T> ToOpt<T>(T target)
        {
            return target == null ? Prelude.None : Prelude.Some(target);
        }
    }
}