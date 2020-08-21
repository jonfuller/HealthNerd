namespace HealthNerd.ViewModels
{
    public class PickerOption<TValue>
    {
        public string DisplayValue { get; set; }
        public TValue Value { get; set; }
    }
}