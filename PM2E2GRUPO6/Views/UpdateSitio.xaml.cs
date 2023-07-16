using Plugin.AudioRecorder;
using Plugin.Media;
using Plugin.Media.Abstractions;
using PM2E2GRUPO6.Models;
using PM2E2GRUPO6.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E2GRUPO6.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateSitio : ContentPage
    {
        byte[] Image;
        private AudioPlayer Audio_Player = new AudioPlayer();
        MediaFile FileFoto = null;
        MediaFile FotoCap = null;
        Sitios sitio;

        private AudioRecorderService Audio_RecorderService = new AudioRecorderService()
        {
            StopRecordingOnSilence = false,
            StopRecordingAfterTimeout = false
        };

        private bool play = false;

        public UpdateSitio(Sitios sitio)
        {
            InitializeComponent();
            this.sitio = sitio;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        void LoadData()
        {
            imgFoto.Source = GetImageResourseFromBytes(sitio.Fotografia);
            txtLatitude.Text = sitio.Latitud.ToString();
            txtLongitude.Text = sitio.Longitud.ToString();
            txtDescription.Text = sitio.Descripcion;
        }

        private ImageSource GetImageResourseFromBytes(byte[] bytes)
        {
            ImageSource retSource = null;

            if (bytes != null)
            {
                byte[] imageAsBytes = (byte[])bytes;
                retSource = ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
            }

            return retSource;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var EstadoPerm = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (EstadoPerm == PermissionStatus.Granted)
            {
                FileFoto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    SaveToAlbum = true
                });

                if (FileFoto == null)
                    return;

                imgFoto.Source = ImageSource.FromStream(() => { return FileFoto.GetStream(); });
                Image = File.ReadAllBytes(FileFoto.Path);
            }
            else
            {
                await Permissions.RequestAsync<Permissions.Camera>();
            }
        }

        private async void BtnGrabar(object sender, EventArgs e)
        {
            var Estado = await Permissions.RequestAsync<Permissions.Microphone>();
            var Estado2 = await Permissions.RequestAsync<Permissions.StorageRead>();
            var Estado3 = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if (Estado != PermissionStatus.Granted & Estado2 != PermissionStatus.Granted & Estado3 != PermissionStatus.Granted)
            {
                return;
            }

            if (Audio_RecorderService.IsRecording)
            {
                await Audio_RecorderService.StopRecording();
                Audio_Player.Play(Audio_RecorderService.GetAudioFilePath());
                txtMessage.Text = "No esta grabando";
                txtMessage.TextColor = Color.Green;
                btnGrabar.Text = "Grabar Audio";
                play = true;
            }
            else
            {
                await Audio_RecorderService.StartRecording();
                txtMessage.Text = "Esta grabando";
                txtMessage.TextColor = Color.Green;
                btnGrabar.Text = "Dejar de Grabar";
            }


        }

        private async void ObtenerLatitud_Longitud()
        {
            try
            {
                var Estado = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (Estado == PermissionStatus.Granted)
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
                if (ex.Message.Equals("Los servicios de ubicación no están habilitados en el dispositivo"))
                {
                    await DisplayAlert("Aviso", "Servicio de localizacion no activo", "OK");
                }
                else
                {
                    await DisplayAlert("Aviso", ex.Message, "OK");
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
            if (FotoCap != null)
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

        private void Button_Clicked(object sender, EventArgs e)
        {

        }

        private async void btnGrabar_Clicked(object sender, EventArgs e)
        {
            var Estado = await Permissions.RequestAsync<Permissions.Microphone>();
            var Estado2 = await Permissions.RequestAsync<Permissions.StorageRead>();
            var Estado3 = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if (Estado != PermissionStatus.Granted & Estado2 != PermissionStatus.Granted & Estado3 != PermissionStatus.Granted)
            {
                return;
            }

            if (Audio_RecorderService.IsRecording)
            {
                await Audio_RecorderService.StopRecording();
                Audio_Player.Play(Audio_RecorderService.GetAudioFilePath());
                txtMessage.Text = "No esta grabando";
                txtMessage.TextColor = Color.Green;
                btnGrabar.Text = "Grabar Audio";
                play = true;
            }
            else
            {
                await Audio_RecorderService.StartRecording();
                txtMessage.Text = "Esta grabando";
                txtMessage.TextColor = Color.Green;
                btnGrabar.Text = "Dejar de Grabar";
            }
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            byte[] audio;

            var ActualConex = Connectivity.NetworkAccess;

            if (ActualConex != NetworkAccess.Internet)
            {
                await DisplayAlert("Aviso", "No hay conexion a internet", "OK");
            }

            if (!play)
            {
                await DisplayAlert("Error", "Aun no ha grabado audio", "OK");
            }

            var Length = ConvertirAudioAByteArray().Length;

            if (Length > 1500000)
            {
                await DisplayAlert("Error", "El audio deber ser mas corto", "OK");
            }

            audio = ConvertirAudioAByteArray();

            if (Image == null)
            {
                await DisplayAlert("Error", "Tavia no ha tomado foto", "OK");
            }

            if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await DisplayAlert("Error", "Debel escribir una descripcion del Sitio", "OK");
            }

            if (string.IsNullOrEmpty(txtLatitude.Text) || string.IsNullOrEmpty(txtLongitude.Text))
            {
                await DisplayAlert("Error", "Aun no se ha obtenido la ubicacion", "OK");
                ObtenerLatitud_Longitud();
                return;
            }

            Sitios sitio = new Sitios();
            sitio.Id = this.sitio.Id;
            sitio.Descripcion = txtDescription.Text;
            sitio.Latitud = double.Parse(txtLatitude.Text);
            sitio.Longitud = double.Parse(txtLongitude.Text);
            sitio.Fotografia = Image;
            sitio.AudioFile = audio;

            FirebaseConnection Crud = new FirebaseConnection();

            var IsSaved = Crud.EditarSitio(sitio);
            await DisplayAlert("Aviso", "Sitio Actualizado", "OK");
            imgFoto.Source = "Foto.png";
            txtDescription.Text = "";
            Image = null;
            
            //ObtenerLatitud_Longitud();

            /*   if (await IsSaved)
               {
                   await DisplayAlert("Aviso", "Sitio Actualizado", "OK");
                   imgFoto.Source = "Foto.png";
                   txtDescription.Text = "";
                   Image = null;
                   //ObtenerLatitud_Longitud();
               }
               else
               {
                   await DisplayAlert("Error", "Sitio no actulizado", "OK");
               }*/

        }

        private void btnUpdateLocation_Clicked(object sender, EventArgs e)
        {
            ObtenerLatitud_Longitud();
        }
    }

    

    
}