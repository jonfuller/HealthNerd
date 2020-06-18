using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthKit;
using HealthNerd.iOS.Utility;
using LanguageExt;
using LanguageExt.SomeHelp;
using NodaTime;
using UnitsNet;
using Xamarin.Forms.Platform.iOS;

using static LanguageExt.Prelude;

namespace HealthNerd.iOS.Services
{
    public static class HealthKitQueries
    {
        public static Task<Option<IEnumerable<Intervaled<int>>>> GetSteps(HKHealthStore store, DateInterval dates)
        {
            return RunQuery(store, HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount), dates,
                sample => Intervaled.Create((int)sample.Quantity.GetDoubleValue(HKUnit.Count), sample.GetInterval()));
        }

        public static Task<Option<IEnumerable<Intervaled<Mass>>>> GetWeight(HKHealthStore store, DateInterval dates)
        {
            return RunQuery(store, HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass), dates,
                sample => Intervaled.Create(GetMass(sample), sample.GetInterval()));

            static Mass GetMass(HKQuantitySample sample) =>
                Mass.FromPounds(sample.Quantity.GetDoubleValue(HKUnit.Pound));
        }

        public static Task<Option<IEnumerable<TOut>>> RunQuery<TOut>(HKHealthStore store, HKQuantityType type, DateInterval dates, Func<HKQuantitySample, TOut> mapper)
        {
            var samplePredicate = HKQuery.GetPredicateForSamples(
                dates.Start.ToDateTimeUnspecified().ToNSDate(),
                dates.End.ToDateTimeUnspecified().ToNSDate(),
                HKQueryOptions.None);

            var theResults = new Option<IEnumerable<TOut>>();

            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            var query = new HKSampleQuery(type, samplePredicate, HKSampleQuery.NoLimit, null, (_, results, err) =>
            {
                if (results == null)
                    theResults = None;
                else
                    theResults = results
                       .Cast<HKQuantitySample>()
                       .Select(mapper)
                       .ToSome();

                waitHandle.Set();
            });

            store.ExecuteQuery(query);

            return Task.Run(() =>
            {
                waitHandle.WaitOne(TimeSpan.FromSeconds(30));
                return theResults;
            });
        }
    }
}