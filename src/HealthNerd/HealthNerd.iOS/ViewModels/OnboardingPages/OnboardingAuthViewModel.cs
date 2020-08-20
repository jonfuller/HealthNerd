﻿using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility.Mvvm;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels.OnboardingPages
{
    public class OnboardingAuthViewModel : OnboardingViewModelBase
    {
        public OnboardingAuthViewModel(INavigationService nav, AuthorizeHealthCommand authorizer) : base(nav)
        {
            AuthorizeHealthCommand = authorizer.GetCommand();
        }

        public Command AuthorizeHealthCommand { get; }
    }
}