using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;
using HealthNerd.ViewModels;
using Moq;
using NodaTime;
using NUnit.Framework;
using Serilog;

namespace HealthNerd.Tests.ViewModels
{
    public class MainPageViewModelTests
    {
        [Test]
        public void AuthorizeHealth_AuthorizesWithOperatingSystem()
        {
            var mockAuthorizer = new Mock<IAuthorizer>();

            var sut = GetSut(authorizer: mockAuthorizer.Object);

            sut.AuthorizeHealthCommand.Execute(null);

            mockAuthorizer.Verify(a => a.RequestAuthorizeAppleHealth());
        }

        [Test]
        public void GoToSettings_NavigatesToSettings()
        {
            var mockNav = new Mock<INavigationService>();

            var sut = GetSut(navService: mockNav.Object);

            sut.GoToSettings.Execute(null);

            mockNav.Verify(a => a.Modal<SettingsViewModel>());
        }

        MainPageViewModel GetSut(
            IAuthorizer authorizer = null,
            INavigationService navService = null)
        {
            var clock = SystemClock.Instance;
            var logger = new LoggerConfiguration().CreateLogger();

            var mockAuthorizer = new Mock<IAuthorizer>();
            var mockSettings = new Mock<ISettingsStore>();
            var mockAnalytics = new Mock<IFirebaseAnalyticsService>();
            var mockActionPresenter = new Mock<IActionPresenter>();
            var mockNavService = new Mock<INavigationService>();

            var authCommand = new AuthorizeHealthCommand(
                authorizer ?? mockAuthorizer.Object,
                clock,
                mockActionPresenter.Object,
                mockAnalytics.Object,
                logger,
                mockSettings.Object);

            return new MainPageViewModel(
                authCommand,
                clock,
                mockSettings.Object,
                navService ?? mockNavService.Object,
                logger,
                mockAnalytics.Object,
                new Mock<IHealthStore>().Object,
                new Mock<IFileManager>().Object,
                mockActionPresenter.Object);
        }
    }
}