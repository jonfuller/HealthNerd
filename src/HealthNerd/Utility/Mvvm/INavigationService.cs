﻿using System.Threading.Tasks;

namespace HealthNerd.Utility.Mvvm
{
    public interface INavigationService
    {
        /// <summary>
        /// Sets the viewmodel to be the main page of the application
        /// </summary>
        void PresentAsMainPage(ViewModelBase viewModel);

        /// <summary>
        /// Sets the viewmodel to be the main page of the application
        /// </summary>
        void PresentAsMainPage<TViewModel>(params (string name, object arg)[] resolveOverrides) where TViewModel : ViewModelBase;

        /// <summary>
        /// Sets the viewmodel as the main page of the application, and wraps its page within a Navigation page
        /// </summary>
        void PresentAsNavigatableMainPage(ViewModelBase viewModel);

        /// <summary>
        /// Sets the viewmodel as the main page of the application, and wraps its page within a Navigation page
        /// </summary>
        void PresentAsNavigatableMainPage<TViewModel>(params (string name, object arg)[] resolveOverrides) where TViewModel : ViewModelBase;

        /// <summary>
        /// Dismisses the active modal.
        /// </summary>
        Task DismissModal();

        /// <summary>
        /// Presents the associated page as a Modal
        /// </summary>
        Task Modal(ViewModelBase viewModel);

        /// <summary>
        /// Presents the associated page as a Modal
        /// </summary>
        Task Modal<TViewModel>(params (string name, object arg)[] resolveOverrides) where TViewModel : ViewModelBase;

        /// <summary>
        /// Navigate to the given page on top of the current navigation stack
        /// </summary>
        Task NavigateTo(ViewModelBase viewModel);

        /// <summary>
        /// Navigate to the given page on top of the current navigation stack
        /// </summary>
        Task NavigateTo<TViewModel>(params (string name, object arg)[] resolveOverrides) where TViewModel : ViewModelBase;

        /// <summary>
        /// Navigate to the previous item in the navigation stack
        /// </summary>
        Task NavigateBack();

        /// <summary>
        /// Navigate back to the element at the root of the navigation stack
        /// </summary>
        Task NavigateBackToRoot();
    }
}