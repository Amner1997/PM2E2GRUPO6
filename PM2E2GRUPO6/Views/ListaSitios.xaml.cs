using Plugin.AudioRecorder;
using PM2E2GRUPO6.Views;
using PM2E2GRUPO6.Models;
using PM2E2GRUPO6.Servicios;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Globalization;

namespace PM2E2GRUPO6.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListaSitios : ContentPage
    {
        FirebaseConnection _connection = new FirebaseConnection();
        public Sitios Site;
        bool editando;

        private readonly AudioPlayer audioPlayer = new AudioPlayer();
        public ListaSitios()
        {
            InitializeComponent();
            LoadData();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
                LoadData();
                editando = false;
                Site = null;
            
        }

        private void EscucharAudio(byte[] bytes)
        {
            var audioDirectory = Path.Combine(FileSystem.AppDataDirectory, "Audio");
            var audioFilePath = Path.Combine(audioDirectory, "temp.wav");

            if (!Directory.Exists(audioDirectory))
                Directory.CreateDirectory(audioDirectory);

            File.WriteAllBytes(audioFilePath, bytes);

            audioPlayer.Play(audioFilePath);
        }


        private void listSites_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Site = (Sitios)e.Item;
        }

        private async void btnViewMapa_Clicked(object sender, EventArgs e)
        {

            try
            {
                await Navigation.PushModalAsync(new VerMapa(Site.Latitud,Site.Longitud,Site.Descripcion, Site));
             
            }
            catch(Exception M)
            {
            
                await DisplayAlert("Advertencia", "Favor seleccione el sitio donde desea ver en el mapa", "Ok");
            }

           
        }

        private void btnViewListen_Clicked(object sender, EventArgs e)
        {
            if(Site == null)
            {
                DisplayAlert("Aviso", "Seleccione un sitio", "OK");
                return;
            }
            EscucharAudio(Site.AudioFile);
        }

        private async void LoadData()
        {
                listSites.ItemsSource = await _connection.GetAllSitios();


                var current = Connectivity.NetworkAccess;

                if (current != NetworkAccess.Internet)
                {
                    await DisplayAlert("Aviso", "No cuenta con acceso a internet", "OK");
                    return;
                }
        }

        private async void SwipeItem_Edit(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new UpdateSitio(Site));
        }

        private async void SwipeItem_Delete(object sender, EventArgs e)
        {
            var resp = await DisplayAlert("Aviso", "Desea eliminar el campo?", "Si", "No");
            if (resp){
                SwipeItem item = sender as SwipeItem;
                var Id = item.CommandParameter.ToString();
                if (Id != null) {
                    await _connection.deleteSite(Id);
                    LoadData();
                    await DisplayAlert("Alerta", "Sitio eliminado correctamente", "Ok");
                    /*bool isDelete = await _connection.DeleteSitios(Id);
                    if (isDelete){
                        await DisplayAlert("Alerta","Sitio eliminado correctamente", "Ok");
                    }else{
                        await DisplayAlert("Alerta","Ha ocurrido un error","Ok");
                    }*/
                } else await DisplayAlert("Error","error","ok");
            }
            else
            {
                await DisplayAlert("Error","Ha ocurrido un error eliminando el sitio","Ok");
            }
        }

        private void btnRefresh_Clicked(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}