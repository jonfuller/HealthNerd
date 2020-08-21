using System.Collections;
using System.Collections.Generic;
using HealthNerd.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Picker = Xamarin.Forms.Picker;

namespace HealthNerd.Controls
{
    public class PickerCell<T> : ViewCell
    {
        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create(
                nameof(SelectedValue), typeof(T), typeof(PickerCell<T>), null,
                BindingMode.TwoWay,
                propertyChanged: (sender, oldValue, newValue) =>
                {
                    var pickerCell = (PickerCell<T>)sender;
                    var picker = pickerCell._picker;

                    picker.SelectedIndex = GetIndex(newValue, pickerCell.ItemsSource);

                    static int GetIndex(object newValue, IEnumerable<PickerOption<T>> source)
                    {
                        if (newValue == null) return -1;

                        var newValueT = (T) newValue;

                        return source.IndexOf(opt => opt.Value.Equals(newValueT));
                    }
                });

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource), typeof(IList<PickerOption<T>>), typeof(PickerCell<T>), new List<PickerOption<T>>(),
                BindingMode.TwoWay,
                propertyChanged: (sender, oldValue, newValue) =>
                {
                    var pickerCell = (PickerCell<T>)sender;

                    pickerCell._picker.ItemsSource = (IList)newValue;
                });

        private readonly Picker _picker;
        private readonly Label _label;

        public PickerCell()
        {
            _picker = new Picker
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.EndAndExpand,
            };
            
            _picker.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUpdateMode(UpdateMode.WhenFinished);
            _picker.ItemDisplayBinding = new Binding(nameof(PickerOption<T>.DisplayValue));
            _picker.SelectedIndexChanged += (sender, args) =>
            {
                var picker = (Picker)sender;

                SelectedValue = picker.SelectedIndex == -1
                    ? default(T)
                    : ((PickerOption<T>)picker.ItemsSource[picker.SelectedIndex]).Value;
            };

            _label = new Label
            {
                VerticalOptions = LayoutOptions.Center
            };

            View = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BindingContext = this,
                Padding = new Thickness(16, 0),
                Children =
                {
                    _label,
                    _picker,
                }
            };
        }

        public string Label
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        public string Title
        {
            get => _picker.Title;
            set => _picker.Title = value;
        }

        public T SelectedValue
        {
            get => (T)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public IList<PickerOption<T>> ItemsSource
        {
            get => (IList<PickerOption<T>>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}