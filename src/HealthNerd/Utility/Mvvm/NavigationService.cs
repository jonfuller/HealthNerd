using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HealthNerd.Utility.Mvvm
{
    public class NavigationService : INavigationService
    {
        private readonly IHaveMainPage _presentationRoot;
        private readonly IViewLocator _viewLocator;
        private readonly TinyIoCContainer _container;

        public NavigationService(IHaveMainPage presentationRoot, IViewLocator viewLocator, TinyIoCContainer container)
        {
            _presentationRoot = presentationRoot;
            _viewLocator = viewLocator;
            _container = container;
        }

        public async Task NavigateTo<TViewModel>(params (string name, object arg)[] resolveOverrides) where TViewModel : ViewModelBase
        {
            await NavigateTo(_container.Resolve<TViewModel>(GetOverloads(resolveOverrides)));
        }

        public async Task Modal<TViewModel>(params (string name, object arg)[] resolveOverrides) where TViewModel : ViewModelBase
        {
            await Modal(_container.Resolve<TViewModel>(GetOverloads(resolveOverrides)));
        }

        public async Task DismissModal()
        {
            await Navigator.PopModalAsync(animated: true);
        }

        public async Task Modal(ViewModelBase viewModel)
        {
            var page = _viewLocator.CreateAndBindPageFor(viewModel);

            await viewModel.BeforeAppearing();
            await viewModel.BeforeFirstShown();

            await Navigator.PushModalAsync(page, animated: true);
        }

        public async Task NavigateTo(ViewModelBase viewModel)
        {
            var page = _viewLocator.CreateAndBindPageFor(viewModel);

            await viewModel.BeforeAppearing();
            await viewModel.BeforeFirstShown();

            await Navigator.PushAsync(page, animated: true);
        }

        public async Task NavigateBack()
        {
            var dismissing = Navigator.NavigationStack.Last().BindingContext as ViewModelBase;
            var goingTo = Navigator.NavigationStack[Index.FromEnd(2)].BindingContext as ViewModelBase;

            goingTo?.BeforeAppearing();
            await Navigator.PopAsync(animated: true);

            dismissing?.AfterDismissed();
        }
        public async Task NavigateBackToRoot()
        {
            var toDismiss = Navigator
               .NavigationStack
               .Skip(1)
               .Select(vw => vw.BindingContext)
               .OfType<ViewModelBase>()
               .ToArray();

            var goingTo = Navigator.NavigationStack.First().BindingContext as ViewModelBase;
            goingTo?.BeforeAppearing();
            await Navigator.PopToRootAsync(animated: true);

            foreach (var viewModel in toDismiss)
            {
                viewModel.AfterDismissed().Start(TaskScheduler.Default);
            }
        }

        private INavigation Navigator => _presentationRoot.MainPage.Navigation;

        public void PresentAsMainPage<TViewModel>(params (string name, object arg)[] resolveOverrides) where TViewModel : ViewModelBase
        {
            PresentAsMainPage(_container.Resolve<TViewModel>(GetOverloads(resolveOverrides)));
        }
        public void PresentAsMainPage(ViewModelBase viewModel)
        {
            var page = _viewLocator.CreateAndBindPageFor(viewModel);

            IEnumerable<ViewModelBase> viewModelsToDismiss = FindViewModelsToDismiss(_presentationRoot.MainPage);

            if (_presentationRoot.MainPage is NavigationPage navPage)
            {
                // If we're replacing a navigation page, unsub from events
                navPage.PopRequested -= NavPagePopRequested;
            }

            viewModel.BeforeAppearing();
            viewModel.BeforeFirstShown();

            _presentationRoot.MainPage = page;

            foreach (ViewModelBase toDismiss in viewModelsToDismiss)
            {
                toDismiss.AfterDismissed();
            }
        }

        public void PresentAsNavigatableMainPage<TViewModel>(params (string name, object arg)[] resolveOverrides) where TViewModel : ViewModelBase
        {
            PresentAsNavigatableMainPage(_container.Resolve<TViewModel>(GetOverloads(resolveOverrides)));
        }

        public void PresentAsNavigatableMainPage(ViewModelBase viewModel)
        {
            var page = _viewLocator.CreateAndBindPageFor(viewModel);

            NavigationPage newNavigationPage = new NavigationPage(page);

            IEnumerable<ViewModelBase> viewModelsToDismiss = FindViewModelsToDismiss(_presentationRoot.MainPage);

            if (_presentationRoot.MainPage is NavigationPage navPage)
            {
                navPage.PopRequested -= NavPagePopRequested;
            }

            viewModel.BeforeAppearing();
            viewModel.BeforeFirstShown();

            // Listen for back button presses on the new navigation bar
            newNavigationPage.PopRequested += NavPagePopRequested;
            _presentationRoot.MainPage = newNavigationPage;

            foreach (ViewModelBase toDismiss in viewModelsToDismiss)
            {
                toDismiss.AfterDismissed();
            }
        }

        private IEnumerable<ViewModelBase> FindViewModelsToDismiss(Page dismissingPage)
        {
            var viewmodels = new List<ViewModelBase>();

            if (dismissingPage is NavigationPage)
            {
                viewmodels.AddRange(
                    Navigator
                       .NavigationStack
                       .Select(p => p.BindingContext)
                       .OfType<ViewModelBase>()
                );
            }
            else
            {
                var viewmodel = dismissingPage?.BindingContext as ViewModelBase;
                if (viewmodel != null) viewmodels.Add(viewmodel);
            }

            return viewmodels;
        }

        private void NavPagePopRequested(object sender, NavigationRequestedEventArgs e)
        {
            var goingTo = Navigator.NavigationStack[Index.FromEnd(2)].BindingContext as ViewModelBase;
            goingTo?.BeforeAppearing();

            if (Navigator.NavigationStack.LastOrDefault()?.BindingContext is ViewModelBase poppingPage)
            {
                poppingPage.AfterDismissed();
            }
        }

        private NamedParameterOverloads GetOverloads((string name, object arg)[] args)
        {
            return new NamedParameterOverloads((args ?? new (string name, object arg)[0])
               .ToDictionary(x => x.name, x => x.arg));
        }
    }
}