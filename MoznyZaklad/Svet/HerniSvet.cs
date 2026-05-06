using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MoznyZaklad.Svet
{
    public class HerniSvet
    {
        public Dictionary<string, Mistnost> Mistnosti { get; private set; } =
            new Dictionary<string, Mistnost>(StringComparer.OrdinalIgnoreCase);

        public Mistnost? StartovniMistnost { get; private set; }

        public HerniSvet()
        {
            NactiMapu("mapa.json");
        }

        private void NactiMapu(string cestaKSouboru)
        {
            try
            {
                if (!File.Exists(cestaKSouboru))
                {
                    Console.WriteLine($"[INFO] Soubor '{cestaKSouboru}' neexistuje.");

                    var nouzova = new Mistnost
                    {
                        Id = "start",
                        Nazev = "Nouzová místnost",
                        Popis = "Čekáme na mapu..."
                    };

                    Mistnosti.Add(nouzova.Id, nouzova);
                    StartovniMistnost = nouzova;
                    return;
                }

                string jsonString = File.ReadAllText(cestaKSouboru);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var nacteneMistnosti = JsonSerializer.Deserialize<List<Mistnost>>(jsonString, options);

                if (nacteneMistnosti == null)
                    throw new Exception("JSON se nepodařilo deserialize.");

                foreach (var m in nacteneMistnosti)
                {
                    Mistnosti[m.Id] = m;
                }

                
                foreach (var m in Mistnosti.Values)
                {
                    foreach (var vychod in m.Vychody)
                    {
                        if (Mistnosti.TryGetValue(vychod.Value, out var cil))
                        {
                            m.SousedniMistnosti[vychod.Key] = cil;
                        }
                    }
                }

                StartovniMistnost = nacteneMistnosti.FirstOrDefault();

                Console.WriteLine($"[SERVER] Mapa načtena! Počet místností: {Mistnosti.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("CHYBA MAPY:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                var nouzova = new Mistnost
                {
                    Id = "error",
                    Nazev = "Chyba mapy",
                    Popis = "Mapa se nepodařila načíst."
                };

                Mistnosti.Clear();
                Mistnosti[nouzova.Id] = nouzova;
                StartovniMistnost = nouzova;
            }
        }
    }
}