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

        public static int NumberOfMonthlySummaries => Settings.Default.NumberOfMonthlySummaries;

        public static bool OmitEmptyColumnsOnMonthlySummary => Settings.Default.OmitEmptyColumnsOnMonthlySummary;
        public static bool OmitEmptyColumnsOnOverallSummary => Settings.Default.OmitEmptyColumnsOnOverallSummary;
        public static bool OmitEmptySheets => Settings.Default.OmitEmptySheets;

        public static CustomSheetsPlacement CustomSheetsPlacement => Settings.Default.CustomSheetsPlacement;
        public static bool UseConstantNameForMostRecentMonthlySummarySheet => Settings.Default.UseConstantNameForMostRecentMonthlySummarySheet;
        public static bool UseConstantNameForPreviousMonthlySummarySheet => Settings.Default.UseConstantNameForPreviousMonthlySummarySheet;
    }
}