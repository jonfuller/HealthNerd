using System.Threading.Tasks;

namespace HealthNerd.Utility.Mvvm
{
    public abstract class ViewModelBase : PropertyChangedBase, IViewModelLifecycle
    {
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
            return Task.CompletedTask;
        }
    }
}