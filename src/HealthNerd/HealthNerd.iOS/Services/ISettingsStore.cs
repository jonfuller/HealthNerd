using LanguageExt;
using NodaTime;
using UnitsNet.Units;

namespace HealthNerd.iOS.Services
{
    public interface ISettingsStore
    {
        Option<LocalDate> SinceDate { get; }
        bool IsHealthKitAuthorized { get; }
        Option<LengthUnit> DistanceUnit { get; }
        Option<MassUnit> MassUnit { get; }

        void SetSinceDate(LocalDate date);
        void SetDistanceUnit(LengthUnit unit);
        void SetMassUnit(MassUnit unit);
        void SetHealthKitAuthorized(Instant timestamp);
    }
}