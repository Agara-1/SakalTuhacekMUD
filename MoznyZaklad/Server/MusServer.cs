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
            SpravceUctu.Inicializace();

            herniSvet = new HerniSvet();
            procesor = new PrikazovyProcesor();

            procesor.Zaregistruj(new ProzkoumejPrikaz());
            procesor.Zaregistruj(new JdiPrikaz());
            procesor.Zaregistruj(new VezmiPrikaz());
            procesor.Zaregistruj(new InventarPrikaz());
            procesor.Zaregistruj(new PolozPrikaz());
            procesor.Zaregistruj(new UtokPrikaz());
            procesor.Zaregistruj(new MluvPrikaz());
            procesor.Zaregistruj(new PomocPrikaz(procesor.VsechnyPrikazy));

            myServer = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            myServer.Start();
            isRunning = true;
            _ = ServerLoop();
        }

        
        public void Stop()
        {
            isRunning = false;
            myServer.Stop();
        }

        private async Task ServerLoop()
        {
            while (isRunning)
            {
                TcpClient client = await myServer.AcceptTcpClientAsync();
                _ = Task.Run(() => ClientLoop(client));
            }
        }

        private async Task ClientLoop(TcpClient client)
        {
            Hrac? hrac = null;
            string aktivniHesloHash = "";

            try
            {
                using (client)
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                {
                    await writer.WriteLineAsync("Zadej jmeno:");
                    string jmeno = (await reader.ReadLineAsync()) ?? "";

                    var ucet = SpravceUctu.NactiUcet(jmeno);

                    if (ucet != null)
                    {
                        await writer.WriteLineAsync("Zadej heslo:");
                        string heslo = (await reader.ReadLineAsync()) ?? "";

                        if (SpravceUctu.VytvorHashHesla(heslo) != ucet.HesloHash)
                        {
                            await writer.WriteLineAsync("Spatne heslo");
                            return;
                        }

                        aktivniHesloHash = ucet.HesloHash;
                        hrac = SpravceUctu.VytvorHraceZeSave(ucet, herniSvet);
                    }
                    else
                    {
                        await writer.WriteLineAsync("Novy ucet - heslo:");
                        string heslo = (await reader.ReadLineAsync()) ?? "";

                        aktivniHesloHash = SpravceUctu.VytvorHashHesla(heslo);

                        hrac = new Hrac(herniSvet.StartovniMistnost!)
                        {
                            Jmeno = jmeno
                        };
                    }

                    hrac.Svet = herniSvet;

                    lock (hrac.AktualniMistnost.Hraci)
                        hrac.AktualniMistnost.Hraci.Add(hrac);

                    await writer.WriteLineAsync($"Ahoj {hrac.Jmeno}");
                    await writer.WriteLineAsync(hrac.AktualniMistnost.VratPopisMistnosti(hrac));

                    bool connected = true;

                    while (connected)
                    {
                        await writer.WriteAsync("\n> ");
                        string? input = await reader.ReadLineAsync();

                        if (input == null) break;

                        if (input.ToLower() == "konec")
                            break;

                        string odpoved = procesor.Zpracuj(hrac, input);

                        if (!string.IsNullOrWhiteSpace(odpoved))
                            await writer.WriteLineAsync(odpoved);

                        
                        if (hrac.Vyhral)
                        {
                            await writer.WriteLineAsync("");
                            await writer.WriteLineAsync("GRATULUJI!");
                            await writer.WriteLineAsync("Dokončil jsi hru a odletěl do bezpečí");
                            await writer.WriteLineAsync("");
                            await writer.WriteLineAsync("Stiskni ENTER pro odpojení...");

                            await reader.ReadLineAsync();
                            break;
                        }
                    }
                }
            }
            finally
            {
                if (hrac != null)
                {
                    lock (hrac.AktualniMistnost.Hraci)
                        hrac.AktualniMistnost.Hraci.Remove(hrac);

                    SpravceUctu.UlozUcet(hrac, aktivniHesloHash);
                }
            }
        }
    }
}