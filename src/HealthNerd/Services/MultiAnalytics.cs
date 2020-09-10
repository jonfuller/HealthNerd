using System;
using System.Collections.Generic;

namespace HealthNerd.Services
{
    public class MultiAnalytics : IAnalytics
    {
        private readonly IAnalytics[] _providers;

        public MultiAnalytics(params IAnalytics[] providers)
        {
            _providers = providers;
        }

        public void TrackPage(string screenName) =>
            EachProvider(p => p.TrackPage(screenName));

        public void LogEvent(string eventId, IDictionary<string, string> parameters) =>
            EachProvider(p => p.LogEvent(eventId, parameters));

        private void EachProvider(Action<IAnalytics> action)
        {
            foreach (var provider in _providers)
                action(provider);
        }
    }
}