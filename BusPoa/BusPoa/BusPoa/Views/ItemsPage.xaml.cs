using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BusPoa.Models;
using BusPoa.Views;
using BusPoa.ViewModels;
using Xamarin.Forms.Maps;

namespace BusPoa.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        bool isMapa = false;
        //string lblBtTrocar = "Estou Esperando um Ônibus";

        public ItemsPage()
        {
            InitializeComponent();

            MyMap.MapType = MapType.Street;

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-30.0277, -51.2287), Distance.FromMiles(1)));
            MyMap.IsVisible = false;
            pickerLinha.IsVisible = true;

            BindingContext = viewModel = new ItemsViewModel();
        }

        //async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        //{
        //    var item = args.SelectedItem as Item;
        //    if (item == null)
        //        return;

        //    await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

        //    // Manually deselect item.
        //    ItemsListView.SelectedItem = null;
        //}

        void btTrocarItem_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
            if (isMapa)
            {
                MyMap.IsVisible = true;
                pickerLinha.IsVisible = false;
                viewModel.lblBtTrocar = "Tô no Bus";
                isMapa = false;
            }
            else
            {
                MyMap.IsVisible = false;
                pickerLinha.IsVisible = true;
                viewModel.lblBtTrocar = "Perando Bus";
                isMapa = true;
            }

        }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            //codigo do que fazer quando troca 
            //envia para o banco as informações do gps

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}