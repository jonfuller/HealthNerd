using System;
using System.Threading.Tasks;
using Foundation;
using HealthKit;
using LanguageExt;
using LanguageExt.Common;

namespace HealthNerd.iOS.Services
{
    public class Authorizer : IAuthorizer
    {
        public async Task<Option<Error>> RequestAuthorizeAppleHealth()
        {
            var typesToRead = new NSSet(
                HKQuantityType.Create(HKQuantityTypeIdentifier.HeartRate)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.RestingHeartRate)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.WalkingHeartRateAverage)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.HeartRateVariabilitySdnn)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.BasalEnergyBurned)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.BodyFatPercentage)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.AppleStandTime)
                , HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount)
                , HKObjectType.GetWorkoutType()
                , HKCategoryType.Create(HKCategoryTypeIdentifier.AppleStandHour)
                , HKCategoryType.Create(HKCategoryTypeIdentifier.SleepAnalysis)
                , HKCategoryType.Create(HKCategoryTypeIdentifier.LowHeartRateEvent)
                , HKCategoryType.Create(HKCategoryTypeIdentifier.HighHeartRateEvent)
                );

            var result = await new HKHealthStore().RequestAuthorizationToShareAsync(
                typesToShare: new NSSet(),
                typesToRead: typesToRead);

            return result.Item1
                ? Option<Error>.None
                : Option<Error>.Some(CreateError(result.Item2));

            Error CreateError(NSError error)
            {
                var exception = new Exception(error.ToString());
                return Error.New((int) error.Code, error.LocalizedDescription, Option<Exception>.Some(exception));
            }
        }
    }
}