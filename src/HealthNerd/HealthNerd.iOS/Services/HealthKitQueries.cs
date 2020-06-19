using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthKit;
using HealthKitData.Core;
using HealthNerd.iOS.Utility;
using LanguageExt;
using LanguageExt.SomeHelp;
using NodaTime;
using UnitsNet;
using Xamarin.Forms.Platform.iOS;

using static LanguageExt.Prelude;

namespace HealthKitData.iOS
{
    public static class HealthKitQueries
    {
        public static Task<Option<IEnumerable<HKWorkout>>> GetWorkouts(HKHealthStore store, DateInterval dates)
        {
            var workoutType = HKObjectType.GetWorkoutType();

            return RunQuery(store, workoutType, dates, sample => (HKWorkout)sample);
        }

        public static async Task<IEnumerable<Record>> GetHealthRecords(HKHealthStore store, DateInterval dates)
        {
            var steps = RunQuantitySampleQuery(store, HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount), dates, RecordParser.ParseWithUnit(HKUnit.Count));
            var weight = RunQuantitySampleQuery(store, HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass), dates, RecordParser.ParseWithUnit(HKUnit.Pound));
            var bodyFatPct = RunQuantitySampleQuery(store, HKQuantityType.Create(HKQuantityTypeIdentifier.BodyFatPercentage), dates, RecordParser.ParseWithUnit(HKUnit.Percent));

            return (await Task.WhenAll(steps, weight, bodyFatPct))
               .Select(x => x.Match(
                    Some: s => s,
                    None: Enumerable.Empty<Record>()))
               .Flatten();
        }

        public static Task<Option<IEnumerable<Intervaled<int>>>> GetSteps(HKHealthStore store, DateInterval dates)
        {
            return RunQuantitySampleQuery(store, HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount), dates,
                sample => Intervaled.Create((int)sample.Quantity.GetDoubleValue(HKUnit.Count), sample.GetInterval()));
        }

        public static Task<Option<IEnumerable<Intervaled<Mass>>>> GetWeight(HKHealthStore store, DateInterval dates)
        {
            return RunQuantitySampleQuery(store, HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass), dates,
                sample => Intervaled.Create(GetMass(sample), sample.GetInterval()));

            static Mass GetMass(HKQuantitySample sample) =>
                Mass.FromPounds(sample.Quantity.GetDoubleValue(HKUnit.Pound));
        }

        public static Task<Option<IEnumerable<Intervaled<Ratio>>>> GetBodyFatPercentage(HKHealthStore store, DateInterval dates)
        {
            return RunQuantitySampleQuery(store, HKQuantityType.Create(HKQuantityTypeIdentifier.BodyFatPercentage), dates,
                sample => Intervaled.Create(GetBodyFatPercentage(sample), sample.GetInterval()));

            static Ratio GetBodyFatPercentage(HKQuantitySample sample) =>
                Ratio.FromPercent(sample.Quantity.GetDoubleValue(HKUnit.Percent));
        }

        private static Task<Option<IEnumerable<TOut>>> RunQuantitySampleQuery<TOut>(HKHealthStore store, HKQuantityType type, DateInterval dates, Func<HKQuantitySample, TOut> mapper)
        {
            return RunQuery(store, type, dates, sample => mapper((HKQuantitySample)sample));
        }

        private static Task<Option<IEnumerable<TOut>>> RunQuery<TOut>(HKHealthStore store, HKSampleType type, DateInterval dates, Func<HKSample, TOut> mapper)
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