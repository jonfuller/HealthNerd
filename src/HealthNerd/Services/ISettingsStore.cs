using LanguageExt;
using NodaTime;
using UnitsNet.Units;

namespace HealthNerd.Services
{
    public interface ISettingsStore
    {
        Option<LocalDate> SinceDate { get; }
        bool IsHealthKitAuthorized { get; }
        Option<LengthUnit> DistanceUnit { get; }
        Option<MassUnit> MassUnit { get; }
        Option<EnergyUnit> EnergyUnit { get; }
        Option<DurationUnit> DurationUnit { get; }
        Option<int> NumberOfMonthlySummaries { get; }
        Option<bool> OmitEmptyColumnsOnMonthlySummary { get; }
        Option<bool> OmitEmptyColumnsOnOverallSummary { get; }
        Option<bool> OmitEmptySheets { get; }

        void SetSinceDate(LocalDate date);
        void SetDistanceUnit(LengthUnit unit);
        void SetMassUnit(MassUnit unit);
        void SetEnergyUnit(EnergyUnit unit);
        void SetHealthKitAuthorized(Instant timestamp);
        void SetDurationUnit(DurationUnit unit);
        void SetNumberOfMonthlySummaries(int numSummaries);
        void SetOmitEmptySheets(bool omit);
        void SetOmitEmptyColumnsOnMonthlySummary(bool omit);
        void SetOmitEmptyColumnsOnOverallSummary(bool omit);
    }
}