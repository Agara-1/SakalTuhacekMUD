using MoznyZaklad.Svet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoznyZaklad.Prikazy
{
    public class PrikazovyProcesor
    {
        private readonly Dictionary<string, IPrikaz> _prikazy = new();
        private Random random = new Random();

        public IEnumerable<IPrikaz> VsechnyPrikazy => _prikazy.Values;

        public void Zaregistruj(IPrikaz prikaz)
        {
            _prikazy[prikaz.Nazev] = prikaz;
        }

        public string Zpracuj(Hrac hrac, string vstup)
        {
            if (string.IsNullOrWhiteSpace(vstup))
                return "";

            if (hrac.VDialogu)
                return ZpracujDialog(hrac, vstup);

            if (hrac.VSouboji)
                return ZpracujSouboj(hrac, vstup);

            var casti = vstup.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var prikaz = casti[0].ToLower();

            if (_prikazy.ContainsKey(prikaz))
                return _prikazy[prikaz].Proved(hrac, casti);

            return "Neznamy prikaz.";
        }

        private string ZpracujSouboj(Hrac hrac, string vstup)
        {
            var npc = hrac.AktualniNepritel;
            if (npc == null) return "Chyba.";

            StringBuilder sb = new StringBuilder();

            switch (vstup)
            {
                case "1":
                    int weaponDmg = hrac.Inventar
                        .Where(p => p.Typ == "weapon")
                        .Sum(p => p.Hodnota);

                    int dmg = 5 + weaponDmg;

                    npc.AktualniZivoty -= dmg;
                    sb.AppendLine($"Zautocil jsi za {dmg} dmg.");

                    if (npc.AktualniZivoty <= 0)
                    {
                        hrac.VSouboji = false;
                        hrac.PorazeneNpc.Add(npc.Id);

                        sb.AppendLine($"{npc.Nazev} byl porazen!");

                        if (npc.Drop != null)
                        {
                            foreach (var drop in npc.Drop)
                            {
                                if (drop.StartsWith("klic"))
                                {
                                    hrac.Klice.Add(drop);
                                    sb.AppendLine($"Ziskal jsi klic: {drop}");
                                }
                                else
                                {
                                    hrac.Inventar.Add(new Predmet
                                    {
                                        Id = drop,
                                        Nazev = drop,
                                        Popis = "Drop"
                                    });

                                    sb.AppendLine($"Ziskal jsi: {drop}");
                                }
                            }
                        }

                        return sb.ToString();
                    }

                    hrac.Zivoty -= npc.Utok;
                    sb.AppendLine($"{npc.Nazev} te zasahl za {npc.Utok}");
                    break;

                case "2":
                    var heal = hrac.Inventar.FirstOrDefault(p => p.Typ == "heal");
                    if (heal == null) return "Nemáš heal.";

                    hrac.Zivoty += heal.Hodnota;
                    hrac.Inventar.Remove(heal);

                    sb.AppendLine($"Pouzil jsi {heal.Nazev}");
                    hrac.Zivoty -= npc.Utok;
                    break;

                case "3":
                    if (random.Next(0, 2) == 0)
                    {
                        hrac.VSouboji = false;
                        return "Utekl jsi!";
                    }
                    else
                    {
                        hrac.Zivoty -= npc.Utok;
                        sb.AppendLine("Utek se nepovedl!");
                    }
                    break;
            }

            if (hrac.Zivoty <= 0)
                return Smrt(hrac);

            sb.AppendLine();
            sb.AppendLine($"HP: {hrac.Zivoty} | {npc.Nazev}: {npc.AktualniZivoty}");
            sb.AppendLine("1-utok 2-heal 3-utek");

            return sb.ToString();
        }

        private string Smrt(Hrac hrac)
        {
            foreach (var item in hrac.Inventar)
                hrac.AktualniMistnost.Predmety.Add(item);

            hrac.Inventar.Clear();
            hrac.Zivoty = 20;
            hrac.VSouboji = false;

            return "💀 Zemrel jsi...\nProbudil jsi se v kajute.";
        }

        private string ZpracujDialog(Hrac hrac, string vstup)
        {
            if (hrac.AktualniDialog == null)
                return "Chyba dialogu.";

            if (!int.TryParse(vstup, out int volba))
                return "Zadej cislo odpovedi.";

            if (volba <= 0 || volba > hrac.AktualniDialog.Options.Count)
                return "Neplatna volba.";

            var vybrana = hrac.AktualniDialog.Options[volba - 1];

            if (vybrana.Akce == "startQuest")
                hrac.MaQuest = true;

            if (vybrana.Next == null || vybrana.Next.Options.Count == 0)
            {
                hrac.VDialogu = false;
                hrac.AktualniDialog = null;

                return vybrana.Next?.Text ?? "Konec dialogu.";
            }

            hrac.AktualniDialog = vybrana.Next;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(vybrana.Next.Text);

            for (int i = 0; i < vybrana.Next.Options.Count; i++)
                sb.AppendLine($"{i + 1} - {vybrana.Next.Options[i].Text}");

            return sb.ToString();
        }
    }
}