using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tic_Tac_Toe_GUI
{
    public class Player
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public virtual string Type { get { throw new NotImplementedException(); } }
        public virtual void Move(Game game) { throw new NotImplementedException(); }
    }
    public class Human : Player
    {
        public override void Move(Game game)
        {
            foreach (var btn in game.btnMap)
                if (string.IsNullOrWhiteSpace(btn.Value.Text))
                    btn.Value.Enabled = true;
        }
    }
    public class AI : Player
    {
        public enum Difficulty { Easy, Normal, Hard, Impossible }
        public Difficulty currentDifficulty { get; set; }
        public override async void Move(Game game)
        {
            foreach (var btn in game.btnMap)
                btn.Value.Enabled = false;

            await Task.Delay(new Random().Next(150, 500));

            switch (currentDifficulty)
            {
                //Movement Heuristics 100%
                case Difficulty.Impossible:
                    game.WriteLog("[AI] Checking for Checkmate possibility.");
                    List<int> CheckMateIndexImpossible = AIAttackAndDefenseLogic(game.pCur.Value, game.btnMap).ToList();
                    if (CheckMateIndexImpossible.Count > 0)
                    {
                        game.WriteLog("[AI] Checkmate movement found! Ignore other heuristics because this is prioritized.");
                        game.form.ProcessInput(game.btnMap[CheckMateIndexImpossible[new Random().Next(0, CheckMateIndexImpossible.Count)]]);
                        return;
                    }

                    game.WriteLog("[AI] Checking for prevent Checkmated possibility.");
                    List<int> DefenseIndexImpossible = AIAttackAndDefenseLogic(game.pCur.Next != null ? game.pCur.Next.Value : game.Players.First.Value, game.btnMap).ToList();
                    if (DefenseIndexImpossible.Count > 0)
                    {
                        game.WriteLog("[AI] Checkmated movement found! Ignore other heuristics because this is prioritized.");
                        game.form.ProcessInput(game.btnMap[DefenseIndexImpossible[new Random().Next(0, DefenseIndexImpossible.Count)]]);
                        return;
                    }

                    game.WriteLog("[AI] Checking best movement!");
                    List<int> MovementHeuristicImpossible = AIMovementLogic(game.pCur.Value, game.pCur.Next != null ? game.pCur.Next.Value : game.Players.First.Value, game.btnMap, 1).ToList();
                    if (MovementHeuristicImpossible.Count > 0)
                    {
                        game.WriteLog("[AI] Best movement found! " + string.Join(", ", MovementHeuristicImpossible));
                        game.form.ProcessInput(game.btnMap[MovementHeuristicImpossible[new Random().Next(0, MovementHeuristicImpossible.Count)]]);
                        return;
                    }

                    game.WriteLog("[AI] No priority heuristics. Continued to next functions.");
                    goto case Difficulty.Easy;

                //Checkmate Heuristics 100%
                case Difficulty.Hard:
                    List<int> CheckMateIndex = AIAttackAndDefenseLogic(game.pCur.Value, game.btnMap).ToList();
                    if (CheckMateIndex.Count > 0)
                    {
                        game.form.ProcessInput(game.btnMap[CheckMateIndex[new Random().Next(0, CheckMateIndex.Count)]]);
                        return;
                    }
                    else goto case Difficulty.Normal;

                //Defense Heuristics && Checkmate Heuristics 20%
                case Difficulty.Normal:
                    if (new Random().Next(0, 10) <= 2 && currentDifficulty == Difficulty.Normal)
                    {
                        List<int> CheckmateIndex = AIAttackAndDefenseLogic(game.pCur.Value, game.btnMap).ToList();
                        if (CheckmateIndex.Count > 0)
                        {
                            game.form.ProcessInput(game.btnMap[CheckmateIndex[new Random().Next(0, CheckmateIndex.Count)]]);
                            return;
                        }
                    }

                    List<int> DefenseIndex = AIAttackAndDefenseLogic(game.pCur.Next != null ? game.pCur.Next.Value : game.Players.First.Value, game.btnMap).ToList();
                    if (DefenseIndex.Count > 0)
                    {
                        game.form.ProcessInput(game.btnMap[DefenseIndex[new Random().Next(0, DefenseIndex.Count)]]);
                        return;
                    }
                    else goto case Difficulty.Easy;

                //Random Move
                case Difficulty.Easy:
                    int RandomMovement = await Task.Run(() =>
                    {
                        int RM;
                        do RM = new Random().Next(1, 10);
                        while (!string.IsNullOrWhiteSpace(game.btnMap[RM].Text));
                        return RM;
                    });
                    game.form.ProcessInput(game.btnMap[RandomMovement]);
                    break;

                default:
                    break;
            }
        }
        private IEnumerable<int> AIAttackAndDefenseLogic(Player p, Dictionary<int, Button> btnMap, int Threshold = 2)
        {
            //1 2 3
            //4 5 6
            //7 8 9
            int State = 0;
            List<int> Checked = new List<int>();
            List<int> Valid = new List<int>();

            //Diagonal 1.5.9 || 3.5.7
            for (int i = 1; i <= 9; i += 4)
            {
                Checked.Add(i);
                if (btnMap[i].Text == p.ID)
                {
                    State++;
                    Valid.Add(i);
                }
            }
            if (State == Threshold)
            {
                foreach (int index in Checked.Where(fx => !Valid.Any(fy => fy == fx)))
                {
                    if (string.IsNullOrWhiteSpace(btnMap[index].Text))
                        yield return index;
                }
            }
            State = 0;
            Checked.Clear();
            Valid.Clear();

            for (int i = 3; i <= 7; i += 2)
            {
                Checked.Add(i);
                if (btnMap[i].Text == p.ID)
                {
                    State++;
                    Valid.Add(i);
                }
            }
            if (State == Threshold)
            {
                foreach (int index in Checked.Where(fx => !Valid.Any(fy => fy == fx)))
                {
                    if (string.IsNullOrWhiteSpace(btnMap[index].Text))
                        yield return index;
                }
            }
            State = 0;
            Checked.Clear();
            Valid.Clear();

            //Horizontal 1.2.3 || 4.5.6 || 7.8.9
            for (int i = 1; i <= 7; i += 3)
            {
                for (int j = i; j <= i + 2; j++)
                {
                    Checked.Add(j);
                    if (btnMap[j].Text == p.ID)
                    {
                        State++;
                        Valid.Add(j);
                    }
                }
                if (State == Threshold)
                {
                    foreach (int index in Checked.Where(fx => !Valid.Any(fy => fy == fx)))
                    {
                        if (string.IsNullOrWhiteSpace(btnMap[index].Text))
                            yield return index;
                    }
                }
                State = 0;
                Checked.Clear();
                Valid.Clear();
            }

            //Vertical 1.4.7 || 2.5.8 || 3.6.9
            for (int i = 1; i <= 3; i++)
            {
                for (int j = i; j <= i + 6; j += 3)
                {
                    Checked.Add(j);
                    if (btnMap[j].Text == p.ID)
                    {
                        State++;
                        Valid.Add(j);
                    }
                }
                if (State == Threshold)
                {
                    foreach (int index in Checked.Where(fx => !Valid.Any(fy => fy == fx)))
                    {
                        if (string.IsNullOrWhiteSpace(btnMap[index].Text))
                            yield return index;
                    }
                }
                State = 0;
                Checked.Clear();
                Valid.Clear();
            }
        }
        private IEnumerable<int> AIMovementLogic(Player p, Player others, Dictionary<int, Button> btnMap, int Threshold = 2)
        {
            //1 2 3
            //4 5 6
            //7 8 9
            int State = 0;
            List<int> Checked = new List<int>();
            List<int> Valid = new List<int>();

            //Diagonal 1.5.9 || 3.5.7
            for (int i = 1; i <= 9; i += 4)
            {
                Checked.Add(i);
                if (btnMap[i].Text == p.ID)
                {
                    State++;
                    Valid.Add(i);
                }
            }
            if (State == Threshold)
            {
                if (!Checked.Any(fx => btnMap[fx].Text == others.ID))
                {
                    foreach (int index in Checked.Where(fx => !Valid.Any(fy => fy == fx)))
                    {
                        if (string.IsNullOrWhiteSpace(btnMap[index].Text))
                            yield return index;
                    }
                }
            }
            State = 0;
            Checked.Clear();
            Valid.Clear();

            for (int i = 3; i <= 7; i += 2)
            {
                Checked.Add(i);
                if (btnMap[i].Text == p.ID)
                {
                    State++;
                    Valid.Add(i);
                }
            }
            if (State == Threshold)
            {
                if (!Checked.Any(fx => btnMap[fx].Text == others.ID))
                {
                    foreach (int index in Checked.Where(fx => !Valid.Any(fy => fy == fx)))
                    {
                        if (string.IsNullOrWhiteSpace(btnMap[index].Text))
                            yield return index;
                    }
                }
            }
            State = 0;
            Checked.Clear();
            Valid.Clear();

            //Horizontal 1.2.3 || 4.5.6 || 7.8.9
            for (int i = 1; i <= 7; i += 3)
            {
                for (int j = i; j <= i + 2; j++)
                {
                    Checked.Add(j);
                    if (btnMap[j].Text == p.ID)
                    {
                        State++;
                        Valid.Add(j);
                    }
                }
                if (State == Threshold)
                {
                    if (!Checked.Any(fx => btnMap[fx].Text == others.ID))
                    {
                        foreach (int index in Checked.Where(fx => !Valid.Any(fy => fy == fx)))
                        {
                            if (string.IsNullOrWhiteSpace(btnMap[index].Text))
                                yield return index;
                        }
                    }
                }
                State = 0;
                Checked.Clear();
                Valid.Clear();
            }

            //Vertical 1.4.7 || 2.5.8 || 3.6.9
            for (int i = 1; i <= 3; i++)
            {
                for (int j = i; j <= i + 6; j += 3)
                {
                    Checked.Add(j);
                    if (btnMap[j].Text == p.ID)
                    {
                        State++;
                        Valid.Add(j);
                    }
                }
                if (State == Threshold)
                {
                    if (!Checked.Any(fx => btnMap[fx].Text == others.ID))
                    {
                        foreach (int index in Checked.Where(fx => !Valid.Any(fy => fy == fx)))
                        {
                            if (string.IsNullOrWhiteSpace(btnMap[index].Text))
                                yield return index;
                        }
                    }
                }
                State = 0;
                Checked.Clear();
                Valid.Clear();
            }
        }
    }
    public static class RandomName
    {
        private static List<string> Names = new List<string>(new string[] {
            "Darmawan","Hantze","Cege","Edu", "Ivan", "Billy","Ricky", "Gunawan","Anton","Robert","Hans",
            "Helena","Charisa","Sarah","Prisia","Athalya","Naura", "Regina","Reina"
        });
        static RandomName()
        {
            Random R = new Random();
            for (int i = 0; i < Names.Count; i++)
            {
                int Rand = R.Next(0, Names.Count);
                string Temp = Names[Rand];
                Names.RemoveAt(Rand);
                Names.Add(Temp);
            }
        }
        public static string GetName(string ex)
        {
            string R = Names[0];
            Names.RemoveAt(0);
            Names.Add(R);
            if (R.CompareTo(ex) == 0)
                return GetName(ex);
            else
                return R;
        }
    }
}
