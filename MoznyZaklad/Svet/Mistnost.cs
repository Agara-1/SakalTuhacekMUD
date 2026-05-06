using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Linq;

namespace MoznyZaklad.Svet
{
    public class Mistnost
    {
        public string Id { get; set; } = string.Empty;
        public string Nazev { get; set; } = string.Empty;
        public string Popis { get; set; } = string.Empty;

        public Dictionary<string, string> Vychody { get; set; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public List<Predmet> Predmety { get; set; } = new List<Predmet>();
        public List<Npc> Npc { get; set; } = new List<Npc>();

        public bool Zamceno { get; set; } = false;
        public string? Klic { get; set; }
        public bool Odemceno { get; set; } = false;

        public bool Nebezpecna { get; set; } = false;

        public bool JeCilova { get; set; } = false;
        public string? PotrebnyItem { get; set; }

        [JsonIgnore]
        public Dictionary<string, Mistnost> SousedniMistnosti { get; set; } =
            new Dictionary<string, Mistnost>(StringComparer.OrdinalIgnoreCase);

        [JsonIgnore]
        public List<Hrac> Hraci { get; set; } = new List<Hrac>();

        public string VratPopisMistnosti(Hrac hrac)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"[{Nazev}]");
            sb.AppendLine(Popis);

            sb.AppendLine("Východy: " +
                (Vychody.Count > 0 ? string.Join(", ", Vychody.Keys) : "žádné"));

            if (Predmety.Count > 0)
                sb.AppendLine("Předměty: " + string.Join(", ", Predmety));

            
            var viditelneNpc = Npc
                .Where(n => !hrac.PorazeneNpc.Contains(n.Id))
                .ToList();

            if (viditelneNpc.Count > 0)
                sb.AppendLine("Postavy: " + string.Join(", ", viditelneNpc));

            
            var ostatni = Hraci.Where(h => h != hrac).ToList();

            if (ostatni.Count > 0)
                sb.AppendLine("Hráči: " + string.Join(", ", ostatni.Select(h => h.Jmeno)));

            return sb.ToString();
        }
    }
}