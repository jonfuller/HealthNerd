using HealthNerd.Services;

namespace HealthNerd.Utility
{
    public static class ActionPresenterExtensions
    {
        public static ActionSheetBuilder ActionSheet(this IActionPresenter target)
        {
            return new ActionSheetBuilder(target);
        }
    }
}