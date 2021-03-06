﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using BusPoa.Models;
using BusPoa.Views;

namespace BusPoa.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }


        private string _lblBtTrocar = "Perando Bus";
        public string lblBtTrocar { get { return _lblBtTrocar; }
            set { SetProperty<string>(ref _lblBtTrocar, value); } }

        private string _lblPickerLinha = "Selecione Sua Linha";
        public string lblPickerLinha
        {
            get { return _lblPickerLinha; }
            set { SetProperty<string>(ref _lblPickerLinha, value); }
        }

        public ItemsViewModel()
        {
            Title = "";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Item;
                Items.Add(_item);
                await DataStore.AddItemAsync(_item);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}