using HealthKit;
using HealthKitData.Core;

namespace HealthKitData.iOS
{
    public static class WorkoutParser
    {
        public static Workout ParseWorkout(HKWorkout workout)
        {
            return new Workout();
        }
    }
}