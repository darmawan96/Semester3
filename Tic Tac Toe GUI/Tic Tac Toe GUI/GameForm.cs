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

            for (int i = 1; i <= 9; i++)
                MapButton.Add(i, Controls.Find("button_" + i, false).Single() as Button);

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
        Button beforeBtn;
        Color beforeClr;
        public Queue<Color> Qclr = new Queue<Color>(new Color[] { Color.LightBlue, Color.Orange });
        public void ProcessInput(object sender)
        {
            if (beforeBtn != null)
                beforeBtn.BackColor = beforeClr;

            Button btnSender = (Button)sender;
            beforeBtn = btnSender;
            beforeClr = btnSender.BackColor;

            btnSender.BackColor = Qclr.Dequeue();
            Qclr.Enqueue(btnSender.BackColor);

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
                Size = new Size(737, 445);
            else
                Size = new Size(353, 443);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void listBox1_MouseHover(object sender, EventArgs e)
        {
        }
    }
}
