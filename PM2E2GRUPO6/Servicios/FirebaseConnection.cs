using Firebase.Database;
using Newtonsoft.Json;
using PM2E2GRUPO6.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.Reactive.Disposables;
using Firebase.Database.Query;

namespace PM2E2GRUPO6.Servicios
{
    public class FirebaseConnection
    {
        public static FirebaseClient conexionFirebase = new FirebaseClient("https://pm2examen2doparcial-default-rtdb.firebaseio.com");

        //Guardar sitios
        public async Task<bool> SaveSitios(Sitios sitio)
        {
            var data = await conexionFirebase.Child(nameof(Sitios)).PostAsync(JsonConvert.SerializeObject(sitio));
            if(!string.IsNullOrEmpty(data.Key))
            {
                return true;
            }
            return false;
        }

        public async Task<string> AgregarSitios(Sitios parametros)
        {
            var data = await conexionFirebase
                  .Child("Sitios")
                  .PostAsync(new Sitios()
                  {
                      Id = parametros.Id,
                      Descripcion = parametros.Descripcion,
                      Latitud = parametros.Latitud,
                      Longitud = parametros.Longitud,
                      Fotografia = parametros.Fotografia,
                      AudioFile = parametros.AudioFile    

                  });
            return data.Key;
        }

        //Listar sitios
        public async Task<List<Sitios>> GetAllSitios()
        {
            return (await conexionFirebase.Child(nameof(Sitios)).OnceAsync<Sitios>()).Select(item => new Sitios
            {

                Descripcion = item.Object.Descripcion,
                Latitud = item.Object.Latitud,
                Longitud = item.Object.Longitud,
                Fotografia = item.Object.Fotografia,
                AudioFile = item.Object.AudioFile,
                Id = item.Object.Id

            }).ToList();
        }

        //Obtener ID
        public async Task<Sitios>GetById(string id)
        {
            return (await conexionFirebase.Child(nameof(Sitios) + "/" + id).OnceSingleAsync<Sitios>());

        }

        // Actualizar Sitios
       /* public async Task<bool> UpdateSitios(Sitios sitio)
        {
            await conexionFirebase.Child(nameof(Sitios) + "/" + sitio.Id).PutAsync(JsonConvert.SerializeObject(sitio));
            return true;
        }*/

        public async Task EditarSitio(Sitios parametros)
        {
            var data = (await conexionFirebase
                 .Child("Sitios")
                 .OnceAsync<Sitios>()).Where(a => a.Object.Id == parametros.Id).FirstOrDefault();

            if (data != null)
            {
                data.Object.Descripcion = parametros.Descripcion;
                data.Object.Latitud = parametros.Latitud;
                data.Object.Longitud = parametros.Longitud;
                data.Object.Fotografia = parametros.Fotografia;
                data.Object.AudioFile = parametros.AudioFile;

                await conexionFirebase
                    .Child("Sitios")
                    .Child(data.Key)
                    .PutAsync(data.Object);
            }
        }



        //Eliminar sitio
        public async Task<bool> DeleteSitios(string id){
            await conexionFirebase.Child(nameof(Sitios) + "/" + id).DeleteAsync();
            return true;
        }

    }

    
}
