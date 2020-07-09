using System;
using HealthNerd.iOS.Utility;
using LanguageExt;
using NodaTime;
using NodaTime.Text;
using UnitsNet.Units;
using Xamarin.Essentials;

namespace HealthNerd.iOS.Services
{
    public class SettingsStore : ISettingsStore
    {
        public Option<LocalDate> SinceDate => PreferencesEx.GetLocalDate(PreferenceKeys.FetchDataSinceDate);
        public Option<LengthUnit> DistanceUnit => PreferencesEx.GetString(PreferenceKeys.DistanceUnit).Select(Enum.Parse<LengthUnit>);
        public Option<MassUnit> MassUnit => PreferencesEx.GetString(PreferenceKeys.MassUnit).Select(Enum.Parse<MassUnit>);
        public bool IsHealthKitAuthorized => Preferences.ContainsKey(PreferenceKeys.HealthKitAuthorized);

        public void SetSinceDate(LocalDate date) => PreferencesEx.SetLocalDate(PreferenceKeys.FetchDataSinceDate, date);
        public void SetDistanceUnit(LengthUnit unit) => Preferences.Set(PreferenceKeys.DistanceUnit, unit.ToString());
        public void SetMassUnit(MassUnit unit) => Preferences.Set(PreferenceKeys.MassUnit, unit.ToString());
        public void SetHealthKitAuthorized(Instant timestamp) => Preferences.Set(PreferenceKeys.HealthKitAuthorized, InstantPattern.ExtendedIso.Format(timestamp));

    }
}