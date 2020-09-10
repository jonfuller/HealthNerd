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

        public void TrackPage(string screenName)
        {
            _client.TrackPageView(screenName);
        }

        public void LogEvent(string eventId, IDictionary<string, string> parameters)
        {
            _client.TrackEvent(eventId, parameters);
        }
    }
}