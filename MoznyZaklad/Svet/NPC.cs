using System;
using System.Collections.Generic;

namespace MoznyZaklad.Svet
{
    public class Npc
    {
        public string Id { get; set; } = "";
        public string Nazev { get; set; } = "";

        public int Zivoty { get; set; }
        public int Utok { get; set; }
        public int AktualniZivoty { get; set; }

        public DialogNode? DialogRoot { get; set; }

        public bool JeQuestNpc { get; set; } = false;
        public List<string> PozadovaneItemy { get; set; } = new List<string>();
        public string? OdmenaItem { get; set; }

        public List<string> Drop { get; set; } = new List<string>();

        public override string ToString()
        {
            return Nazev;
        }
    }

    public class DialogNode
    {
        public string Text { get; set; } = "";
        public List<DialogOption> Options { get; set; } = new List<DialogOption>();
    }

    public class DialogOption
    {
        public string Text { get; set; } = "";
        public DialogNode? Next { get; set; }
        public string? Akce { get; set; }
    }
}