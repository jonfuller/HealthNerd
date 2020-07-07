using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HealthKit;
using HealthKitData.iOS;
using HealthNerd.iOS.Services;
using NodaTime;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IAuthorizer _authorizer;
        private readonly IAlertPresenter _alertPresenter;
        private readonly IClock _clock;
        private readonly ISettingsStore _settings;

        public MainPageViewModel(IAuthorizer authorizer, IAlertPresenter alertPresenter, IClock clock, ISettingsStore settings)
        {
            _authorizer = authorizer;
            _alertPresenter = alertPresenter;
            _clock = clock;
            _settings = settings;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool NeedsHealthAuthorization => !_settings.IsHealthKitAuthorized;
        public bool NeedsConfiguration => _settings.SinceDate.IsNone;

        public Command GoToDataSettings => new Command(() =>
        {
            _alertPresenter.DisplayAlert("You did it!", "You found the edge of the simulation. Check back later.", "Bye now!");
        });

        public Command AuthorizeHealthCommand => new Command(async () =>
        {
            (await _authorizer.RequestAuthorizeAppleHealth()).Match(
                error =>
                {
                    _alertPresenter.DisplayAlert(
                        Resources.AppRes.MainPage_HealtKitAuthorization_Error_Title,
                        Resources.AppRes.MainPage_HealtKitAuthorization_Error_Message,
                        Resources.AppRes.MainPage_HealtKitAuthorization_Error_Button);
                    // TODO: log to analytics
                    Console.WriteLine(error.Message);
                },
                () =>
                {
                    _settings.SetHealthKitAuthorized(_clock.GetCurrentInstant());
                    OnPropertyChanged(nameof(NeedsHealthAuthorization));

                    _alertPresenter.DisplayAlert(
                        Resources.AppRes.MainPage_HealtKitAuthorization_Success_Title,
                        Resources.AppRes.MainPage_HealtKitAuthorization_Success_Message,
                        Resources.AppRes.MainPage_HealtKitAuthorization_Success_Button);
                });
        });

        public Command QueryHealthCommand => new Command(async () =>
        {
            var store = new HKHealthStore();

            var fetchDate = _settings.SinceDate.Match(
                Some: s => s,
                None: LocalDate.FromDateTime(new DateTime(2020, 01, 01)));

            var dateRange = new DateInterval(
                start: fetchDate,
                end: LocalDate.FromDateTime(DateTime.Today));

            var workouts = await HealthKitQueries.GetWorkouts(store, dateRange);
            var records = await HealthKitQueries.GetHealthRecords(store, dateRange);

            Output.CreateExcelReport(records, workouts).IfSome(async f =>
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