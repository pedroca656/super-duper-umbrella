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
using System.Threading;
using System.Diagnostics;
using Plugin.Geolocator;

namespace BusPoa.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        bool isMapa = false;
        //string lblBtTrocar = "Estou Esperando um Ônibus";
        public static CancellationTokenSource CancellationToken { get; set; }
        List<Localizacao> listaTeste;

        private class Localizacao
        {
            double lat;
            double lon;

            public Localizacao(double la, double lo)
            {
                lat = la;
                lon = lo;
            }
        }

        public ItemsPage()
        {
            InitializeComponent();

            listaTeste = new List<Localizacao>();

            MyMap.MapType = MapType.Street;

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-30.0277, -51.2287), Distance.FromMiles(1)));
            MyMap.IsVisible = false;
            pickerLinha.IsVisible = true;
            pickerLinhaEspera.IsVisible = false;

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
                pickerLinhaEspera.IsVisible = true;
                viewModel.lblBtTrocar = "Tô no Bus";
                isMapa = false;
            }
            else
            {
                MyMap.IsVisible = false;
                pickerLinha.IsVisible = true;
                pickerLinhaEspera.IsVisible = false;
                viewModel.lblBtTrocar = "Perando Bus";
                isMapa = true;
                //cancela a thread de buscar a localizacao
                CancellationToken.Cancel();
            }

        }

        async void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            //codigo do que fazer quando troca 
            //envia para o banco as informacoes do gps

            //token para cancelar a task quando ele trocar para "esperando um onibus"
            CancellationToken = new CancellationTokenSource();
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    //5000 = 5 segundos, 10000 = 10 segundos
                    CancellationToken.Token.ThrowIfCancellationRequested();
                    await Task.Delay(10000, CancellationToken.Token).ContinueWith(async (arg) =>
                    {

                        if (!CancellationToken.Token.IsCancellationRequested)
                        {
                            CancellationToken.Token.ThrowIfCancellationRequested();

                            //aqui pega as informacoes do gps e upa no banco

                            var locator = CrossGeolocator.Current;
                            locator.DesiredAccuracy = 50;

                            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));

                            double longitude = position.Longitude;
                            double latitude = position.Latitude;

                            //adiciona na lista de testes fingindo que eh o banco
                            listaTeste.Add(new Localizacao(latitude, longitude));

                            string longit = string.Format("{0:0.0000000}", longitude);
                            string lat = string.Format("{0:0.0000000}", latitude);

                            Debug.WriteLine("Armazenando \nLongitude: " + longit +"\nLatitude: " + lat);
                        }
                    });

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("EX 1: " + ex.Message);
                }
            }

        }

        void OnPickerLinhaEsperaSelectedIndexChanged(object sender, EventArgs e)
        {
            //codigo do que fazer quando troca 
            //recebe do banco as informacoes da linha selecionada

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}