using System;
using HealthNerd.ViewModels.OnboardingPages;
using Xamarin.Forms;

namespace HealthNerd.Utility
{
    public class OnboardingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WelcomeTemplate { get; set; }
        public DataTemplate AuthTemplate { get; set; }
        public DataTemplate WhatTemplate { get; set; }
        public DataTemplate SettingsTemplate { get; set; }
        public DataTemplate FinishTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                OnboardingWelcomeViewModel v => WelcomeTemplate,
                OnboardingAuthViewModel v => AuthTemplate,
                OnboardingWhatViewModel v => WhatTemplate,
                OnboardingSettingsViewModel v => SettingsTemplate,
                OnboardingFinishViewModel v => FinishTemplate,
                _ =>
                    throw new ArgumentException(
                        message: "not a recognized onboarding viewmodel",
                        paramName: nameof(item))
            };
        }
    }
}