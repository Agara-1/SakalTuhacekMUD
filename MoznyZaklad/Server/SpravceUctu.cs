using MoznyZaklad.Svet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace MoznyZaklad.Server
{
    public class DataHraci
    {
        public string Jmeno { get; set; } = string.Empty;
        public string HesloHash { get; set; } = string.Empty;
        public string MistnostId { get; set; } = string.Empty;

        public int Zivoty { get; set; } = 20;

        public List<Predmet> Inventar { get; set; } = new();
        public List<string> Klice { get; set; } = new();

        public bool MaQuest { get; set; }
        public bool QuestSplnen { get; set; }

        public List<string> PorazeneNpc { get; set; } = new();
    }

    public static class SpravceUctu
    {
        private static readonly string SlozkaUlozeni = "Saves";

        public static void Inicializace()
        {
            if (!Directory.Exists(SlozkaUlozeni))
                Directory.CreateDirectory(SlozkaUlozeni);
        }

        public static string VytvorHashHesla(string heslo)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(heslo));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));

                return builder.ToString();
            }
        }

        
        public static DataHraci? NactiUcet(string jmeno)
        {
            string cesta = Path.Combine(SlozkaUlozeni, $"{jmeno.ToLower()}.json");

            if (!File.Exists(cesta))
                return null;

            try
            {
                string json = File.ReadAllText(cesta);
                return JsonSerializer.Deserialize<DataHraci>(json);
            }
            catch
            {
                return null;
            }
        }

        
        public static void UlozUcet(Hrac hrac, string hesloHash)
        {
            string cesta = Path.Combine(SlozkaUlozeni, $"{hrac.Jmeno.ToLower()}.json");

            var data = new DataHraci
            {
                Jmeno = hrac.Jmeno,
                HesloHash = hesloHash,
                MistnostId = hrac.AktualniMistnost.Id,

                Zivoty = hrac.Zivoty,
                Inventar = hrac.Inventar,
                Klice = hrac.Klice.ToList(),
                MaQuest = hrac.MaQuest,
                QuestSplnen = hrac.QuestSplnen,
                PorazeneNpc = hrac.PorazeneNpc.ToList()
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(cesta, json);
        }

        
        public static Hrac VytvorHraceZeSave(DataHraci data, HerniSvet svet)
        {
            var mistnost = svet.Mistnosti.ContainsKey(data.MistnostId)
                ? svet.Mistnosti[data.MistnostId]
                : svet.StartovniMistnost;

            var hrac = new Hrac(mistnost)
            {
                Jmeno = data.Jmeno,
                Zivoty = data.Zivoty,
                Inventar = data.Inventar ?? new List<Predmet>(),
                Klice = new HashSet<string>(data.Klice ?? new List<string>()),
                MaQuest = data.MaQuest,
                QuestSplnen = data.QuestSplnen,
                PorazeneNpc = new HashSet<string>(data.PorazeneNpc ?? new List<string>())
            };

            return hrac;
        }
    }
}