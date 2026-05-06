using MoznyZaklad.Svet;
using System.Linq;
using System.Text;

namespace MoznyZaklad.Prikazy
{
    public class VezmiPrikaz : IPrikaz
    {
        public string Nazev => "vezmi";
        public string Napoveda => "vezmi <predmet>";

        public string Proved(Hrac hrac, string[] argumenty)
        {
            if (argumenty.Length < 2)
                return "Pouziti: vezmi <predmet>";

            string jmeno = argumenty[1].ToLower();

            var mistnost = hrac.AktualniMistnost;

            var predmet = mistnost.Predmety
                .FirstOrDefault(p => p.Nazev.ToLower() == jmeno);

            if (predmet == null)
                return "Takovy predmet tu neni.";

            mistnost.Predmety.Remove(predmet);

            if (predmet.Id.StartsWith("klic"))
            {
                hrac.Klice.Add(predmet.Id);
            }
            else
            {
                hrac.Inventar.Add(predmet);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Vzal jsi: {predmet.Nazev}");

            if (predmet.Id == "klic3" && mistnost.Id == "dul")
            {
                var mutant = new Npc
                {
                    Id = "mutant_dul",
                    Nazev = "mutant",
                    Zivoty = 20,
                    AktualniZivoty = 20,
                    Utok = 4,
                    Drop = new System.Collections.Generic.List<string>() 
                };

                hrac.AktualniNepritel = mutant;
                hrac.VSouboji = true;

                sb.AppendLine("Sebral jsi klíč... ale ze tmy tě přepadl mutant!");
                sb.AppendLine("1-utok 2-heal 3-utek");
            }

            if (predmet.Id == "core" && mistnost.Id == "laborator")
            {
                var boss = new Npc
                {
                    Id = "boss_final",
                    Nazev = "boss",
                    Zivoty = 25,
                    AktualniZivoty = 25,
                    Utok = 5,
                    Drop = new System.Collections.Generic.List<string>()
                };

                hrac.AktualniNepritel = boss;
                hrac.VSouboji = true;

                sb.AppendLine("Jakmile jsi vzal core, objevil se boss!");
                sb.AppendLine("1-utok 2-heal 3-utek");
            }

            return sb.ToString();
        }
    }
}