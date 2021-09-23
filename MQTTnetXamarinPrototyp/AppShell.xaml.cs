using MQTTnetXamarinPrototyp.ViewModels;
using MQTTnetXamarinPrototyp.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MQTTnetXamarinPrototyp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(BinLinkingPage), typeof(BinLinkingPage));
            Routing.RegisterRoute(nameof(BinLinkingSynchronizationPage), typeof(BinLinkingSynchronizationPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
