using HealthKitData.Core.Excel.Settings;
using NodaTime;
using UnitsNet.Units;

namespace HealthNerd.Utility
{
    public static class SettingsDefaults
    {
        public static LocalDate EarliestFetchDate => new LocalDate(2020, 1, 1);
        public static LengthUnit DistanceUnit => LengthUnit.Mile;
        public static MassUnit MassUnit => MassUnit.Pound;
        public static EnergyUnit EnergyUnit => EnergyUnit.Calorie;
        public static DurationUnit DurationUnit => DurationUnit.Minute;

        public static int NumberOfMonthlySummaries => 3;

        public static bool OmitEmptyColumnsOnMonthlySummary => false;
        public static bool OmitEmptyColumnsOnOverallSummary => false;
        public static bool OmitEmptySheets => false;

        public static CustomSheetsPlacement CustomSheetsPlacement => CustomSheetsPlacement.AfterSummary;
        public static bool UseConstantNameForMostRecentMonthlySummarySheet => true;
        public static bool UseConstantNameForPreviousMonthlySummarySheet => true;
    }
}