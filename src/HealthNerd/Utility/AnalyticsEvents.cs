namespace HealthNerd.Utility
{
    public static class AnalyticsEvents
    {
        public static class Settings
        {
            public static string For(string settingName) => $"{nameof(Settings)}_{settingName}";
            public static string ParamValue = "value";
        }
        public static class AuthorizeHealth
        {
            public static string Failure = $"{nameof(AuthorizeHealth)}_{nameof(Failure)}";
            public static string Success = $"{nameof(AuthorizeHealth)}_{nameof(Success)}";
        }
        public static class FileExport
        {
            public static string Failure = $"{nameof(FileExport)}_{nameof(Failure)}";
            public static string Success = $"{nameof(FileExport)}_{nameof(Success)}";

            public static string Success_WorkoutCount = "WorkoutCount";
            public static string Success_RecordCount = "RecordCount";
        }

    }
}