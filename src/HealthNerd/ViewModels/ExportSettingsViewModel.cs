using System.Collections.Generic;
using System.Globalization;
using HealthNerd.Services;
using HealthNerd.Utility;
using HealthNerd.Utility.Mvvm;
using Resources;
using UnitsNet.Units;
using Xamarin.Forms;

namespace HealthNerd.ViewModels
{
    public class ExportSettingsViewModel : ViewModelBase
    {
        private readonly ISettingsStore _settings;
        private readonly IFirebaseAnalyticsService _analytics;

        public ExportSettingsViewModel(ISettingsStore settings, IFirebaseAnalyticsService analytics, INavigationService nav)
        {
            _settings = settings;
            _analytics = analytics;

            Dismiss = new Command(() => nav.DismissModal());
            DistanceUnits = new List<PickerOption<LengthUnit>>
            {
                new PickerOption<LengthUnit> {DisplayValue = AppRes.Settings_LengthUnit_Meters, Value = LengthUnit.Meter},
                new PickerOption<LengthUnit> {DisplayValue = AppRes.Settings_LengthUnit_Miles, Value = LengthUnit.Mile},
            };
            MassUnits = new List<PickerOption<MassUnit>>
            {
                new PickerOption<MassUnit> {DisplayValue = AppRes.Settings_MassUnit_Kilograms, Value = MassUnit.Kilogram},
                new PickerOption<MassUnit> {DisplayValue = AppRes.Settings_MassUnit_Pounds, Value = MassUnit.Pound},
            };
            EnergyUnits = new List<PickerOption<EnergyUnit>>
            {
                new PickerOption<EnergyUnit> {DisplayValue = AppRes.Settings_EnergyUnit_Kilocalories, Value = EnergyUnit.Kilocalorie},
                new PickerOption<EnergyUnit> {DisplayValue = AppRes.Settings_EnergyUnit_Calories, Value = EnergyUnit.Calorie},
            };
            DurationUnits = new List<PickerOption<DurationUnit>>
            {
                new PickerOption<DurationUnit> {DisplayValue = AppRes.Settings_DurationUnit_Minutes, Value = DurationUnit.Minute},
                new PickerOption<DurationUnit> {DisplayValue = AppRes.Settings_DurationUnit_Hours, Value = DurationUnit.Hour},
            };
        }

        public Command Dismiss { get; }

        public List<PickerOption<LengthUnit>> DistanceUnits { get; }
        public List<PickerOption<MassUnit>> MassUnits { get; }
        public List<PickerOption<EnergyUnit>> EnergyUnits { get; }
        public List<PickerOption<DurationUnit>> DurationUnits { get; }

        public LengthUnit DistanceUnit
        {
            get
            {
                return _settings.DistanceUnit.Match(
                    Some: unit => unit,
                    None: () => SettingsDefaults.DistanceUnit);
            }
            set
            {
                _settings.SetDistanceUnit(value);
                _analytics.LogEvent(AnalyticsEvents.Settings.For(nameof(DistanceUnit)), AnalyticsEvents.Settings.ParamValue, value.ToString());
                OnPropertyChanged();
            }
        }

        public MassUnit MassUnit
        {
            get
            {
                return _settings.MassUnit.Match(
                    Some: unit => unit,
                    None: () => SettingsDefaults.MassUnit);
            }
            set
            {
                _settings.SetMassUnit(value);
                _analytics.LogEvent(AnalyticsEvents.Settings.For(nameof(MassUnit)), AnalyticsEvents.Settings.ParamValue, value.ToString());
                OnPropertyChanged();
            }
        }

        public EnergyUnit EnergyUnit
        {
            get
            {
                return _settings.EnergyUnit.Match(
                    Some: unit => unit,
                    None: () => SettingsDefaults.EnergyUnit);
            }
            set
            {
                _settings.SetEnergyUnit(value);
                _analytics.LogEvent(AnalyticsEvents.Settings.For(nameof(EnergyUnit)), AnalyticsEvents.Settings.ParamValue, value.ToString());
                OnPropertyChanged();
            }
        }

        public DurationUnit DurationUnit
        {
            get
            {
                return _settings.DurationUnit.Match(
                    Some: unit => unit,
                    None: () => SettingsDefaults.DurationUnit);
            }
            set
            {
                _settings.SetDurationUnit(value);
                _analytics.LogEvent(AnalyticsEvents.Settings.For(nameof(DurationUnit)), AnalyticsEvents.Settings.ParamValue, value.ToString());
                OnPropertyChanged();
            }
        }

        public string NumberMonthlySummaries
        {
            get
            {
                return _settings.NumberOfMonthlySummaries.Match(
                    Some: s => s,
                    None: () => SettingsDefaults.NumberOfMonthlySummaries).ToString(CultureInfo.CurrentUICulture);
            }
            set
            {
                if (int.TryParse(value, out var parsed))
                {
                    _settings.SetNumberOfMonthlySummaries(parsed);
                    _analytics.LogEvent(AnalyticsEvents.Settings.For(nameof(NumberMonthlySummaries)), AnalyticsEvents.Settings.ParamValue, value);
                    OnPropertyChanged();
                }
            }
        }

        public bool OmitEmptySheets
        {
            get
            {
                return _settings.OmitEmptySheets.Match(
                    Some: s => s,
                    None: () => SettingsDefaults.OmitEmptySheets);
            }
            set
            {
                _settings.SetOmitEmptySheets(value);
                _analytics.LogEvent(AnalyticsEvents.Settings.For(nameof(OmitEmptySheets)), AnalyticsEvents.Settings.ParamValue, value.ToString());
                OnPropertyChanged();
            }
        }

        public bool OmitEmptyColumnsOnOverallSummary
        {
            get
            {
                return _settings.OmitEmptyColumnsOnOverallSummary.Match(
                    Some: s => s,
                    None: () => SettingsDefaults.OmitEmptyColumnsOnOverallSummary);
            }
            set
            {
                _settings.SetOmitEmptyColumnsOnOverallSummary(value);
                _analytics.LogEvent(AnalyticsEvents.Settings.For(nameof(OmitEmptyColumnsOnOverallSummary)), AnalyticsEvents.Settings.ParamValue, value.ToString());
                OnPropertyChanged();
            }
        }

        public bool OmitEmptyColumnsOnMonthlySummary
        {
            get
            {
                return _settings.OmitEmptyColumnsOnMonthlySummary.Match(
                    Some: s => s,
                    None: () => SettingsDefaults.OmitEmptyColumnsOnMonthlySummary);
            }
            set
            {
                _settings.SetOmitEmptyColumnsOnMonthlySummary(value);
                _analytics.LogEvent(AnalyticsEvents.Settings.For(nameof(OmitEmptyColumnsOnMonthlySummary)), AnalyticsEvents.Settings.ParamValue, value.ToString());
                OnPropertyChanged();
            }
        }
    }
}