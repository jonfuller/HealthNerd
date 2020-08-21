using System.Threading.Tasks;

namespace HealthNerd.Utility.Mvvm
{
    public interface IViewModelLifecycle
    {
        /// <summary>
        /// Called exactly once, before the viewmodel enters the navigation stack
        /// </summary>
        Task BeforeFirstShown();

        /// <summary>
        /// Called exactly once, when the viewmodel leaves the navigation stack
        /// </summary>
        Task AfterDismissed();

        /// <summary>
        /// Called before a viewmodel appears, when navigating either forwards or backwards
        /// </summary>
        Task BeforeAppearing();
        //Task AfterAppearing(); // Called after a viewmodel appears, when navigating either forwards or backwards
        //Task BeforeNavigateAway(); // Called before a viewmodel disappears, when navigating either forwards or backwards
        //Task AfterNavigateAway(); // Called after a viewmodel disappears, when navigating either forwards or backwards
    }
}