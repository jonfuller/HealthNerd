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

        private void EachProvider(Action<IAnalytics> action)
        {
            foreach (var provider in _providers)
                action(provider);
        }
        public void LogEvent(string eventId)
        {
            EachProvider(p => p.LogEvent(eventId));
        }

        public void LogEvent(string eventId, string paramName, string value)
        {
            EachProvider(p => p.LogEvent(eventId, paramName, value));
        }

        public void LogEvent(string eventId, IDictionary<string, string> parameters)
        {
            EachProvider(p => p.LogEvent(eventId, parameters));
        }
    }
}