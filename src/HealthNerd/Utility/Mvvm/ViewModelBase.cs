using System.Threading.Tasks;
using HealthNerd.Services;

namespace HealthNerd.Utility.Mvvm
{
    public abstract class ViewModelBase : PropertyChangedBase, IViewModelLifecycle
    {
        private readonly IAnalytics _analytics;

        protected ViewModelBase(IAnalytics analytics)
        {
            _analytics = analytics;

        }
        public virtual Task BeforeFirstShown()
        {
            return Task.CompletedTask;
        }

        public virtual Task AfterDismissed()
        {
            return Task.CompletedTask;
        }

        public virtual Task BeforeAppearing()
        {
            _analytics.TrackPage(GetType());
            return Task.CompletedTask;
        }
    }
}