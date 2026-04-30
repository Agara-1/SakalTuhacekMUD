using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoznyZaklad.Prikazy
{
    using MoznyZaklad.Svet;
    using System.Text;

    public class JdiPrikaz : IPrikaz
    {
        public string Nazev => "jdi";
        public string Napoveda => "jdi <jmeno_mistnosti> - presune hrace do sousedni mistnosti";

        public string Proved(Hrac hrac, string[] argumenty)
        {
            if (argumenty.Length < 2)
            {
                return "Pouziti: jdi <jmeno_mistnosti>";
            }

            string cilovaMistnost = argumenty[1].ToLower();

            if (!hrac.AktualniMistnost.SousedniMistnosti.ContainsKey(cilovaMistnost))
            {
                return "Do teto mistnosti se odsud neda jit.";
            }

            Mistnost puvodni = hrac.AktualniMistnost;
            Mistnost nova = puvodni.SousedniMistnosti[cilovaMistnost];

            lock (puvodni.Hraci)
            {
                puvodni.Hraci.Remove(hrac);
            }

            lock (nova.Hraci)
            {
                nova.Hraci.Add(hrac);
            }

            hrac.AktualniMistnost = nova;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Presunul ses do mistnosti: {nova.Nazev}");
            sb.AppendLine();
            sb.AppendLine(nova.VratPopisMistnosti(hrac));

            return sb.ToString();
        }
    }
}
