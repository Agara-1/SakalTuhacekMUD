using MoznyZaklad.Svet;
using System.Linq;
using System.Text;

namespace MoznyZaklad.Prikazy
{
    public class InventarPrikaz : IPrikaz
    {
        public string Nazev => "inventar";
        public string Napoveda => "inventar - zobrazi inventar, klice a HP";

        public string Proved(Hrac hrac, string[] argumenty)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"HP: {hrac.Zivoty}");
            sb.AppendLine();

            sb.AppendLine("Inventář:");

            if (hrac.Inventar.Count == 0)
            {
                sb.AppendLine("- prázdný");
            }
            else
            {
                foreach (var item in hrac.Inventar)
                {
                    sb.AppendLine($"- {item.Nazev}");
                }
            }

            sb.AppendLine();

            sb.AppendLine("Klíče:");

            if (hrac.Klice.Count == 0)
            {
                sb.AppendLine("- žádné");
            }
            else
            {
                foreach (var klic in hrac.Klice)
                {
                    sb.AppendLine($"- {klic}");
                }
            }

            return sb.ToString();
        }
    }
}