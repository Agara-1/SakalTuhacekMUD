using MoznyZaklad.Svet;
using System.Linq;
using System.Text;

namespace MoznyZaklad.Prikazy
{
    public class UtokPrikaz : IPrikaz
    {
        public string Nazev => "utoc";
        public string Napoveda => "utoc <npc> - zahaji souboj";

        public string Proved(Hrac hrac, string[] argumenty)
        {
            if (argumenty.Length < 2)
                return "Pouziti: utoc <npc>";

            string cil = argumenty[1].ToLower();

            var npc = hrac.AktualniMistnost.Npc
                .FirstOrDefault(n => n.Nazev.ToLower() == cil);

            if (npc == null)
                return "Takova postava tu neni.";

            hrac.VSouboji = true;
            hrac.AktualniNepritel = npc;

            npc.AktualniZivoty = npc.Zivoty;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Zahajil jsi souboj s {npc.Nazev}!");
            sb.AppendLine();

            sb.AppendLine($"Tvoje HP: {hrac.Zivoty}");
            sb.AppendLine($"{npc.Nazev} HP: {npc.AktualniZivoty}");
            sb.AppendLine();

            sb.AppendLine("Co udelas?");
            sb.AppendLine("1 - utok");
            sb.AppendLine("2 - heal");
            sb.AppendLine("3 - utek");

            return sb.ToString();
        }
    }
}