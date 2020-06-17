using Foundation;
using HealthKit;
using NodaTime;

namespace HealthNerd.iOS.Utility
{
    public static class NodaTimeExtensions
    {
        public static Instant ToInstant(this NSDate target)
        {
            return Instant.FromUnixTimeSeconds((long) target.SecondsSince1970);
        }

        public static Interval GetInterval(this HKQuantitySample target)
        {
            return new Interval(target.StartDate.ToInstant(), target.EndDate.ToInstant());
        }
    }
}