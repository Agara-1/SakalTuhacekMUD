using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoznyZaklad.Svet
{
    public class Hrac
    {
        public string Jmeno { get; set; } = "";
        public Mistnost AktualniMistnost { get; set; }

        public Hrac(Mistnost startovniMistnost)
        {
            AktualniMistnost = startovniMistnost;
        }
    }
}
