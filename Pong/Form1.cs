/*
 * Description:     A basic PONG simulator
 * Author: Mr. T (With Brendan Fixing his code...)      
 * Date: 21/09/20   
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        private Random rnd = new Random();
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int BALL_SPEED = 4;
        Rectangle ball;

        //paddle speeds and rectangles
        const int PADDLE_SPEED = 10;
        Rectangle p1, p2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 2;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }
        
        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //set starting position for paddles on new game and point scored 
            const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            

            p1.Width = p2.Width = 5;    //height for both paddles set the same
            p1.Height = p2.Height = 50;  //width for both paddles set the same

            //p1 starting position
            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            //p2 starting position
            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            //ball height/width set
            ball.Width = 10;
            ball.Height = 10;

            //ball start position set
            ball.X = this.Width / 2 - ball.Width / 2;
            ball.Y = this.Height / 2 - ball.Height / 2;
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position
            if (ballMoveRight == true)
            {
                ball.X = ball.X + BALL_SPEED;
            }
            else
            {
                ball.X = ball.X - BALL_SPEED;
            }
            if (ballMoveDown == true)
            {
                ball.Y = ball.Y + BALL_SPEED; 
            }
            else
            {
                ball.Y = ball.Y - BALL_SPEED;
            }
            #endregion

            #region update paddle positions

            if (aKeyDown == true && p1.Y > 56)
            {
                p1.Y = p1.Y - PADDLE_SPEED;
            }
            else if (zKeyDown == true && p1.Y < this.Height - p1.Height)
            {
                p1.Y = p1.Y + PADDLE_SPEED;
            }
            if (jKeyDown == true && p2.Y > 56)
            {
                p2.Y = p2.Y - PADDLE_SPEED;
            }
            else if (mKeyDown == true && p2.Y < this.Height - p2.Height)
            {
                p2.Y = p2.Y + PADDLE_SPEED;
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 56) // if ball hits top line
            {
                ballMoveDown = true;
                collisionSound.Play();
            }
            else if (ball.Y > this.Height - ball.Height)
            {
                ballMoveDown = false;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles

            if (p1.IntersectsWith(ball) || p2.IntersectsWith(ball))
            {
                ballMoveRight = !ballMoveRight;
                collisionSound.Play();
                BALL_SPEED++;
            }
            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
               player2Score++;
               scoreSound.Play();
               BALL_SPEED = 4;
               if (player2Score == gameWinScore)
               {
                    GameOver("Player 2");
               }
               else
               {
                    ballMoveRight = !ballMoveRight;
                    SetParameters();
               }
            }

            if (ball.X > this.Width - ball.Width)  // ball hits right wall logic
            {
                player1Score++;
                scoreSound.Play();
                BALL_SPEED = 4;
                if (player1Score == gameWinScore)
                {
                    GameOver("Player 1");
                }
                else
                {
                    ballMoveRight = !ballMoveRight;
                    SetParameters();
                }
            }
            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;
            gameUpdateLoop.Enabled = false;
            startLabel.Visible = true;
            startLabel.Text = winner + " Has Won!";
            startLabel.Refresh();
            Thread.Sleep(2000);
            startLabel.Text = "Would you like to play again? (Space/N)";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(whiteBrush, p1);
            e.Graphics.FillRectangle(whiteBrush, p2);
            e.Graphics.FillRectangle(whiteBrush, ball);
            e.Graphics.FillRectangle(whiteBrush, 0, 50, this.Width, 5);
            e.Graphics.DrawString("Player One: " + player1Score, drawFont, whiteBrush, 50, 20);
            e.Graphics.DrawString("Player Two: " + player2Score, drawFont, whiteBrush, this.Width - 175, 20);
        }
    }
}
