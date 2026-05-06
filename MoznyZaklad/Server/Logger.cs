using System;
using System.IO;

namespace MoznyZaklad.Server
{
    public static class Logger
    {
        private static readonly string souborLogu = "server.log";
        private static readonly object zamek = new object(); 

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
           
            lock (zamek)
            {
                try
                {
                    string cas = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string radek = $"[{cas}] [{typ}] {zprava}";

                    File.AppendAllText(souborLogu, radek + Environment.NewLine);

                    Console.WriteLine(radek);
                }
                catch
                {
                }
            }
        }
    }
}