using LanguageExt;
using NodaTime;
using NodaTime.Text;
using Xamarin.Essentials;
using static LanguageExt.Prelude;

namespace HealthNerd.Utility
{
    public static class PreferencesEx
    {
        public static Option<int> GetInt(string key)
        {
            return Preferences.ContainsKey(key)
                ? Some(Preferences.Get(key, 0))
                : None;
        }

        public static Option<bool> GetBool(string key)
        {
            return Preferences.ContainsKey(key)
                ? Some(Preferences.Get(key, false))
                : None;
        }

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
            return target == null
                ? None
                : Some(target);
        }
    }
}