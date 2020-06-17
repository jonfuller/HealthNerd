using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HealthKit;
using HealthNerd.iOS.Services;
using NodaTime;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels
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

        public Command QueryHealthCommand => new Command(async () =>
        {
            var workoutType = HKObjectType.GetWorkoutType();
            
            var store = new HKHealthStore();

            var dateRange = new DateInterval(
                start: LocalDate.FromDateTime(DateTime.Today).Minus(Period.FromDays(10)),
                end: LocalDate.FromDateTime(DateTime.Today));

            var steps = await HealthKitQueries.GetSteps(store, dateRange);
            var weight = await HealthKitQueries.GetWeight(store, dateRange);

            steps.IfSome(stepss =>
            {
                foreach (var s in stepss)
                {
                    Console.WriteLine($"{s.Interval.Start} - {s.Value}");
                }
            });

            weight.IfSome(weights =>
            {
                foreach (var w in weights)
                {
                    Console.WriteLine($"{w.Interval.Start} - {w.Value.Pounds} lbs");
                }
            });

        });
    }
}