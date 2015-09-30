using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tic_Tac_Toe_GUI
{
    public class Game
    {
        private Player p1, p2;
        //private Player pCur{get;set;}
        //public Queue<Player> Players { get; set; }
        public LinkedList<Player> Players { get; set; }
        public LinkedListNode<Player> pCur { get; set; }
        public Dictionary<int, Button> btnMap { get; set; }
        private Label sl;
        private Form2 form;
        public Game(GameStartConfig gs, Label statusLabel, Dictionary<int, Button> MapButton, Form2 fm)
        {
            sl = statusLabel;
            btnMap = MapButton;
            form = fm;
            switch (gs.GM)
            {
                case GameMode.HumanVSHuman:
                    p1 = new Human() { Name = gs.Player1Name, ID = "X" };
                    p2 = new Human() { Name = gs.Player2Name, ID = "O" };
                    break;
                case GameMode.HumanVSAI:
                    p1 = new Human() { Name = gs.Player1Name, ID = "X" };
                    p2 = new AI() { Name = "[AI] " + RandomName.GetName(p1.Name), ID = "O", currentDifficulty = gs.Difficulty };
                    break;
                case GameMode.AIVSAI:
                    p1 = new AI() { Name = "[AI] " + RandomName.GetName(""), ID = "X", currentDifficulty = gs.Difficulty };
                    p2 = new AI() { Name = "[AI] " + RandomName.GetName(""), ID = "O", currentDifficulty = gs.Difficulty };
                    break;
                default:
                    break;
            }
            Players = new LinkedList<Player>();
            Players.AddLast(p1);
            Players.AddLast(p2);
            pCur = Players.First;

            fm.Text = string.Format("{0} VS {1}", p1.Name, p2.Name);
            WriteTurn();

            if (p1 is AI)
                p1.Move(form);
        }
        private bool CheckWin(Player p, out Queue<int> btnIndex)
        {
            //1 2 3
            //4 5 6
            //7 8 9
            btnIndex = new Queue<int>();
            int State = 0;

            //Diagonal 1.5.9 || 3.5.7
            for (int i = 1; i <= 9; i += 4)
                if (btnMap[i].Text == p.ID)
                {
                    btnIndex.Enqueue(i);
                    State++;
                }
            if (State == 3) return true;
            else State = 0;
            btnIndex.Clear();

            for (int i = 3; i <= 7; i += 2)
                if (btnMap[i].Text == p.ID)
                {
                    btnIndex.Enqueue(i);
                    State++;
                }
            if (State == 3) return true;
            else State = 0;
            btnIndex.Clear();

            //Horizontal 1.2.3 || 4.5.6 || 7.8.9
            for (int i = 1; i <= 7; i += 3)
            {
                State = 0;
                for (int j = i; j <= i + 2; j++)
                {
                    if (btnMap[j].Text == p.ID)
                    {
                        btnIndex.Enqueue(j);
                        State++;
                    }
                }
                if (State == 3) return true;
                else State = 0;
                btnIndex.Clear();
            }

            //Vertical 1.4.7 || 2.5.8 || 3.6.9
            for (int i = 1; i <= 3; i++)
            {
                State = 0;
                for (int j = i; j <= i + 6; j += 3)
                {
                    if (btnMap[j].Text == p.ID)
                    {
                        btnIndex.Enqueue(j);
                        State++;
                    }
                }
                if (State == 3) return true;
                else State = 0;
                btnIndex.Clear();
            }

            return false;
        }
        private bool CheckGameDraw()
        {
            return btnMap.Count(fx => string.IsNullOrWhiteSpace(fx.Value.Text)) == 0;
        }
        public void NewMovement(int Index)
        {
            btnMap[Index].Text = pCur.Value.ID;
            btnMap[Index].Enabled = false;
            form.listbox.Items.Add(string.Format("{0} Move to index {1}", pCur.Value.Name, Index));

            Queue<int> btnIndex;
            if (CheckWin(pCur.Value, out btnIndex))
            {
                Write(string.Format("{0} Win!!!", pCur.Value.Name));

                foreach (var btn in btnMap)
                    btn.Value.Enabled = false;

                while (btnIndex.Count > 0)
                    btnMap[btnIndex.Dequeue()].BackColor = System.Drawing.Color.LightBlue;

                return;
            }
            else if (CheckGameDraw())
            {
                Write("Game Draw!!!");
                return;
            }
            else
            {
                pCur = pCur.Next ?? Players.First;
                WriteTurn();
                pCur.Value.Move(form);
            }
        }
        public void Write(string text)
        {
            sl.Text = text;
        }
        private void WriteTurn()
        {
            Write(string.Format("{0}'s turn", pCur.Value.Name));
        }
    }
    public enum GameMode
    {
        HumanVSHuman, HumanVSAI, AIVSAI
    }
    public class GameStartConfig
    {
        public GameMode GM { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public AI.Difficulty Difficulty { get; set; }
        public bool Log { get; set; }
    }
}
