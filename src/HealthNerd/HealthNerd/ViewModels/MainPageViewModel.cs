using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HealthNerd.Annotations;
using Xamarin.Forms;

namespace HealthNerd.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IAuthorizer _authorizer;

        public MainPageViewModel(IAuthorizer authorizer)
        {
            _authorizer = authorizer;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Command AuthorizeHealthCommand => new Command(async () =>
        {
            (await _authorizer.RequestAuthorizeAppleHealth()).Match(
                error => Console.WriteLine(error.Message),
                () => App.Current.MainPage.DisplayAlert("yay", "you did it!", "thanks!"));
        });

        public Command QueryHealthCommand => new Command(() =>
        {
            
        });
    }
}