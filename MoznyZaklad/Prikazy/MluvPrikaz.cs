using MoznyZaklad.Svet;
using System.Linq;
using System.Text;

namespace MoznyZaklad.Prikazy
{
    public class MluvPrikaz : IPrikaz
    {
        public string Nazev => "mluv";
        public string Napoveda => "mluv <npc>";

        public string Proved(Hrac hrac, string[] argumenty)
        {
            if (argumenty.Length < 2)
                return "Pouziti: mluv <npc>";

            string jmeno = argumenty[1].ToLower();

            var npc = hrac.AktualniMistnost.Npc
                .FirstOrDefault(n => n.Nazev.ToLower() == jmeno);

            if (npc == null)
                return "Takova postava tu neni.";

            if (npc.JeQuestNpc && hrac.MaQuest && !hrac.QuestSplnen)
            {
                bool maVse = npc.PozadovaneItemy.All(req =>
                    hrac.Inventar.Any(i => i.Id == req)
                );

                if (maVse)
                {
                    foreach (var req in npc.PozadovaneItemy)
                    {
                        var item = hrac.Inventar.First(i => i.Id == req);
                        hrac.Inventar.Remove(item);
                    }

                    var reward = new Predmet
                    {
                        Id = npc.OdmenaItem ?? "odmena",
                        Nazev = npc.OdmenaItem ?? "odmena",
                        Popis = "Odmena za quest",
                        Typ = "weapon",
                        Hodnota = 5
                    };

                    hrac.Inventar.Add(reward);
                    hrac.QuestSplnen = true;

                    return $"Quest splnen! Dostal jsi: {reward.Nazev}";
                }
                else
                {
                    return "Ještě jsi mi nepřinesl všechny věci!";
                }
            }

            if (npc.DialogRoot == null)
                return "Tato postava nema dialog.";

            hrac.VDialogu = true;
            hrac.AktualniDialog = npc.DialogRoot;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(npc.DialogRoot.Text);

            for (int i = 0; i < npc.DialogRoot.Options.Count; i++)
            {
                sb.AppendLine($"{i + 1} - {npc.DialogRoot.Options[i].Text}");
            }

            return sb.ToString();
        }
    }
}