using System;
using System.Collections.Generic;

namespace HealthNerd.Services
{
    public interface IAnalytics
    {
        void TrackPage(string screenName);
        void LogEvent(string eventId, IDictionary<string, string> parameters);
    }

    public static class AnalyticsExtensions
    {
        public static void LogEvent(this IAnalytics target, string eventId)
        {
            target.LogEvent(eventId, (IDictionary<string, string>)null);
        }

        public static void LogEvent(this IAnalytics target, string eventId, string paramName, string value)
        {
            target.LogEvent(eventId, new Dictionary<string, string>
            {
                { paramName, value }
            });
        }

        public static void TrackPage(this IAnalytics target, Type screenType)
        {
            target.TrackPage(screenType.FullName);
        }
    }
}