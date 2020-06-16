using System;
using System.ComponentModel;
using Xamarin.Forms;
using MainPageViewModel = HealthNerd.iOS.ViewModels.MainPageViewModel;

namespace HealthNerd.iOS.Pages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;

        private async void On_AuthorizeHealthClicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("title", "message", "accept", "cancel");

            if (answer)
                ViewModel.AuthorizeHealthCommand.Execute(null);
        }
    }
}
