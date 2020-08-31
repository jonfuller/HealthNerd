using System;
using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;
using HealthNerd.ViewModels;
using LanguageExt;
using Moq;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Testing;
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

        public class ExportSpreadsheet
        {
            [Test]
            public void QueriesHealth()
            {
                var mockHealthStore = new Mock<IHealthStore>();

                var sut = GetSut(healthStore: mockHealthStore.Object);

                sut.QueryHealthCommand.Execute(null);

                mockHealthStore.Verify(a => a.GetHealthRecordsAsync(It.IsAny<DateInterval>()));
                mockHealthStore.Verify(a => a.GetWorkoutsAsync(It.IsAny<DateInterval>()));
            }

            [Test]
            public void QueriesHealthWithDateRangeFromSettingsSinceDateThroughToday()
            {
                var mockHealthStore = new Mock<IHealthStore>();
                var mockSettings = new Mock<ISettingsStore>();

                mockSettings
                   .Setup(s => s.SinceDate)
                   .Returns(Prelude.Some(new LocalDate(2020, 01, 01)));

                var sut = GetSut(
                    healthStore: mockHealthStore.Object,
                    settings: mockSettings.Object);

                sut.QueryHealthCommand.Execute(null);

                mockHealthStore.Verify(a => a.GetHealthRecordsAsync(It.Is((DateInterval actual) =>
                    actual.Start == new LocalDate(2020, 01, 01) &&
                    actual.End == LocalDate.FromDateTime(DateTime.Today))));
                mockHealthStore.Verify(a => a.GetWorkoutsAsync(It.Is((DateInterval actual) =>
                    actual.Start == new LocalDate(2020, 01, 01) &&
                    actual.End == LocalDate.FromDateTime(DateTime.Today))));
            }

            [Test]
            public void CleansUpOldExports()
            {
                var mockFileManager = new Mock<IFileManager>();
                var clock = FakeClock.FromUtc(2020, 01, 01, 01, 01, 00);

                var sut = GetSut(
                    clock: clock,
                    fileManager: mockFileManager.Object);

                sut.QueryHealthCommand.Execute(null);

                var fiveMinutesAgo = clock.InTzdbSystemDefaultZone().GetCurrentLocalDateTime().Minus(Period.FromMinutes(5));

                mockFileManager.Verify(f => f.DeleteExportsBefore(It.Is((LocalDateTime l) =>
                    l == fiveMinutesAgo)));
            }
        }


        static MainPageViewModel GetSut(
            IClock clock = null,
            IAuthorizer authorizer = null,
            INavigationService navService = null,
            ISettingsStore settings = null,
            IFileManager fileManager = null,
            IHealthStore healthStore = null)
        {
            clock ??= SystemClock.Instance;
            var logger = new LoggerConfiguration().CreateLogger();

            var mockAuthorizer = new Mock<IAuthorizer>();
            var mockSettings = new Mock<ISettingsStore>();
            var mockAnalytics = new Mock<IFirebaseAnalyticsService>();
            var mockActionPresenter = new Mock<IActionPresenter>();
            var mockNavService = new Mock<INavigationService>();
            var mockFileManager = new Mock<IFileManager>();

            var authCommand = new AuthorizeHealthCommand(
                authorizer ?? mockAuthorizer.Object,
                clock,
                mockActionPresenter.Object,
                mockAnalytics.Object,
                logger,
                settings ?? mockSettings.Object);

            return new MainPageViewModel(
                authCommand,
                clock,
                settings ?? mockSettings.Object,
                navService ?? mockNavService.Object,
                logger,
                mockAnalytics.Object,
                healthStore ?? new Mock<IHealthStore>().Object,
                fileManager ?? mockFileManager.Object,
                mockActionPresenter.Object);
        }
    }
}