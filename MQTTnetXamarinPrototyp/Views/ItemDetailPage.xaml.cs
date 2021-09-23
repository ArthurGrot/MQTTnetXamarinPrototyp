using MQTTnetXamarinPrototyp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MQTTnetXamarinPrototyp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}