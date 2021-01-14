using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Squiggle
{


    public partial class Squiggles : Form
    {

        // Declare necessary variables for gameplay and instantiate snake + food
        Snake player = new Snake();
        SnakeFood food = new SnakeFood();
        public int snakeLength = 3;
        public int playerScore = 0;
        public int gameTick = 0;
        public bool gameOver = false;

        public Squiggles()
        {
            InitializeComponent();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            // Instantiate objects to draw snake, snake body, and food.
            base.OnPaint(e);
            Brush myBrush = new SolidBrush(Color.Green);
            Brush myBrush2 = new SolidBrush(Color.LimeGreen);
            Brush myBrush3 = new SolidBrush(Color.White);
            Graphics g = canvas.CreateGraphics();

            // Give feedback through color if dead, doesn't draw because error pause
            if (gameOver == true) { myBrush2 = new SolidBrush(Color.LightSlateGray); myBrush = new SolidBrush(Color.LightSlateGray); }

            int i = 1;
            Snake temp = new Snake();
            temp = player;
            g.FillRectangle(myBrush2, player.xCoordinate, player.yCoordinate, 16, 16);
            g.FillRectangle(myBrush3, food.xCoordinate, food.yCoordinate, 16, 16);

            // Draw snake body
            while (i <= snakeLength - 1)
            {
                temp = temp.nextSegment;
                g.FillRectangle(myBrush, temp.xCoordinate, temp.yCoordinate, 16, 16);
                i++;
            }
        }

        private void Squiggle_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect key input to direct snake while not allowing turning directly into body from head
            if (gameOver == false && timer1.Enabled == false) { timer1.Enabled = true; pictureBox1.Enabled = false; pictureBox1.Visible = false; }
            if (e.KeyCode == Keys.Down && player.yVelocity != 1 && player.lastYVelocity != 1)
            {
                player.xVelocity = 0;
                player.yVelocity = -1;
            }
            if (e.KeyCode == Keys.Up && player.yVelocity != -1 && player.lastYVelocity != -1)
            {
                player.xVelocity = 0;
                player.yVelocity = 1;
            }
            if (e.KeyCode == Keys.Right && player.xVelocity != -1 && player.lastXVelocity != -1)
            {
                player.xVelocity = 1;
                player.yVelocity = 0;
            }
            if (e.KeyCode == Keys.Left && player.xVelocity != 1 && player.lastXVelocity != 1)
            {
                player.xVelocity = -1;
                player.yVelocity = 0;
            }
        }

        public void snakeMovement()
        {
            int i = 1;
            Snake temp = player;

            // Iterate through each piece of snake and perform movement
            while (i <= snakeLength)
            {
                if (temp.nextSegment != null) temp.nextSegment.xVelocity = temp.lastXVelocity;
                if (temp.nextSegment != null) temp.nextSegment.yVelocity = temp.lastYVelocity;
                if (temp.xVelocity == 1)
                {
                    temp.xCoordinate += 20;
                    temp.lastXVelocity = 1;
                    temp.lastYVelocity = 0;
                    if (temp.xCoordinate > 760) temp.xCoordinate = 0;
                }
                if (temp.xVelocity == -1)
                {
                    temp.xCoordinate -= 20;
                    temp.lastXVelocity = -1;
                    temp.lastYVelocity = 0;
                    if (temp.xCoordinate < 0) temp.xCoordinate = 760;
                }
                if (temp.yVelocity == 1)
                {
                    temp.yCoordinate -= 20;
                    temp.lastYVelocity = 1;
                    temp.lastXVelocity = 0;
                    if (temp.yCoordinate < 0) temp.yCoordinate = 540;
                }
                if (temp.yVelocity == -1)
                {
                    temp.yCoordinate += 20;
                    temp.lastYVelocity = -1;
                    temp.lastXVelocity = 0;
                    if (temp.yCoordinate > 540) temp.yCoordinate = 0;
                }
                temp = temp.nextSegment;
                i++;
            }

        }


        public bool foodCollisionCheck()
        {
            // Call for increase snake size if food eaten detected
            if ((player.xCoordinate == food.xCoordinate) && (player.yCoordinate == food.yCoordinate))
            {
                if (timer1.Interval > 5) timer1.Interval -= 5;
                else { timer1.Interval = 2; }
                food.foodEaten();
                addSnake();
                return true;
            }
            return false;
        }

        public bool selfCollisionCheck()
        {
            // Check head doesn't collide with body
            int i = 2;
            Snake temp = player.nextSegment;
            while (i <= snakeLength)
            {
                if ((player.xCoordinate == temp.xCoordinate) && (player.yCoordinate == temp.yCoordinate)) { gameOver = true; return true; }
                i++;
                temp = temp.nextSegment;
            }
            return false;
        }

        public void addScore(int scoreAdd)
        {
            playerScore += scoreAdd;
        }

        public void addSnake()
        {

            // Iterate through snake to find tail, then create next body segment while transferring velocity
            int i = 1;
            Snake temp = player;
            while (i != snakeLength)
            {
                temp = temp.nextSegment;
                i++;
            }
            if (temp.lastXVelocity == 1) temp.nextSegment = new Snake((temp.xCoordinate - 20), temp.yCoordinate);
            if (temp.lastXVelocity == -1) temp.nextSegment = new Snake((temp.xCoordinate + 20), temp.yCoordinate);
            if (temp.lastYVelocity == 1) temp.nextSegment = new Snake(temp.xCoordinate, (temp.yCoordinate + 20));
            if (temp.lastYVelocity == -1) temp.nextSegment = new Snake(temp.xCoordinate, (temp.yCoordinate - 20));
            snakeLength++;

        }
        
        public void gameOverParce()
        {

            // Prompt user if they would like to replay

            DialogResult result = MessageBox.Show($"Your score is {playerScore:d6}! Would you like to play again?", "Game Over!", MessageBoxButtons.YesNo, MessageBoxIcon.Hand);
            switch (result)
            {
                case DialogResult.Yes:
                    Application.Restart();
                    break;
                case DialogResult.No:
                    this.Close();
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Game timer to call movement and chek for collision, handle score, update feedback
            snakeMovement();
            if (selfCollisionCheck()) { timer1.Enabled = false; gameOverParce(); }
                gameTick++;
                if (gameTick == 10) { addScore(1); gameTick = 0; }
                if (foodCollisionCheck()) addScore(50);
                canvas.Invalidate();
                if (this.Focused) Squiggles.ActiveForm.Text = ($"Squiggle | Score: {playerScore:d6} | X: {player.xCoordinate} Y: {player.yCoordinate} | Length: {snakeLength}");
            }


        }

        public class Snake
        {
            public int xCoordinate { get; set; } // -x goes left, +x goes right
            public int yCoordinate { get; set; } // -y should go down, +y should go up
            public int xVelocity { get; set; }  // the speed will remain constant. direction dependant on key input
            public int yVelocity { get; set; } // the speed will remain constant. direction dependant on key input
            public int lastXVelocity { get; set; }
            public int lastYVelocity { get; set; }
            public Snake nextSegment { get; set; } = null;

            // Use the default constructor as a method of just spawning a snake in the center of the game
            public Snake()
            {
                xCoordinate = 380;
                yCoordinate = 280;
                xVelocity = 0;
                yVelocity = -1;
                lastXVelocity = 0;
                lastYVelocity = -1;
                nextSegment = new Snake(380, 260);
                nextSegment.lastYVelocity = -1;
                nextSegment.nextSegment = new Snake(380, 240);
                nextSegment.nextSegment.lastYVelocity = -1;
            }

            public Snake(int xCoord, int yCoord)
            {
                xCoordinate = xCoord;
                yCoordinate = yCoord;
                xVelocity = 0;
                yVelocity = 0;
                lastXVelocity = 0;
                lastYVelocity = 0;

            }


        }

        public class SnakeFood
        {
            public int xCoordinate { get; set; }
            public int yCoordinate { get; set; }

            public SnakeFood()
            {
                // Randomize food placement in gameplay plane
                xCoordinate = (new Random().Next(0, 38) * 20);
                yCoordinate = (new Random().Next(0, 27) * 20);
            }

            public void foodEaten()
            {
                // Randomize food placement while guaranteeing it won't duplicate the same position

                int oldX = xCoordinate;
                int oldY = yCoordinate;
                while ((xCoordinate == oldX) || (yCoordinate == oldY))
                {
                    xCoordinate = (new Random().Next(0, 38) * 20);
                    yCoordinate = (new Random().Next(0, 27) * 20);
                }
            }

        }

    }


