using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Hangman
{
    public partial class Form1 : Form
    {
        //This boolean will track whether the post has been drawn
        private int incorrectGuesses = 0;
        private bool postDrawn = false;
        private bool bodyPartsDrawn = false;
        string word = "";
        List<Label> labels = new List<Label>();
        int amount = 0;
        public Form1()
        {
            InitializeComponent();
            //Subscribe to the Paint event of panel1
            //panel1.Paint += Panel1_Paint;
        }

        //enum represent the hangman body parts
        enum BodyParts
        {
            Head,
            Body,
            Right_Arm,
            Left_Arm,
            Right_Leg,
            Left_Leg
        }

        //event handler for the Paint event of panel1

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            // Check if the post has already been drawn
            if (!postDrawn)
            {
                // If not drawn, call DrawingPost to draw the post and set postDrawn to true
                DrawingPost(e.Graphics);
                postDrawn = true;
            }


        }

        // Method to draw the post
        void DrawingPost(Graphics G)
        {
            Graphics g = panel1.CreateGraphics();
            //this will determine the thickness of pen and color when drawing the line for the post
            Pen p = new Pen(Color.Pink, 15);

            //g.DrawLine(p, new Point(175, 218), new Point(175, 5)) 
            // determines the straightness and length of the vertical post to the right.
            g.DrawLine(p, new Point(175, 218), new Point(175, 5));

            // g.DrawLine(p, new Point(183, 5), new Point(65, 5)); = horizontal line on top of panel strteches to post and the straigtness of the horizontal post.
            g.DrawLine(p, new Point(183, 5), new Point(65, 5));

            // g.DrawLine(p, new Point(183, 5), new Point(65, 5)); = horizontal line on bottom of panel strteches under the vertical.
            g.DrawLine(p, new Point(218, 218), new Point(130, 218));

            //g.DrawLine(p, new Point(60, 0), new Point(60, 50)) = determines the straigtness and the length of the horizontal post to hangman head
            g.DrawLine(p, new Point(60, 0), new Point(60, 50));


            DrawBodyPart(BodyParts.Head);
            DrawBodyPart(BodyParts.Body);
            DrawBodyPart(BodyParts.Left_Arm);
            DrawBodyPart(BodyParts.Right_Arm);
            DrawBodyPart(BodyParts.Left_Leg);
            DrawBodyPart(BodyParts.Right_Leg);
            // MessageBox.Show(GetRandomWord());
        }

        // Method to draw a specific body part of the hangman
        void DrawBodyPart(BodyParts bp)
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.Purple, 5);
            if (bp == BodyParts.Head)
                g.DrawEllipse(p, 40, 50, 40, 40);

            else if (bp == BodyParts.Body)
            {
                g.DrawLine(p, new Point(60, 90), new Point(60, 173));
            }
            else if (bp == BodyParts.Left_Arm)
            {
                g.DrawLine(p, new Point(60, 100), new Point(30, 120));
            }

            else if (bp == BodyParts.Right_Arm)
            {
                g.DrawLine(p, new Point(60, 100), new Point(90, 120));
            }

            else if (bp == BodyParts.Left_Leg)
            {
                g.DrawLine(p, new Point(60, 170), new Point(30, 190));
            }

            else if (bp == BodyParts.Right_Leg)
            {
                g.DrawLine(p, new Point(60, 170), new Point(90, 190));
            }

        }


        //make labels in the groupbox where letters will appear

        void MakeLabels()
        {
            word = GetRandomWord();
            char[] chars = word.ToCharArray();
            int between = 330 / chars.Length - 1;
            int x = 10; // Initial X position
            for (int i = 0; i < chars.Length - 1; i++) // Change chars.Length - 1 to chars.Length
            {
                labels.Add(new Label());
                labels[i].Location = new Point(x, 80); // Use x for the X position
                labels[i].Text = "_";
                labels[i].Parent = groupBox2;
                labels[i].BringToFront();
                labels[i].CreateControl();
                x += between + 20; // Increment x by the gap between labels and an additional offset
            }
            label1.Text = "Word Length: " + (chars.Length - 1).ToString();
        }

        //this will generate random word from the wordlist link
        string GetRandomWord()
        {
            /*provides common method for sending data and receiving data from a
            resource identified by a Uniform Resource Identifier*/
            WebClient wc = new WebClient();

            /* this calls the DownloadString method of the WebClient instance
             * 'wc' to download the content from the URL*/
            string wordList = wc.DownloadString("https://www.dictionary-thesaurus.com/wordlists/Adjectives%28929%29.txt");

            /*splits the wordlist string into an array so each word in the
            list is a separate line */
            string[] words = wordList.Split('\n');

            //creates a new random class
            Random ran = new Random();

            //returns randomly selcted word from the words array. 
            return words[ran.Next(0, words.Length - 1)];
        }

        // Event handler for the Form's Shown event
        private void Form1_Shown(object sender, EventArgs e)
        {
            // Trigger the Paint event of panel1 to draw the hangman
            panel1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MakeLabels();

        }

        // Method to check if the game is over
        private void CheckGameOver()
        {
            if (incorrectGuesses >= 6) // If 6 or more incorrect guesses
            {
                MessageBox.Show("Game Over! The word was " + word, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // You may want to reset the game here
            }
        }

        //matching a letter with randomly generated word
        private void button1_Click(object sender, EventArgs e)
        {
            char letter = textBox1.Text.ToLower().ToCharArray()[0];
            if (!char.IsLetter(letter))
            {
                MessageBox.Show("You can only submit letters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (word.Contains(letter))
            {
                char[] letters = word.ToCharArray();
                for (int i = 0; i < letters.Length; i++)
                {
                    if (letters[i] == letter)
                        labels[i].Text = letter.ToString();
                }
            }

            else
            {
                MessageBox.Show("The letter that you guessed isn't in the word!", "Sorry");
                label2.Text += " " + letter.ToString() + ",";
                DrawBodyPart((BodyParts)amount);
                amount++;

                incorrectGuesses++; // Increment incorrect guesses
                CheckGameOver(); // Check if the game is over
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
