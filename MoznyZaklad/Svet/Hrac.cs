using System;
using System.Collections.Generic;

namespace MoznyZaklad.Svet
{
    public class Hrac
    {
        public string Jmeno { get; set; } = "";

        public Mistnost AktualniMistnost { get; set; }

        public int Zivoty { get; set; } = 20;

        public List<Predmet> Inventar { get; set; } = new List<Predmet>();
        public HashSet<string> Klice { get; set; } = new HashSet<string>();

        public bool MaQuest { get; set; }
        public bool QuestSplnen { get; set; }

        public HashSet<string> PorazeneNpc { get; set; } = new HashSet<string>();

        public bool VSouboji { get; set; }
        public Npc? AktualniNepritel { get; set; }

        public bool VDialogu { get; set; }
        public DialogNode? AktualniDialog { get; set; }

        public bool Vyhral { get; set; } = false;

        public HerniSvet? Svet { get; set; }

        public Hrac(Mistnost start)
        {
            AktualniMistnost = start ?? throw new ArgumentNullException(nameof(start));
        }

        public void Reset()
        {
            if (Svet == null) return;

            AktualniMistnost = Svet.StartovniMistnost!;
            Zivoty = 20;

            Inventar.Clear();
            Klice.Clear();

            MaQuest = false;
            QuestSplnen = false;

            PorazeneNpc.Clear();

            VSouboji = false;
            AktualniNepritel = null;

            VDialogu = false;
            AktualniDialog = null;
        }
    }
}