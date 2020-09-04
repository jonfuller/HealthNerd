using NodaTime;

namespace HealthNerd.Utility
{
    public class Configuration
    {
        public Period LatestEligibleExportPeriod => Period.FromMinutes(5);
    }
}