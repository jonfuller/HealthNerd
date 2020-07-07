using LanguageExt;
using NodaTime;

namespace HealthNerd.iOS.Services
{
    public interface ISettingsStore
    {
        Option<LocalDate> SinceDate { get; }
        bool IsHealthKitAuthorized { get; }

        void SetSinceDate(LocalDate date);
        void SetHealthKitAuthorized(Instant timestamp);

    }
}