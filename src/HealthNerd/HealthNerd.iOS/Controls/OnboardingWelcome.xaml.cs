using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HealthNerd.iOS.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnboardingWelcome : ContentView
    {
        public OnboardingWelcome()
        {
            InitializeComponent();
        }
    }
}