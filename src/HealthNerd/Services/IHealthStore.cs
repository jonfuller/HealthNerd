using System.Collections.Generic;
using System.Threading.Tasks;
using HealthKitData.Core;
using NodaTime;

namespace HealthNerd.Services
{
    public interface IHealthStore
    {
        Task<IEnumerable<Workout>> GetWorkoutsAsync(DateInterval dateRange);
        Task<IEnumerable<Record>> GetHealthRecordsAsync(DateInterval dateRange);
    }
}