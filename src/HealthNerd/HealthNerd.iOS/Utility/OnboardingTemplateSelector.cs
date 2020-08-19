using System;
using HealthNerd.iOS.ViewModels.OnboardingPages;
using Xamarin.Forms;

namespace HealthNerd.iOS.Utility
{
    public class OnboardingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WelcomeTemplate { get; set; }
        public DataTemplate FinishTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                OnboardingWelcomeViewModel v => WelcomeTemplate,
                OnboardingFinishViewModel v => FinishTemplate,
                _ =>
                    throw new ArgumentException(
                        message: "not a recognized onboarding viewmodel",
                        paramName: nameof(item))
            };
        }
    }
}