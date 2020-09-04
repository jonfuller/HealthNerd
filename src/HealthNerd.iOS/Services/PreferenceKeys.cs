namespace HealthNerd.iOS.Services
{
    public static class PreferenceKeys
    {
        public const string FetchDataSinceDate = nameof(FetchDataSinceDate);
        public const string HealthKitAuthorized = nameof(HealthKitAuthorized);
        public const string DistanceUnit = nameof(DistanceUnit);
        public const string MassUnit = nameof(MassUnit);
        public const string EnergyUnit = nameof(EnergyUnit);
        public static string DurationUnit = nameof(DurationUnit);
        public static string NumberOfMonthlySummaries = nameof(NumberOfMonthlySummaries);
        public static string OmitEmptyColumnsOnMonthlySummary = nameof(OmitEmptyColumnsOnMonthlySummary);
        public static string OmitEmptyColumnsOnOverallSummary = nameof(OmitEmptyColumnsOnOverallSummary);
        public static string OmitEmptySheets = nameof(OmitEmptySheets);
        public static string CustomSheetsLocation = nameof(CustomSheetsLocation);
    }
}