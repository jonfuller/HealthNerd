using System;
using System.Collections.Generic;
using HealthNerd.Services;
using LanguageExt;

namespace HealthNerd.Utility
{
    public class ActionSheetBuilder
    {
        private readonly IActionPresenter _presenter;
        private Option<ActionSheetItem> _cancel;
        private Option<ActionSheetItem> _destroy;
        private List<ActionSheetItem> _actions;

        public ActionSheetBuilder(IActionPresenter presenter)
        {
            _presenter = presenter;
            _cancel = Prelude.None;
            _destroy = Prelude.None;
            _actions = new List<ActionSheetItem>();
        }

        public ActionSheetBuilder WithCancel(string text, Action cancelAction)
        {
            _cancel = Prelude.Some(new ActionSheetItem
            {
                Text = text,
                ToTake = cancelAction
            });
            return this;
        }

        public ActionSheetBuilder WithDestroy(string text, Action destroyAction)
        {
            _destroy = Prelude.Some(new ActionSheetItem
            {
                Text = text,
                ToTake = destroyAction
            });
            return this;
        }

        public ActionSheetBuilder With(string text, Action action)
        {
            _actions.Add(new ActionSheetItem
            {
                Text = text,
                ToTake = action
            });
            return this;
        }

        public void Show(string title)
        {
            _presenter.DisplayActionSheet(title, _cancel, _destroy, _actions);
        }
    }
}