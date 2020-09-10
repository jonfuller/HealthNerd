using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HealthNerd.Services;
using HealthNerd.Utility;
using HealthNerd.Utility.Mvvm;
using HealthNerd.ViewModels;
using LanguageExt;
using Moq;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Testing;
using NUnit.Framework;
using Serilog;
using Xamarin.Essentials;
using static LanguageExt.Prelude;

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
            public class Nominal
            {
                [Test]
                public void QueriesHealth()
                {
                    var mockHealthStore = new Mock<IHealthStore>();

                    var sut = GetSut(healthStore: mockHealthStore.Object);

                    sut.Exporter.Command.Execute(null);

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

                    sut.Exporter.Command.Execute(null);

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

                    sut.Exporter.Command.Execute(null);

                    var fiveMinutesAgo = clock.InTzdbSystemDefaultZone().GetCurrentLocalDateTime().Minus(Period.FromMinutes(5));

                    mockFileManager.Verify(f => f.DeleteExportsBefore(It.Is((LocalDateTime l) =>
                        l == fiveMinutesAgo)));
                }

                [Test]
                public void SharesFile()
                {
                    var mockShare = new Mock<IShare>();

                    var sut = GetSut(
                        share: mockShare.Object);

                    sut.Exporter.Command.Execute(null);

                    mockShare.Verify(s => s.RequestAsync(It.IsAny<ShareFileRequest>()));
                }
            }

            public class HasRecentExport
            {
                [Test]
                public void AsksToUseRecentExport()
                {
                    var mockFileManager = new Mock<IFileManager>();
                    var mockActionPresenter = new Mock<IActionPresenter>();

                    mockFileManager
                       .Setup(x => x.GetLatestExportFile())
                       .Returns(Some((new FileInfo("arbitrary.xlsx"), LocalDateTime.FromDateTime(DateTime.Now), Output.XlsxContentType)));

                    var sut = GetSut(
                        fileManager: mockFileManager.Object,
                        actionPresenter: mockActionPresenter.Object);

                    sut.Exporter.Command.Execute(null);

                    mockActionPresenter.Verify(a => a.DisplayActionSheet(
                        It.IsAny<string>(),
                        It.IsAny<Option<ActionSheetItem>>(),
                        It.IsAny<Option<ActionSheetItem>>(),
                        It.IsAny<IEnumerable<ActionSheetItem>>()));
                }

                [Test]
                public void UseRecentExport_ExportsExisting()
                {
                    var mockHealth = new Mock<IHealthStore>();
                    var mockFileManager = new Mock<IFileManager>();

                    var mockShare = new Mock<IShare>();

                    mockFileManager
                       .Setup(x => x.GetLatestExportFile())
                       .Returns(Some((new FileInfo("arbitrary.xlsx"), LocalDateTime.FromDateTime(DateTime.Now), Output.XlsxContentType)));

                    var sut = GetSut(
                        fileManager: mockFileManager.Object,
                        healthStore: mockHealth.Object,
                        actionPresenter: new ShimActionPresenter(actions =>
                            actions.First().ToTake()), // execute the "use recent export" option,
                        share: mockShare.Object);

                    sut.Exporter.Command.Execute(null);

                    mockShare.Verify(s => s.RequestAsync(It.Is((ShareFileRequest r) =>
                        r.File.FullPath.EndsWith("arbitrary.xlsx"))));
                    mockHealth.Verify(h => h.GetHealthRecordsAsync(It.IsAny<DateInterval>()),
                        Times.Never);
                }

                [Test]
                public void CreateNewExport_ExportsNew()
                {
                    var mockHealth = new Mock<IHealthStore>();
                    var mockFileManager = new Mock<IFileManager>();

                    var mockShare = new Mock<IShare>();

                    mockFileManager
                       .Setup(x => x.GetLatestExportFile())
                       .Returns(Some((new FileInfo("latest.xlsx"), LocalDateTime.FromDateTime(DateTime.Now), Output.XlsxContentType)));
                    mockFileManager
                       .Setup(x => x.GetNewFileName())
                       .Returns(new FileInfo("newFile.xlsx"));

                    var sut = GetSut(
                        fileManager: mockFileManager.Object,
                        healthStore: mockHealth.Object,
                        actionPresenter: new ShimActionPresenter(actions =>
                            actions.Last().ToTake()), // execute the "create new" option,
                        share: mockShare.Object);

                    sut.Exporter.Command.Execute(null);

                    mockShare.Verify(s => s.RequestAsync(It.Is((ShareFileRequest r) =>
                        r.File.FullPath.EndsWith("newFile.xlsx"))));
                    mockHealth.Verify(h => h.GetHealthRecordsAsync(It.IsAny<DateInterval>()));
                }
            }
        }

        static MainPageViewModel GetSut(
            IClock clock = null,
            IShare share = null,
            IAuthorizer authorizer = null,
            INavigationService navService = null,
            IActionPresenter actionPresenter = null,
            ISettingsStore settings = null,
            IFileManager fileManager = null,
            IHealthStore healthStore = null)
        {
            clock ??= SystemClock.Instance;
            var logger = new LoggerConfiguration().CreateLogger();

            var mockAuthorizer = new Mock<IAuthorizer>();
            var mockSettings = new Mock<ISettingsStore>();
            var mockAnalytics = new Mock<IAnalytics>();
            var mockActionPresenter = new Mock<IActionPresenter>();
            var mockNavService = new Mock<INavigationService>();
            var mockFileManager = new Mock<IFileManager>();

            mockFileManager
               .Setup(x => x.GetNewFileName())
               .Returns(new FileInfo("arbitrary-filename-for-testing.xlsx"));

            var authCommand = new AuthorizeHealthCommand(
                authorizer ?? mockAuthorizer.Object,
                clock,
                actionPresenter ?? mockActionPresenter.Object,
                mockAnalytics.Object,
                logger,
                settings ?? mockSettings.Object);

            var exporter = new ExportSpreadsheetCommand(
                fileManager ?? mockFileManager.Object,
                actionPresenter ?? mockActionPresenter.Object,
                settings ?? mockSettings.Object,
                clock,
                mockAnalytics.Object,
                healthStore ?? new Mock<IHealthStore>().Object,
                share ?? new Mock<IShare>().Object,
                new Configuration(),
                logger);

            return new MainPageViewModel(
                authCommand,
                exporter,
                settings ?? mockSettings.Object,
                navService ?? mockNavService.Object,
                mockAnalytics.Object);
        }
    }

    class ShimActionPresenter : IActionPresenter
    {
        private readonly Action<IEnumerable<ActionSheetItem>> _onDisplayActionSheet;

        public ShimActionPresenter(Action<IEnumerable<ActionSheetItem>> onDisplayActionSheet)
        {
            _onDisplayActionSheet = onDisplayActionSheet;
        }

        public void DisplayAlert(string title, string message, string buttonText) { }
        public void DisplayActionSheet(
            string title,
            Option<ActionSheetItem> cancel,
            Option<ActionSheetItem> destroy,
            IEnumerable<ActionSheetItem> actions) => _onDisplayActionSheet(actions);
    }
}
