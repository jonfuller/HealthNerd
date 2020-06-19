using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HealthKit;
using HealthNerd.iOS.Services;
using NodaTime;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IAuthorizer _authorizer;

        private bool _hasAuthorizedWithHealth;
        private bool _needsHealthAuthorization;

        public MainPageViewModel(IAuthorizer authorizer)
        {
            _authorizer = authorizer;
            _needsHealthAuthorization = true;
            _hasAuthorizedWithHealth = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool NeedsHealthAuthorization
        {
            get => _needsHealthAuthorization;
            set
            {
                _needsHealthAuthorization = value;
                OnPropertyChanged();
            }
        }

        public bool HasAuthorizedWithHealth
        {
            get => _hasAuthorizedWithHealth;
            set
            {
                _hasAuthorizedWithHealth = value;
                OnPropertyChanged();
            }
        }
        public Command DoDataSelection => new Command(() =>
        {
            App.Current.MainPage.DisplayAlert("You did it!", "You found the edge of the simulation. Check back later.", "Bye now!");
        });

        public Command AuthorizeHealthCommand => new Command(async () =>
        {
            (await _authorizer.RequestAuthorizeAppleHealth()).Match(
                error => Console.WriteLine(error.Message),
                () => App.Current.MainPage.DisplayAlert("yay", "you did it!", "thanks!"));
        });

        public Command QueryHealthCommand => new Command(async () =>
        {
            var store = new HKHealthStore();

            var dateRange = new DateInterval(
                start: LocalDate.FromDateTime(DateTime.Today).Minus(Period.FromDays(10)),
                end: LocalDate.FromDateTime(DateTime.Today));

            var steps = await HealthKitQueries.GetSteps(store, dateRange);
            var weight = await HealthKitQueries.GetWeight(store, dateRange);
            var bodyFatPct = await HealthKitQueries.GetBodyFatPercentage(store, dateRange);
            var workouts = await HealthKitQueries.GetWorkouts(store, dateRange);

            Output.Create(steps).IfSome(async f =>
            {
                await Share.RequestAsync(new ShareFileRequest
                {
                    File = new ShareFile(f.filename.FullName, f.contentType.Name)
                });
            });
        });

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}