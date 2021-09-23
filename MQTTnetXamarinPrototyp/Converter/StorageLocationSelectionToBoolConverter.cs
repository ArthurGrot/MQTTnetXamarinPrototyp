using MQTTnetXamarinPrototyp.Models;
using MQTTnetXamarinPrototyp.ViewModels;
using MQTTnetXamarinPrototyp.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MQTTnetXamarinPrototyp.Converter
{
    public class StorageLocationSelectionToBoolConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || value.GetType() != typeof(StorageLocationSynchronization) || parameter.GetType() != typeof(StorageLocationSynchronization))
                return false;



            StorageLocationSynchronization storageLocation = (StorageLocationSynchronization)value;
            StorageLocationSynchronization storageLocationParameter = (StorageLocationSynchronization)parameter;


            if (storageLocation.Name == storageLocationParameter.Name)
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
