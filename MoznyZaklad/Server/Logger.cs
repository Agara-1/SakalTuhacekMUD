using System;
using System.IO;

namespace MoznyZaklad.Server
{
    public static class Logger
    {
        private static readonly string souborLogu = "server.log";
        private static readonly object zamek = new object(); // Zámek pro bezpečný zápis z více vláken

        public static void Info(string zprava)
        {
            Zapis("INFO", zprava);
        }

        public static void Chyba(string zprava)
        {
            Zapis("CHYBA", zprava);
        }

        private static void Zapis(string typ, string zprava)
        {
            // Lock zajistí, že v jednu chvíli může do souboru zapisovat jen jeden proces
            lock (zamek)
            {
                try
                {
                    string cas = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string radek = $"[{cas}] [{typ}] {zprava}";

                    // Zápis do textového souboru (pokud neexistuje, sám se vytvoří)
                    File.AppendAllText(souborLogu, radek + Environment.NewLine);

                    // Zároveň to pořád vypisujeme do tvé černé konzole, abys to viděl i ty
                    Console.WriteLine(radek);
                }
                catch
                {
                    // Záměrně prázdné. Pokud by selhal zápis na disk (např. plný disk),
                    // nesmí to v žádném případě shodit běžící hru.
                }
            }
        }
    }
}