using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hangman
{
    public partial class Hangman : Form
    {
        public Hangman()
        {
            InitializeComponent();
            Paint += Form1_Paint;
        }

        string[] words = System.IO.File.ReadAllLines("csci324.txt"); //reads in dictionary

        Random r = new Random();
        Button[] alphabetButtons;
        List<Label> labels = new List<Label>();
        bool ignore;
        bool winner;
        int stage = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            alphabetButtons = this.Controls.OfType<Button>().Except(new Button[] { Button1 }).ToArray();
            Array.ForEach(alphabetButtons, b => b.Click += btn_click);
            Button1.PerformClick();
        }

        private void btn_click(object sender, EventArgs e)
        {
            if (ignore)
            {
                return;
            }
            Button b = (Button)sender;
            b.Enabled = false;

            Array.ForEach(labels.ToArray(), lbl => lbl.Text = lbl.Tag.ToString() == b.Text ? b.Text : lbl.Text);

            Array.ForEach(labels.ToArray(), lbl =>
            {
                if (lbl.Tag.ToString() == b.Text)
                {
                    GlobalVariables.correct_letters++;
                }
            }
            );

            for (int x = 1; x <= labels.Count - 1; x++)
            {
                labels[x].Left = labels[x - 1].Right;
            }

            if (labels[labels.Count - 1].Right > this.ClientSize.Width - 14)
            {
                this.SetClientSizeCore(labels[labels.Count - 1].Right + 14, 381);
            }

            stage += !labels.Any(lbl => lbl.Text == b.Text) ? 1 : 0;
            ignore = labels.All(lbl => lbl.Text != " ") || stage == 6;

            winner = GlobalVariables.correct_letters == GlobalVariables.word_length;
            if (winner)
            {
                MessageBox.Show("You won!");
                this.Close();
            }
            else if (stage >= 6)
            {
                this.Invalidate();

                MessageBox.Show("You lost!");
                this.Close();
            }
            
            this.Invalidate();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //"new" button, resets the game, generates new hangman scenario
            this.SetClientSizeCore(546, 381);
            GlobalVariables.word = words[r.Next(0, words.Length)].ToUpper();
            Array.ForEach(this.Controls.OfType<Label>().ToArray(), lbl => lbl.Dispose());
            Array.ForEach(alphabetButtons, b => b.Enabled = true);
            labels = new List<Label>();
            int startX = 14;
            GlobalVariables.word_length = 0;
            GlobalVariables.correct_letters = 0;
            foreach (char c in GlobalVariables.word)
            {
                Label lbl = new Label();

                if (Char.IsLetter(c))
                {
                    lbl.Text = " "; // draws the space for the guess
                    lbl.Font = new Font(this.Font.Name, 35, FontStyle.Underline);
                    GlobalVariables.word_length++; //increment for each counter
                }

                else
                {
                    lbl.Text = Char.ToString(c);  //draw the character
                    lbl.Font = new Font(this.Font.Name, 35, FontStyle.Regular);
                }

                lbl.Location = new Point(startX, 250);
                lbl.Tag = c.ToString();
                lbl.AutoSize = true;
                this.Controls.Add(lbl);
                labels.Add(lbl);
                startX = lbl.Right;
            }
            ignore = false;
            winner = false;
            stage = 0;
            this.Invalidate();
        }

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // draws box and place to hang the man
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 85, 190, 210, 190);
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 148, 190, 148, 50);            
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 148, 50, 198, 50);              
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 198, 50, 198, 70);
            
            if (stage >= 1) //head
            {
                e.Graphics.DrawEllipse(new Pen(Color.Black, 2), new Rectangle(188, 70, 20, 20));
            }
            if (stage >= 2) //body
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 198, 90, 198, 130);
            }
            if (stage >= 3) //left arm
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 198, 95, 183, 115);
            }
            if (stage >= 4) //right arm
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 198, 95, 213, 115);
            }
            if (stage >= 5) //left leg
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 198, 130, 183, 170);
            }
            if (stage >= 6) //right leg
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 198, 130, 213, 170);

            } 
           
        }

        private void button28_Click(object sender, EventArgs e)
        {
            //debug button, shows word
            MessageBox.Show(GlobalVariables.word);
            stage--;
        }
    }

    public static class GlobalVariables
    {
       public static int word_length = 0;
       public static int correct_letters = 0;
        public static string word = "";
    }
}
