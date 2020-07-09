using System;
using System.Collections.Generic;
using HealthNerd.iOS.Controls;
using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility;
using HealthNerd.iOS.Utility.Mvvm;
using NodaTime;
using Resources;
using UnitsNet.Units;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsStore _settings;

        public SettingsViewModel(ISettingsStore settings, IAuthorizer authorizer, IClock clock)
        {
            _settings = settings;

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

            AuthorizeHealthCommand = new Command(async () =>
            {
                (await authorizer.RequestAuthorizeAppleHealth()).Match(
                    error =>
                    {
                        // TODO: log to analytics
                    },
                    () =>
                    {
                        _settings.SetHealthKitAuthorized(clock.GetCurrentInstant());
                        OnPropertyChanged(nameof(HealthAuthorizationStatusText));
                    });
            });
        }

        public DateTime EarliestFetchDate
        {
            get
            {
                return _settings.SinceDate.Match(
                    Some: d => d.ToDateTimeUnspecified(),
                    None: () => SettingsDefaults.EarliestFetchDate.ToDateTimeUnspecified());
            }
            set
            {
                _settings.SetSinceDate(LocalDate.FromDateTime(value));
                OnPropertyChanged();
            }
        }

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
                OnPropertyChanged();
            }
        }

        public List<PickerOption<LengthUnit>> DistanceUnits { get; }
        public List<PickerOption<MassUnit>> MassUnits { get; }
        public List<PickerOption<EnergyUnit>> EnergyUnits { get; }
        public List<PickerOption<DurationUnit>> DurationUnits { get; }

        public string HealthAuthorizationStatusText => _settings.IsHealthKitAuthorized
            ? AppRes.Settings_IsAuthorizedButton_True
            : AppRes.Settings_IsAuthorizedButton_False;

        public Command AuthorizeHealthCommand { get; }

    }
}