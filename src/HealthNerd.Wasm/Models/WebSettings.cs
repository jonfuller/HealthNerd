namespace HealthNerd.Wasm.Models;

public class WebSettings
{
    public string DistanceUnit { get; set; } = "Mile";
    public string MassUnit { get; set; } = "Pound";
    public string EnergyUnit { get; set; } = "Calorie";
    public string DurationUnit { get; set; } = "Minute";
    public int NumberOfMonthlySummaries { get; set; } = 3;
    public DateTime SinceDate { get; set; } = new DateTime(2020, 1, 1);
    public bool OmitEmptyColumnsOnMonthlySummary { get; set; } = false;
    public bool OmitEmptyColumnsOnOverallSummary { get; set; } = false;
    public bool OmitEmptySheets { get; set; } = false;
    public string CustomSheetsPlacement { get; set; } = "AfterSummary";
    public bool UseConstantNameForMostRecentMonthlySummarySheet { get; set; } = true;
    public bool UseConstantNameForPreviousMonthlySummarySheet { get; set; } = true;
}
