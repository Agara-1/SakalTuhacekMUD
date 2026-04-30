using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MoznyZaklad.Svet
{
    public class Mistnost
    {
        public string Id { get; set; } = string.Empty;
        public string Nazev { get; set; } = string.Empty;
        public string Popis { get; set; } = string.Empty;

        // Tohle se načítá z JSONu
        public Dictionary<string, string> Vychody { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // ZDE JE OPRAVA PRO JdiPrikaz (Tohle jsi tam neměl!)
        [JsonIgnore]
        public Dictionary<string, Mistnost> SousedniMistnosti { get; set; } = new Dictionary<string, Mistnost>(StringComparer.OrdinalIgnoreCase);

        // Tohle se z JSONu nenačítá, odkomentoval jsem ti to
        [JsonIgnore]
        public List<Hrac> Hraci { get; set; } = new List<Hrac>();

        public string VratPopisMistnosti(Hrac hrac)
        {
            string text = $"[{Nazev}]\n{Popis}\n";
            text += "Východy: " + (Vychody.Count > 0 ? string.Join(", ", Vychody.Keys) : "žádné");
            return text;
        }
    }
}