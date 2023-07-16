using Plugin.AudioRecorder;
using PM2E2GRUPO6.Models;
using PM2E2GRUPO6.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

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

        }

        private void listSites_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private void btnViewMapa_Clicked(object sender, EventArgs e)
        {

        }

        private void btnViewListen_Clicked(object sender, EventArgs e)
        {

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

      /*  private async void EliminarSitio(Sitios site)
        {
            var Estado = await DisplayAlert("Aviso", $"Quiere eliminar el sitio con descripcion: {site.Descripcion}?", "SI", "NO");

            if (Estado)
            {
                var Resultado = await _connection.GetById(Site.Id.ToString());

                if (Resultado != null)
                {
                    var deleteResult = await _connection.GetById(Resultado);

                    if (deleteResult)
                    {
                        await DisplayAlert("Aviso", "El sitio se eliminó correctamente", "OK");
                        LoadData();
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "No se pudo eliminar el sitio", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Aviso", "El sitio no se encontró", "OK");
                }
            }
        }*/




        private async void SwipeItem_Edit(object sender, EventArgs e)
        {
            await DisplayAlert("Aviso", "Editar", "OK");
            await Navigation.PushModalAsync(new UpdateSitio());
        }

        private async void SwipeItem_Delete(object sender, EventArgs e)
        {
            await DisplayAlert("Aviso", "Eliminar", "OK");
        }
    }
}