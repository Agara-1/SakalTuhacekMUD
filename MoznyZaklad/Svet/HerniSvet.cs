using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; // TADY JE TA OPRAVA CHYBY CS0103

namespace MoznyZaklad.Svet
{
    public class HerniSvet
    {
        public Dictionary<string, Mistnost> Mistnosti { get; private set; } = new Dictionary<string, Mistnost>(StringComparer.OrdinalIgnoreCase);
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
                    var nouzova = new Mistnost { Id = "start", Nazev = "Nouzová místnost", Popis = "Čekáme na mapu..." };
                    Mistnosti.Add(nouzova.Id, nouzova);
                    StartovniMistnost = nouzova;
                    return;
                }

                string jsonString = File.ReadAllText(cestaKSouboru);
                var nacteneMistnosti = JsonSerializer.Deserialize<List<Mistnost>>(jsonString);

                if (nacteneMistnosti != null)
                {
                    foreach (var m in nacteneMistnosti)
                    {
                        Mistnosti[m.Id] = m;
                    }

                    // Magie pro JdiPrikaz (propojení načtených místností)
                    foreach (var m in Mistnosti.Values)
                    {
                        foreach (var vychod in m.Vychody)
                        {
                            if (Mistnosti.TryGetValue(vychod.Value, out var cilovaMistnost))
                            {
                                m.SousedniMistnosti[vychod.Key] = cilovaMistnost;
                            }
                        }
                    }

                    StartovniMistnost = nacteneMistnosti.FirstOrDefault();
                    Console.WriteLine($"[SERVER] Mapa načtena! Počet místností: {Mistnosti.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHYBA] Nepodařilo se zpracovat mapu: {ex.Message}");
            }
        }
    }
}