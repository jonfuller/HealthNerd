using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace HealthNerd.Services
{
    public class AppInsightsAnalytics : IAnalytics
    {
        private readonly TelemetryClient _client;

        public AppInsightsAnalytics(TelemetryClient client)
        {
            _client = client;
        }

        public void LogEvent(string eventId)
        {
            _client.TrackEvent(eventId);
        }

        public void LogEvent(string eventId, string paramName, string value)
        {
            _client.TrackEvent(eventId, new Dictionary<string, string>{{paramName, value}});
        }

        public void LogEvent(string eventId, IDictionary<string, string> parameters)
        {
            _client.TrackEvent(eventId, parameters);
        }
    }
}