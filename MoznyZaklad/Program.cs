using MoznyZaklad.Server;
using System;
using System.IO;
using System.Text.Json;


namespace MoznyZaklad
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 5555; 
            string configFile = "config.json";

            if (File.Exists(configFile))
            {
                try
                {
                    string jsonString = File.ReadAllText(configFile);
                    var config = JsonSerializer.Deserialize<ServerConfig>(jsonString);
                    if (config != null && config.Port > 0)
                    {
                        port = config.Port;
                        Console.WriteLine($"[CONFIG] Úspěšně načten port {port} z config.json");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CHYBA CONFIGU] Nepodařilo se načíst JSON: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("[VAROVÁNÍ] Soubor config.json nenalezen, použije se výchozí port 5555.");
            }

            MusServer server = new MusServer(port);
            server.Start();

            Console.WriteLine("Pro vypnutí serveru stiskni ENTER...");
            Console.ReadLine();

            server.Stop();
        }
    }
}