using LanguageExt;
using System;
using System.Collections.Generic;

namespace HealthNerd.Services
{
    public interface IActionPresenter
    {
        void DisplayAlert(string title, string message, string buttonText);
        void DisplayActionSheet(string title, Option<ActionSheetItem> cancel, Option<ActionSheetItem> destroy, IEnumerable<ActionSheetItem> actions);
    }
    public class ActionSheetItem
    {
        public string Text { get; set; }
        public Action ToTake { get; set; }
    }
}