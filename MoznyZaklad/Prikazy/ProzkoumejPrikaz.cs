using MoznyZaklad.Svet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoznyZaklad.Prikazy
{
    public class ProzkoumejPrikaz : IPrikaz
    {
        public string Nazev => "prozkoumej";
        public string Napoveda => "prozkoumej - vypise aktualni mistnost";

        public string Proved(Hrac hrac, string[] argumenty)
        {
            return hrac.AktualniMistnost.VratPopisMistnosti(hrac);
        }
    }
}
