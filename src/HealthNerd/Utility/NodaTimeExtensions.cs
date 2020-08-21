using LanguageExt;
using NodaTime.Text;

namespace HealthNerd.Utility
{
    public static class NodaTimeExtensions
    {
        public static Option<T> ToOption<T>(this ParseResult<T> parseResult)
        {
            return parseResult.Success
                ? Prelude.Some(parseResult.Value)
                : Prelude.None;
        }
    }
}