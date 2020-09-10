using System.Collections.Generic;
using Foundation;
using HealthNerd.Services;
using HealthNerd.Utility;

namespace HealthNerd.iOS.Services
{
    public class FirebaseAnalytics : IAnalytics
    {
        public void TrackPage(string screenName) => LogEvent("screen_view", new Dictionary<string, string>{{ "screen_name", screenName }});

        public void LogEvent(string eventId, IDictionary<string, string> parameters)
        {
#if true
            if (parameters == null)
            {
              Firebase.Analytics.Analytics.LogEvent(eventId, parameters: null);
              return;
            }

            var keys = new List<NSString>();
            var values = new List<NSString>();
            foreach (var item in parameters)
            {
              keys.Add(new NSString(item.Key));
              values.Add(new NSString(item.Value));
            }

            var parametersDictionary = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values.ToArray(), keys.ToArray(), keys.Count);
            Firebase.Analytics.Analytics.LogEvent(eventId, parametersDictionary);
#endif
        }
    }
}