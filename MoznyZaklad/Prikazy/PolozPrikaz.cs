using MoznyZaklad.Svet;
using System.Linq;

namespace MoznyZaklad.Prikazy
{
    public class PolozPrikaz : IPrikaz
    {
        public string Nazev => "poloz";
        public string Napoveda => "poloz <predmet> - polozi predmet do mistnosti";

        public string Proved(Hrac hrac, string[] argumenty)
        {
            if (argumenty.Length < 2)
                return "Pouziti: poloz <predmet>";

            string nazev = argumenty[1].ToLower();

            var predmet = hrac.Inventar
                .FirstOrDefault(p => p.Nazev.ToLower() == nazev);

            if (predmet == null)
                return "Takovy predmet nemas v inventari.";

            hrac.Inventar.Remove(predmet);
            hrac.AktualniMistnost.Predmety.Add(predmet);

            return $"Polozil jsi: {predmet.Nazev}";
        }
    }
}