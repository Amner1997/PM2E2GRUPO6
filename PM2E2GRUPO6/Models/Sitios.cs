using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM2E2GRUPO6.Models
{
    public class Sitios
    {
        public int Id { get; set; }

        public String Descripcion { get; set; }

        public double Latitud { get; set; }

        public double Longitud { get; set; }

        public byte[] Fotografia { get; set; }

        public byte[] AudioFile { get; set; }
    }
}
