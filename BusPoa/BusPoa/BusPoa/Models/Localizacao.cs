using System;

namespace BusPoa.Models
{
    public class Localizacao
    {
        public string Id { get; set; }
        // public string Onibus { get; set; }
        public string Linha { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Data { get; set; }
    }
}