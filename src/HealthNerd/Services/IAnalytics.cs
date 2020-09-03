using System.Collections.Generic;

namespace HealthNerd.Services
{
    public interface IAnalytics
    {
        void LogEvent(string eventId);
        void LogEvent(string eventId, string paramName, string value);
        void LogEvent(string eventId, IDictionary<string, string> parameters);
    }
}