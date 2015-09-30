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
    public partial class StartForm : Form
    {
        GameMode GM;
        AI.Difficulty DIFF;
        public StartForm()
        {
            InitializeComponent();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Show();
            label2.Show();
            textBox2.Show();
            GM = GameMode.HumanVSHuman;
            groupBox3.Hide();
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Show();
            label2.Hide();
            textBox2.Hide();
            GM = GameMode.HumanVSAI;
            groupBox3.Show();
        }
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            GM = GameMode.AIVSAI;
            groupBox2.Hide();
            groupBox3.Show();
        }
        public GameStartConfig gsc
        {
            get; set;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (GM == GameMode.AIVSAI)
            {
                gsc = new GameStartConfig() { GM = GM, Difficulty = DIFF, Log = false };
                new GameForm(this, gsc).Show();
                this.Hide();
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Player 1 Name cannot be empty!!!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (GM == GameMode.HumanVSHuman)
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Player 2 Name cannot be empty!!!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            gsc = new GameStartConfig() { GM = GM, Player1Name = textBox1.Text, Player2Name = textBox2.Text, Difficulty = DIFF, Log = false };
            new GameForm(this, gsc).Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        internal void NewForm2()
        {
            new GameForm(this, gsc).Show();
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            DIFF = AI.Difficulty.Easy;
        }
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            DIFF = AI.Difficulty.Normal;
        }
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            DIFF = AI.Difficulty.Hard;
        }
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            DIFF = AI.Difficulty.Impossible;
        }
    }
}
