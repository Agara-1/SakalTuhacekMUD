using System;

namespace MoznyZaklad.Svet
{
    public class Predmet
    {
        public string Id { get; set; } = "";
        public string Nazev { get; set; } = "";
        public string Popis { get; set; } = "";

        public string Typ { get; set; } = ""; 
        public int Hodnota { get; set; } = 0;

        public override string ToString()
        {
            return Nazev;
        }
    }
}