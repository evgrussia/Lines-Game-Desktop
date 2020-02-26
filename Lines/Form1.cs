using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lines
{
    public enum Item
    {
        none,
        ball,
        jump,
        next,
        path
    }
    public struct Ball
    {
        public int x;
        public int y;
        public int color;
    }
    public delegate void ShowItem(Ball ball, Item item);
    public partial class Form1 : Form
    {
        int max = 9;
        int size = 64;
        PictureBox[,] box;
        Game game;
        
        public Form1()
        {
            InitializeComponent();
            CreateBoxes();
         
            game = new Game(max, ShowItem);
            timer.Enabled = true;

        }
        public void CreateBoxes()   
        {
            box = new PictureBox[max, max];
            
            for (int x = 0; x < max; x++)
                for (int y = 0; y < max; y++)
                {
                    
                    box[x, y] = new PictureBox();
                    panel.Controls.Add(box[x, y]);
                    box[x, y].BorderStyle = BorderStyle.FixedSingle;
                    box[x, y].Size = new Size(size, size);
                    box[x, y].Location = new Point(x * (size-1), y * (size-1));
                    box[x, y].Image = global::Lines.Properties.Resources.empty;
                    box[x, y].SizeMode = PictureBoxSizeMode.Zoom;
                    box[x,y].Click += new System.EventHandler(this.ClickBox);
                    box[x, y].Tag = new Point(x, y);
                }
            
             panel.Size = new Size(max * (size-1)+2, max *( size-1)+2);
        }
        private void ClickBox(object sender, EventArgs e)
        {
            Point xy = (Point)((PictureBox)sender).Tag;
            game.ClickBox(xy.X, xy.Y);
        }
        private Bitmap imgBall (int nr)
        {
            switch (nr)
            {
               case 1: return global::Lines.Properties.Resources.ball1;
               case 2: return global::Lines.Properties.Resources.ball2;
               case 3: return global::Lines.Properties.Resources.ball3;
               case 4: return global::Lines.Properties.Resources.ball4;
               case 5: return global::Lines.Properties.Resources.ball5;
               case 6: return global::Lines.Properties.Resources.ball6;
            }
            return null;
        }
        private Bitmap imgJump(int nr)
        {
            switch (nr)
            {
                case 1: return global::Lines.Properties.Resources.ball1s;
                case 2: return global::Lines.Properties.Resources.ball2s;
                case 3: return global::Lines.Properties.Resources.ball3s;
                case 4: return global::Lines.Properties.Resources.ball4s;
                case 5: return global::Lines.Properties.Resources.ball5s;
                case 6: return global::Lines.Properties.Resources.ball6s;
            }
            return null;
        }
        private Bitmap imgNext(int nr)
        {
            switch (nr)
            {
                case 1: return global::Lines.Properties.Resources.ball1n;
                case 2: return global::Lines.Properties.Resources.ball2n;
                case 3: return global::Lines.Properties.Resources.ball3n;
                case 4: return global::Lines.Properties.Resources.ball4n;
                case 5: return global::Lines.Properties.Resources.ball5n;
                case 6: return global::Lines.Properties.Resources.ball6n;
            }
            return null;
        }
        private void ShowItem(Ball ball, Item item)
        {
            Image img;

            switch (item)
            {
                case Item.none: img = global::Lines.Properties.Resources.empty; break;
                case Item.ball: img = imgBall(ball.color); break;
                case Item.next: img = imgNext(ball.color); break;
                case Item.jump: img = imgJump(ball.color); break;
                case Item.path: img = global::Lines.Properties.Resources.path; break;
                default: img = global::Lines.Properties.Resources.empty; break;
            }
            box[ball.x, ball.y].Image = img;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            game.Step();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
