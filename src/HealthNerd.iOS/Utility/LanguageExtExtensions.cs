using LanguageExt;

namespace HealthNerd.iOS.Utility
{
    public static class LanguageExtExtensions
    {
        public static Option<T> Flatten<T>(this Option<Option<T>> target)
        {
            return target.Match(
                Some: x => x.Match(
                    Some: Prelude.Some,
                    None: Prelude.None),
                None: Prelude.None);
        }
    }
}