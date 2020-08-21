using HealthNerd.Services;

namespace HealthNerd.iOS.Services
{
    public class AlertPresenter : IAlertPresenter
    {
        public void DisplayAlert(string title, string message, string buttonText)
        {
            App.Current.MainPage.DisplayAlert(title, message, buttonText);
        }
    }
}