using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tic_Tac_Toe_GUI
{
    public partial class GameForm : Form
    {

        StartForm fm;
        public Dictionary<int, Button> MapButton { get; set; }
        public Game game { get; set; }
        public ListBox listbox { get { return listBox1; } }
        public GameForm(StartForm fm1, GameStartConfig gs)
        {
            InitializeComponent();

            MapButton = new Dictionary<int, Button>();
            MapButton.Add(1, button_1);
            MapButton.Add(2, button_2);
            MapButton.Add(3, button_3);
            MapButton.Add(4, button_4);
            MapButton.Add(5, button_5);
            MapButton.Add(6, button_6);
            MapButton.Add(7, button_7);
            MapButton.Add(8, button_8);
            MapButton.Add(9, button_9);

            game = new Game(gs, statusLabel, MapButton, this);
            fm = fm1;
            log = gs.Log;
            tlog();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
        }
        bool IsRestarting = false;
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!IsRestarting)
                fm.Close();
        }
        public void ProcessInput(object sender)
        {
            Button btnSender = (Button)sender;
            int btnIndex = int.Parse(btnSender.Name.Split('_').Last());
            game.NewMovement(btnIndex);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ProcessInput(sender);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ProcessInput(sender);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ProcessInput(sender);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            ProcessInput(sender);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            ProcessInput(sender);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            ProcessInput(sender);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            ProcessInput(sender);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            ProcessInput(sender);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            ProcessInput(sender);
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            IsRestarting = true;
            fm.NewForm2();
            this.Close();
        }
        bool log;
        private void button2_Click_1(object sender, EventArgs e)
        {
            log = !log;

            tlog();

            fm.gsc.Log = log;
        }
        private void tlog()
        {
            if (log)
                Size = new Size(630, 443);
            else
                Size = new Size(353, 443);
        }
        int previndex = 0;
        Color prevcolor;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (previndex != 0)
                MapButton[previndex].BackColor = prevcolor;

            if (listBox1.SelectedItem == null)
                return;

            int index = int.Parse(listBox1.SelectedItem.ToString().Last().ToString());
            previndex = index;

            prevcolor = MapButton[index].BackColor;
            MapButton[index].BackColor = Color.Wheat;
        }
        private void listBox1_MouseHover(object sender, EventArgs e)
        {
        }
    }
}
