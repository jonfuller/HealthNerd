namespace HealthNerd.iOS.Services
{
    public interface IAlertPresenter
    {
        void DisplayAlert(string title, string message, string buttonText);
    }
}