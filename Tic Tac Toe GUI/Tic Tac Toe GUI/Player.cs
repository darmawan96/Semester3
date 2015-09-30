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
        public virtual void Move(Form2 form) { throw new NotImplementedException(); }
    }
    public class Human : Player
    {
        public override void Move(Form2 form)
        {
            foreach (var btn in form.MapButton)
                if (string.IsNullOrWhiteSpace(btn.Value.Text))
                    btn.Value.Enabled = true;
        }
    }
    public class AI : Player
    {
        public enum Difficulty { Easy, Normal, Hard, Impossible }
        public Difficulty currentDifficulty { get; set; }
        public override async void Move(Form2 form)
        {
            foreach (var btn in form.MapButton)
                btn.Value.Enabled = false;

            await Task.Delay(new Random().Next(150, 500));

            switch (currentDifficulty)
            {
                //Movement Heuristics 100%
                case Difficulty.Impossible:

                    List<int> CheckMateIndexImpossible = AIAttackAndDefenseLogic(form.game.pCur.Value, form.MapButton).ToList();
                    if (CheckMateIndexImpossible.Count > 0)
                    {
                        form.ProcessInput(form.MapButton[CheckMateIndexImpossible[new Random().Next(0, CheckMateIndexImpossible.Count)]]);
                        return;
                    }

                    List<int> DefenseIndexImpossible = AIAttackAndDefenseLogic(form.game.pCur.Next != null ? form.game.pCur.Next.Value : form.game.Players.First.Value, form.MapButton).ToList();
                    if (DefenseIndexImpossible.Count > 0)
                    {
                        form.ProcessInput(form.MapButton[DefenseIndexImpossible[new Random().Next(0, DefenseIndexImpossible.Count)]]);
                        return;
                    }

                    List<int> MovementHeuristicImpossible = AIMovementLogic(form.game.pCur.Value, form.game.pCur.Next != null ? form.game.pCur.Next.Value : form.game.Players.First.Value, form.MapButton, 1).ToList();
                    if (MovementHeuristicImpossible.Count > 0)
                    {
                        form.ProcessInput(form.MapButton[MovementHeuristicImpossible[new Random().Next(0, MovementHeuristicImpossible.Count)]]);
                        return;
                    }

                    goto case Difficulty.Easy;

                //Checkmate Heuristics 100%
                case Difficulty.Hard:
                    List<int> CheckMateIndex = AIAttackAndDefenseLogic(form.game.pCur.Value, form.MapButton).ToList();
                    if (CheckMateIndex.Count > 0)
                    {
                        form.ProcessInput(form.MapButton[CheckMateIndex[new Random().Next(0, CheckMateIndex.Count)]]);
                        return;
                    }
                    else goto case Difficulty.Normal;

                //Defense Heuristics && Checkmate Heuristics 20%
                case Difficulty.Normal:
                    if (new Random().Next(0, 10) <= 2 && currentDifficulty == Difficulty.Normal)
                    {
                        List<int> CheckmateIndex = AIAttackAndDefenseLogic(form.game.pCur.Value, form.MapButton).ToList();
                        if (CheckmateIndex.Count > 0)
                        {
                            form.ProcessInput(form.MapButton[CheckmateIndex[new Random().Next(0, CheckmateIndex.Count)]]);
                            return;
                        }
                    }

                    List<int> DefenseIndex = AIAttackAndDefenseLogic(form.game.pCur.Next != null ? form.game.pCur.Next.Value : form.game.Players.First.Value, form.MapButton).ToList();
                    if (DefenseIndex.Count > 0)
                    {
                        form.ProcessInput(form.MapButton[DefenseIndex[new Random().Next(0, DefenseIndex.Count)]]);
                        return;
                    }
                    else goto case Difficulty.Easy;

                //Random Move
                case Difficulty.Easy:
                    int RandomMovement = await Task.Run(() =>
                    {
                        int RM;
                        do RM = new Random().Next(1, 10);
                        while (!string.IsNullOrWhiteSpace(form.MapButton[RM].Text));
                        return RM;
                    });
                    form.ProcessInput(form.MapButton[RandomMovement]);
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
            "Darmawan","Hantze","Cege","Edu","Ricky","Robert","Hans",
            "Helena","Charisa","Sarah","Prisia","Athalya"
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
