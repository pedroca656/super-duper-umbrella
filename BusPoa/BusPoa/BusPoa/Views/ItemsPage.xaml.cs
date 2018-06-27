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
using BusPoa.Services;

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


        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        public ItemsPage()
        {
            InitializeComponent();

            listaTeste = new List<Localizacao>();

            MyMap.MapType = MapType.Street;

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-30.0277, -51.2287), Distance.FromMiles(1)));
            MyMap.IsVisible = false;
            pickerLinha.IsVisible = true;
            pickerLinhaEspera.IsVisible = false;

            CancellationToken = new CancellationTokenSource();

            BindingContext = viewModel = new ItemsViewModel();

            getlocation();
        }

        async void getlocation()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMiles(1)));

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
                CancellationToken.Cancel();
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
                    await Task.Delay(3000, CancellationToken.Token).ContinueWith(async (arg) =>
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

                            var l = new Localizacao()
                            {
                                //Id = Guid.NewGuid().ToString(),
                                Data = DateTime.Now,
                                Linha = (string)pickerLinha.SelectedItem,
                                Latitude = latitude,
                                Longitude = longitude
                            };

                            MessagingCenter.Send(this, "AddLocalizacao", l);

                            var x = await  ((AzureDataStore)DataStore).AddLocalizacaoAsync(l);

                            //adiciona na lista de testes fingindo que eh o banco
                            listaTeste.Add(l);

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

        async void OnPickerLinhaEsperaSelectedIndexChanged(object sender, EventArgs e)
        {
            //codigo do que fazer quando troca 
            //recebe do banco as informacoes da linha selecionada


            CancellationToken = new CancellationTokenSource();
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    //5000 = 5 segundos, 10000 = 10 segundos
                    CancellationToken.Token.ThrowIfCancellationRequested();
                    await Task.Delay(5000, CancellationToken.Token).ContinueWith(async (arg) =>
                    {

                        if (!CancellationToken.Token.IsCancellationRequested)
                        {
                            CancellationToken.Token.ThrowIfCancellationRequested();

                            //aqui pega as informacoes do banco e coloca no mapa

                            

                            var x = await ((AzureDataStore)DataStore).GetLocalizacaoAsync((string)pickerLinhaEspera.SelectedItem);

                            if (x == null)
                            {
                                var xx = 0;
                            }

                            string longit = string.Format("{0:0.0000000}", x.Longitude);
                            string lat = string.Format("{0:0.0000000}", x.Latitude);

                            Debug.WriteLine("Lendo \nLongitude: " + longit + "\nLatitude: " + lat);


                            MyMap.Pins.Clear();
                            MyMap.Pins.Add(new Pin() { Position = new Position(x.Latitude, x.Longitude), Type = PinType.Place, Label = (string)pickerLinhaEspera.SelectedItem });
                        }
                    });

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("EX 1: " + ex.Message);
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}