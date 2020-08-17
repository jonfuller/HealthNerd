using System;
using System.Linq;
using Xamarin.Forms;

namespace HealthNerd.iOS.Utility.Mvvm
{
    public class ViewLocator : IViewLocator
    {
        public Page CreateAndBindPageFor<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
        {
            var pageType = FindPageForViewModel(viewModel.GetType());

            var page = (Page)Activator.CreateInstance(pageType);

            page.BindingContext = viewModel;

            return page;
        }

        private static Type FindPageForViewModel(Type viewModelType)
        {
            var pageTypeName = viewModelType.Name.Replace("ViewModel", string.Empty);
            var pageType = viewModelType.Assembly
               .GetTypes()
               .Where(t => t.Name.Equals(pageTypeName, StringComparison.InvariantCultureIgnoreCase))
               .FirstOrDefault(t => t.IsSubclassOf(typeof(Page)));

            if (pageType == null)
                throw new ArgumentException(pageTypeName + " type does not exist");

            return pageType;
        }
    }
}