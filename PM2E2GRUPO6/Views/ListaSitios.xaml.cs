using Plugin.AudioRecorder;
using PM2E2GRUPO6.Views;
using PM2E2GRUPO6.Models;
using PM2E2GRUPO6.Servicios;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
                LoadData();
                editando = false;
                Site = null;
            
        }

        private void listSites_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Site = (Sitios)e.Item;
        }

        private void btnDelete_Clicked(object sender, EventArgs e)
        {

        }

        private async void btnViewMapa_Clicked(object sender, EventArgs e)
        {

            try
            {
                await Navigation.PushModalAsync(new VerMapa(Site.Latitud,Site.Longitud,Site.Descripcion));
             
            }
            catch(Exception M)
            {
            
                await DisplayAlert("Advertencia", "Favor seleccione el sitio donde desea ver en el mapa", "Ok");
            }

           
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
            await Navigation.PushModalAsync(new UpdateSitio(Site));
        }

        private async void SwipeItem_Delete(object sender, EventArgs e)
        {
            var resp = await DisplayAlert("Aviso", "Desea eliminar el campo?", "Si", "No");
            if (resp){
                SwipeItem item = sender as SwipeItem;
                var id = item.CommandParameter.ToString();
                //var id = ((SwipedEventArgs)e).Parameter as string;
                if (id != null) await DisplayAlert("Great", "valor " + id, "Ok");
                else await DisplayAlert("Error","error","ok");
            }
            else
            {
                await DisplayAlert("Error","Ha ocurrido un error eliminando el sitio","Ok");
            }
        }
    }
}