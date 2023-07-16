using Plugin.AudioRecorder;
using Plugin.Media;
using Plugin.Media.Abstractions;
using PM2E2GRUPO6.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private AudioPlayer audioPlayer = new AudioPlayer();
        MediaFile FileFoto = null;
        Sitios sitio;

        private AudioRecorderService audioRecorderService = new AudioRecorderService()
        {
            StopRecordingOnSilence = false,
            StopRecordingAfterTimeout = false
        };

        private bool reproducir = false;

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

        

        private void Button_Clicked(object sender, EventArgs e)
        {

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

        private void btnGrabar_Clicked(object sender, EventArgs e)
        {

        }

        private void btnAdd_Clicked(object sender, EventArgs e)
        {

        }

        private void btnUpdateLocation_Clicked(object sender, EventArgs e)
        {

        }
    }
}