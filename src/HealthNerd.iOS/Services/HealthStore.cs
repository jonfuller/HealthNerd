using System.Collections.Generic;
using System.Threading.Tasks;
using HealthKit;
using HealthKitData.Core;
using HealthKitData.iOS;
using HealthNerd.Services;
using HealthNerd.ViewModels;
using NodaTime;

namespace HealthNerd.iOS.Services
{
    public class HealthStore : IHealthStore
    {
        private readonly HKHealthStore _healthStore;

        public HealthStore(HKHealthStore healthStore)
        {
            _healthStore = healthStore;
        }
        public async Task<IEnumerable<Workout>> GetWorkoutsAsync(DateInterval dateRange)
        {
            return await HealthKitQueries.GetWorkouts(_healthStore, dateRange);
        }

        public async Task<IEnumerable<Record>> GetHealthRecordsAsync(DateInterval dateRange)
        {
            return await HealthKitQueries.GetHealthRecords(_healthStore, dateRange);
        }
    }
}