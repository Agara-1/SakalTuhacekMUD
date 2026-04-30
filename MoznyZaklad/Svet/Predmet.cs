using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoznyZaklad.Svet
{
    public class Predmet
    {
        public string Nazev { get; }
        public string Popis { get; }

        public Predmet(string nazev, string popis)
        {
            Nazev = nazev;
            Popis = popis;
        }

        public override string ToString()
        {
            return Nazev;
        }
    }
}
