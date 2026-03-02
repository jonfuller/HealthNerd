using HealthKitData.Core.Excel.Settings;
using HealthNerd.Wasm.Models;
using UnitsNet.Units;

namespace HealthNerd.Wasm.Services;

/// <summary>
/// Converts a <see cref="WebSettings"/> POCO to the
/// <see cref="Settings"/> object expected by HealthKitData.Core's ExcelReport.
/// Property names and enum values mirror those in src/HealthNerd/Services/Output.cs.
/// </summary>
public static class SettingsMapper
{
    public static Settings ToExcelSettings(WebSettings ws)
    {
        return new Settings
        {
            DistanceUnit = ws.DistanceUnit == "Mile" ? LengthUnit.Mile : LengthUnit.Meter,
            WeightUnit   = ws.MassUnit == "Pound"   ? MassUnit.Pound   : MassUnit.Kilogram,
            EnergyUnit   = ws.EnergyUnit == "Calorie"  ? EnergyUnit.Calorie  : EnergyUnit.Kilocalorie,
            DurationUnit = ws.DurationUnit == "Minute" ? DurationUnit.Minute : DurationUnit.Hour,

            NumberOfMonthlySummaries = ws.NumberOfMonthlySummaries,

            OmitEmptyColumnsOnMonthlySummary = ws.OmitEmptyColumnsOnMonthlySummary,
            OmitEmptyColumnsOnOverallSummary = ws.OmitEmptyColumnsOnOverallSummary,
            OmitEmptySheets                  = ws.OmitEmptySheets,

            CustomSheetsPlacement = ws.CustomSheetsPlacement == "BeforeSummary"
                ? CustomSheetsPlacement.BeforeSummary
                : CustomSheetsPlacement.AfterSummary,

            UseConstantNameForMostRecentMonthlySummarySheet = ws.UseConstantNameForMostRecentMonthlySummarySheet,
            UseConstantNameForPreviousMonthlySummarySheet   = ws.UseConstantNameForPreviousMonthlySummarySheet,
        };
    }
}
