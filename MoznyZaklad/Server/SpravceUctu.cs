using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MoznyZaklad.Server
{
    // Datová přepravka pro uložení do JSONu
    public class DataHraci
    {
        public string Jmeno { get; set; } = string.Empty;
        public string HesloHash { get; set; } = string.Empty;
        public string MistnostId { get; set; } = string.Empty;
    }

    public static class SpravceUctu
    {
        private static readonly string SlozkaUlozeni = "Saves";

        public static void Inicializace()
        {
            // Pokud neexistuje složka "Saves", vytvoříme ji
            if (!Directory.Exists(SlozkaUlozeni))
            {
                Directory.CreateDirectory(SlozkaUlozeni);
            }
        }

        // Tato metoda vezme heslo (např. "kocka") a udělá z něj nesmyslný text (Hash)
        public static string VytvorHashHesla(string heslo)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(heslo));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static DataHraci? NactiUcet(string jmeno)
        {
            string cesta = Path.Combine(SlozkaUlozeni, $"{jmeno.ToLower()}.json");
            if (File.Exists(cesta))
            {
                try
                {
                    string json = File.ReadAllText(cesta);
                    return JsonSerializer.Deserialize<DataHraci>(json);
                }
                catch { return null; }
            }
            return null; // Účet neexistuje
        }

        public static void UlozUcet(string jmeno, string hesloHash, string mistnostId)
        {
            string cesta = Path.Combine(SlozkaUlozeni, $"{jmeno.ToLower()}.json");
            var data = new DataHraci { Jmeno = jmeno, HesloHash = hesloHash, MistnostId = mistnostId };

            // WriteIndented udělá JSON hezky čitelný a odsazený
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(cesta, json);
        }
    }
}