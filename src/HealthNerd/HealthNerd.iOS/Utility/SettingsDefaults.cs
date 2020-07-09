using HealthKitData.Core.Excel.Settings;
using NodaTime;
using UnitsNet.Units;

namespace HealthNerd.iOS.Utility
{
    public static class SettingsDefaults
    {
        public static LocalDate EarliestFetchDate => new LocalDate(2020, 1, 1);
        public static LengthUnit DistanceUnit => Settings.Default.DistanceUnit;
        public static MassUnit MassUnit => Settings.Default.WeightUnit;
        public static EnergyUnit EnergyUnit => Settings.Default.EnergyUnit;
        public static DurationUnit DurationUnit => Settings.Default.DurationUnit;
    }
}