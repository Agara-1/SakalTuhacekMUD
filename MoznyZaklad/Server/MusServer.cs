using MoznyZaklad.Prikazy;
using MoznyZaklad.Svet;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MoznyZaklad.Server
{
    public class MusServer
    {
        private readonly TcpListener myServer;
        private bool isRunning;
        private readonly HerniSvet herniSvet;
        private readonly PrikazovyProcesor procesor;

        public MusServer(int port)
        {
            SpravceUctu.Inicializace(); // Zapneme složku Saves

            herniSvet = new HerniSvet();
            procesor = new PrikazovyProcesor();

            procesor.Zaregistruj(new ProzkoumejPrikaz());
            procesor.Zaregistruj(new JdiPrikaz());
            procesor.Zaregistruj(new PomocPrikaz(procesor.VsechnyPrikazy));

            myServer = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            myServer.Start();
            isRunning = true;
            Logger.Info($"MUD server byl spusten a nasloucha...");
            _ = ServerLoop();
        }

        public void Stop()
        {
            isRunning = false;
            myServer.Stop();
            Logger.Info("MUD server byl ukoncen.");
        }

        private async Task ServerLoop()
        {
            while (isRunning)
            {
                try
                {
                    TcpClient client = await myServer.AcceptTcpClientAsync();
                    _ = Task.Run(() => ClientLoop(client));
                }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex)
                {
                    Logger.Chyba($"Chyba serveru: {ex.Message}");
                }
            }
        }

        private async Task ClientLoop(TcpClient client)
        {
            Logger.Info("Nové spojení navázáno (čekám na login).");
            Hrac? hrac = null;
            string aktivniHesloHash = ""; // Sem si dočasně uložíme hash pro uložení při odpojení

            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                {
                    await writer.WriteLineAsync("Vitej v ukazkovem MUDu.");
                    await writer.WriteLineAsync("Zadej sve jmeno:");

                    string? jmeno = await reader.ReadLineAsync();

                    if (string.IsNullOrWhiteSpace(jmeno))
                    {
                        await writer.WriteLineAsync("Neplatne jmeno. Spojeni bude ukonceno.");
                        return;
                    }

                    jmeno = jmeno.Trim();

                    // --- START LOGIN SYSTÉMU ---
                    DataHraci? ucet = SpravceUctu.NactiUcet(jmeno);

                    if (ucet != null)
                    {
                        await writer.WriteLineAsync("Ucet nalezen. Zadej sve heslo:");
                        string? heslo = await reader.ReadLineAsync();

                        if (heslo == null || SpravceUctu.VytvorHashHesla(heslo) != ucet.HesloHash)
                        {
                            await writer.WriteLineAsync("Spatne heslo! Budes odpojen.");
                            return;
                        }
                        aktivniHesloHash = ucet.HesloHash;
                        await writer.WriteLineAsync("Prihlaseni uspesne!");
                    }
                    else
                    {
                        await writer.WriteLineAsync("Jmeno neznam, vytvarim novy ucet. Zadej nove heslo:");
                        string? heslo = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(heslo)) return;

                        aktivniHesloHash = SpravceUctu.VytvorHashHesla(heslo);
                        await writer.WriteLineAsync("Ucet vytvoren a chranen heslem!");
                    }
                    // --- KONEC LOGIN SYSTÉMU ---

                    // --- ZJIŠTĚNÍ POZICE HRÁČE ---
                    Mistnost startMistnost = herniSvet.StartovniMistnost!;

                    // Pokud má účet uloženou místnost a my ji máme v mapě načtenou z JSONu:
                    if (ucet != null && herniSvet.Mistnosti.ContainsKey(ucet.MistnostId))
                    {
                        startMistnost = herniSvet.Mistnosti[ucet.MistnostId];
                    }

                    hrac = new Hrac(startMistnost);
                    hrac.Jmeno = jmeno;

                    lock (hrac.AktualniMistnost.Hraci)
                    {
                        hrac.AktualniMistnost.Hraci.Add(hrac);
                    }

                    Logger.Info($"Prihlasil se hrac: {hrac.Jmeno}");

                    await writer.WriteLineAsync($"\nAhoj, {hrac.Jmeno}!");
                    await writer.WriteLineAsync(hrac.AktualniMistnost.VratPopisMistnosti(hrac));

                    bool clientConnected = true;

                    while (clientConnected)
                    {
                        await writer.WriteAsync("\n> ");
                        string? data = await reader.ReadLineAsync();

                        if (data == null) { clientConnected = false; continue; }

                        data = data.Trim();
                        if (string.IsNullOrWhiteSpace(data)) continue;

                        if (data.ToLower() == "konec")
                        {
                            await writer.WriteLineAsync("Spojeni bylo ukonceno. Pozice ulozena.");
                            clientConnected = false;
                            continue;
                        }

                        string odpoved = procesor.Zpracuj(hrac, data);
                        if (!string.IsNullOrWhiteSpace(odpoved))
                        {
                            await writer.WriteLineAsync(odpoved);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Chyba($"Chyba klienta: {ex.Message}");
            }
            finally
            {
                if (hrac != null)
                {
                    lock (hrac.AktualniMistnost.Hraci)
                    {
                        hrac.AktualniMistnost.Hraci.Remove(hrac);
                    }

                    // --- ZDE PROBÍHÁ ULOŽENÍ STAVU PŘI ODPOJENÍ ---
                    SpravceUctu.UlozUcet(hrac.Jmeno, aktivniHesloHash, hrac.AktualniMistnost.Id);

                    Logger.Info($"Hrac {hrac.Jmeno} se odpojil. Jeho pozice byla ulozena.");
                }
            }
        }
    }
}