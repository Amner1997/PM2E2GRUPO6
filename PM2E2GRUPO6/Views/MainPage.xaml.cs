using Plugin.AudioRecorder;
using Plugin.Media;
using Plugin.Media.Abstractions;
using PM2E2GRUPO6.Models;
using PM2E2GRUPO6.Servicios;
using PM2E2GRUPO6.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM2E2GRUPO6
{
    public partial class MainPage : ContentPage
    {
        private AudioPlayer Audio_Player = new AudioPlayer();
        private int sitioIdCounter = 1; // Variable para el contador de ID
        private AudioRecorderService Audio_RecorderService = new AudioRecorderService();
        MediaFile FotoCap = null;
        private bool play = false;
        byte[] Imagen;

        public MainPage()
        {
            InitializeComponent();

        }

        private int GenerarSiguienteId()
        {
            
            int id = sitioIdCounter;
            sitioIdCounter++;
            return id;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GenerarSiguienteId();
            ObtenerLatitud_Longitud();
        }

        private async void BtnAdd(object sender, EventArgs e)
        {
            var ActualConex = Connectivity.NetworkAccess;

            if(ActualConex != NetworkAccess.Internet)
            {
                await DisplayAlert("Aviso", "No hay conexion a internet", "OK");
                return;
            }


            if (Imagen == null)
            {
                await DisplayAlert("Error", "Todavia no ha tomado foto", "OK");
                return;
            }

            if (string.IsNullOrEmpty(txtLatitude.Text) || string.IsNullOrEmpty(txtLongitude.Text))
            {
                await DisplayAlert("Error", "Aun no se ha obtenido la ubicacion", "OK");
                ObtenerLatitud_Longitud();
                return;
            }

            if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await DisplayAlert("Error", "Debe escribir una descripcion del Sitio", "OK");
                return;
            }

            if (!play)
            {
                await DisplayAlert("Error", "Aun no ha grabado audio", "OK");
                return;
            }

            var Length = ConvertirAudioAByteArray().Length;

            if (Length > 1500000)
            {
                await DisplayAlert("Error", "El audio deber ser mas corto", "OK");
                return;
            }

            Sitios sitio = new Sitios();
            sitio.Id = GenerarSiguienteId();
            sitio.Descripcion = txtDescription.Text;
            sitio.Latitud = double.Parse(txtLatitude.Text);
            sitio.Longitud = double.Parse(txtLongitude.Text);
            sitio.Fotografia = Imagen;
            sitio.AudioFile = ConvertirAudioAByteArray();

            FirebaseConnection Crud = new FirebaseConnection();

            var IsSaved = Crud.AgregarSitios(sitio);

            await DisplayAlert("Aviso", "Sitio Agregado", "OK");
            imgFoto.Source = "Foto.png";
            txtDescription.Text = "";
            Imagen = null;

            ObtenerLatitud_Longitud();
        }

        // Codigo para iniciar la grabacion de audio
        private async void BtnGrabar(object sender, EventArgs e)
        {
            var Estado = await Permissions.RequestAsync<Permissions.Microphone>();
            var Estado2 = await Permissions.RequestAsync<Permissions.StorageRead>();
            var Estado3 = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if(Estado != PermissionStatus.Granted & Estado2 != PermissionStatus.Granted & Estado3 != PermissionStatus.Granted)
            {
                return;
            }

            if (Audio_RecorderService.IsRecording)
            {
                await Audio_RecorderService.StopRecording();
                Audio_Player.Play(Audio_RecorderService.GetAudioFilePath());
                txtMessage.Text = "No esta grabando";
                btnGrabar.ImageSource = "record.png";
                txtMessage.TextColor = Color.Black;
                btnGrabar.Text= "Grabar Audio";
                play = true;
            }
            else
            {
                await Audio_RecorderService.StartRecording();
                txtMessage.Text = "Esta grabando";
                btnGrabar.ImageSource = "record2.png";
                txtMessage.TextColor = Color.Red;
                btnGrabar.Text = "Dejar de Grabar";
            }

        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var EstadoPerm = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (EstadoPerm == PermissionStatus.Granted)
            {
                    FotoCap = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                        SaveToAlbum = true
                    });

                    if (FotoCap == null)
                        return;

                    imgFoto.Source = ImageSource.FromStream(() => { return FotoCap.GetStream(); });
                    Imagen = File.ReadAllBytes(FotoCap.Path);
            }
            else
            {
                await Permissions.RequestAsync<Permissions.Camera>();
            }
        }

        private async void Message(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        private async void ObtenerLatitud_Longitud()
        {
            try
            {
                var Estado = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if(Estado == PermissionStatus.Granted)
                {
                    var Localizacion = await Geolocation.GetLocationAsync();
                    txtLatitude.Text = Math.Round(Localizacion.Latitude, 5) + "";
                    txtLongitude.Text = Math.Round(Localizacion.Longitude, 5) + "";
                }
                else
                {
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
            }
            catch (Exception ex)
            {
                if(ex.Message.Equals("Los servicios de ubicación no están habilitados en el dispositivo"))
                {
                    Message("Error", "Servicio de localizacion no encendido");
                }
                else
                {
                    Message("Error", ex.Message);
                }
            }
        }

        public byte[] ReadData(Stream Entrada)
        {
            byte[] Bufer = new byte[16 * 1024];
            using (MemoryStream MemSt = new MemoryStream())
            {
                int Leer;
                while ((Leer = Entrada.Read(Bufer, 0, Bufer.Length)) > 0)
                {
                    MemSt.Write(Bufer, 0, Leer);
                }
                return MemSt.ToArray();
            }
        }

        // convertir el audio a un ByteArray
        private Byte[] ConvertirAudioAByteArray()
        {
            Stream Audio = Audio_RecorderService.GetAudioFileStream();

            Byte[] Bytes = ReadData(Audio);
            return Bytes;
        }

        private Byte[] ConvertirFotoAByteArray()
        {
            if(FotoCap != null)
            {
                using (MemoryStream MemSt = new MemoryStream())
                {
                    Stream stream = FotoCap.GetStream();
                    stream.CopyTo(MemSt);
                    return MemSt.ToArray();
                }
            }
            return null;
        }

        private async void btnList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ListaSitios());
        }
    }
}
