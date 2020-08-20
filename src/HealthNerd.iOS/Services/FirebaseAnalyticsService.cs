using System.Collections.Generic;
using Firebase.Analytics;
using Foundation;

namespace HealthNerd.iOS.Services
{
    public class FirebaseAnalyticsService : IFirebaseAnalyticsService
    {
        public void LogEvent(string eventId)
        {
            LogEvent(eventId, (IDictionary<string, string>)null);
        }

        public void LogEvent(string eventId, string paramName, string value)
        {
            LogEvent(eventId, new Dictionary<string, string>
            {
                { paramName, value }
            });
        }

        public void SetUserId(string userId)
        {
#if true
            Analytics.SetUserId(userId);
#endif
        }

        public void LogEvent(string eventId, IDictionary<string, string> parameters)
        {
#if true
            if (parameters == null)
            {
              Analytics.LogEvent(eventId, parameters: null);
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
            Analytics.LogEvent(eventId, parametersDictionary);
#endif
        }
    }
}