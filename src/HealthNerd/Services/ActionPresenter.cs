using System.Collections.Generic;
using LanguageExt;
using System.Linq;

namespace HealthNerd.Services
{
    public class ActionPresenter : IActionPresenter
    {
        public void DisplayAlert(string title, string message, string buttonText)
        {
            App.Current.MainPage.DisplayAlert(title, message, buttonText);
        }

        public async void DisplayActionSheet(string title, Option<ActionSheetItem> cancel, Option<ActionSheetItem> destroy, IEnumerable<ActionSheetItem> actions)
        {
            var actionList = actions.ToList();

            var cancelText = cancel.MatchUnsafe(_ => _.Text, () => null);
            var destroyText = destroy.MatchUnsafe(_ => _.Text, () => null);

            var action = await App.Current.MainPage.DisplayActionSheet(title, cancelText, destroyText, actionList.Select(a => a.Text).ToArray());

            var selected = actionList
                .Concat(cancel)
                .Concat(destroy)
                .Find(x => x.Text == action);

            selected.IfSome(x => x.ToTake());
        }
    }
}