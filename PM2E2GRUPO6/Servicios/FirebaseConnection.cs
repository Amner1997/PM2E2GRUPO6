using Firebase.Database;
using Newtonsoft.Json;
using PM2E2GRUPO6.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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





    }

    
}
