using System.Linq;
using System.Text;
using MoznyZaklad.Svet;

namespace MoznyZaklad.Prikazy
{
    public class JdiPrikaz : IPrikaz
    {
        public string Nazev => "jdi";
        public string Napoveda => "jdi <smer>";

        public string Proved(Hrac hrac, string[] argumenty)
        {
            if (argumenty.Length < 2)
                return "Pouziti: jdi <smer>";

            string smer = argumenty[1].ToLower();

            if (!hrac.AktualniMistnost.SousedniMistnosti.ContainsKey(smer))
                return "Tam se jit neda.";

            var nova = hrac.AktualniMistnost.SousedniMistnosti[smer];

            if (nova.Zamceno && !nova.Odemceno)
            {
                if (nova.Klic != null && !hrac.Klice.Contains(nova.Klic))
                    return "Dvere jsou zamcene.";

                nova.Odemceno = true;
            }

            if (nova.JeCilova)
            {
                bool maCore = hrac.Inventar.Any(i => i.Id == nova.PotrebnyItem);

                if (!maCore)
                    return "Raketa neni pripravena. Chybi ti core!";

                hrac.Reset();
                hrac.Vyhral = true;

                return "Vyhral jsi! Gratuluji!";
            }

            hrac.AktualniMistnost = nova;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Presunul ses do: {nova.Nazev}");
            sb.AppendLine(nova.VratPopisMistnosti(hrac));

            return sb.ToString();
        }
    }
}