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
        public Option<EnergyUnit> EnergyUnit => PreferencesEx.GetString(PreferenceKeys.EnergyUnit).Select(Enum.Parse<EnergyUnit>);
        public Option<DurationUnit> DurationUnit => PreferencesEx.GetString(PreferenceKeys.DurationUnit).Select(Enum.Parse<DurationUnit>);
        public Option<int> NumberOfMonthlySummaries => PreferencesEx.GetInt(PreferenceKeys.NumberOfMonthlySummaries);
        public Option<bool> OmitEmptyColumnsOnMonthlySummary => PreferencesEx.GetBool(PreferenceKeys.OmitEmptyColumnsOnMonthlySummary);
        public Option<bool> OmitEmptyColumnsOnOverallSummary => PreferencesEx.GetBool(PreferenceKeys.OmitEmptyColumnsOnOverallSummary);
        public Option<bool> OmitEmptySheets => PreferencesEx.GetBool(PreferenceKeys.OmitEmptySheets);

        public bool IsHealthKitAuthorized => Preferences.ContainsKey(PreferenceKeys.HealthKitAuthorized);

        public void SetSinceDate(LocalDate date) => PreferencesEx.SetLocalDate(PreferenceKeys.FetchDataSinceDate, date);
        public void SetDistanceUnit(LengthUnit unit) => Preferences.Set(PreferenceKeys.DistanceUnit, unit.ToString());
        public void SetMassUnit(MassUnit unit) => Preferences.Set(PreferenceKeys.MassUnit, unit.ToString());
        public void SetEnergyUnit(EnergyUnit unit) => Preferences.Set(PreferenceKeys.EnergyUnit, unit.ToString());
        public void SetDurationUnit(DurationUnit unit) => Preferences.Set(PreferenceKeys.DurationUnit, unit.ToString());
        public void SetNumberOfMonthlySummaries(int numSummaries) => Preferences.Set(PreferenceKeys.NumberOfMonthlySummaries, numSummaries);
        public void SetOmitEmptySheets(bool omit) => Preferences.Set(PreferenceKeys.OmitEmptySheets, omit);
        public void SetOmitEmptyColumnsOnMonthlySummary(bool omit) => Preferences.Set(PreferenceKeys.OmitEmptyColumnsOnMonthlySummary, omit);
        public void SetOmitEmptyColumnsOnOverallSummary(bool omit) => Preferences.Set(PreferenceKeys.OmitEmptyColumnsOnOverallSummary, omit);

        public void SetHealthKitAuthorized(Instant timestamp) => Preferences.Set(PreferenceKeys.HealthKitAuthorized, InstantPattern.ExtendedIso.Format(timestamp));
    }
}