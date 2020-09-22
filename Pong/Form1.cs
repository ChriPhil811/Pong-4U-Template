/*
 * Description:     A basic PONG simulator
 * Author:           Christopher Phillis
 * Date:            September 21, 2020
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
        SolidBrush drawBrush = new SolidBrush(Color.White);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, kKeyDown, mKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int ballSpeed = 4;
        Rectangle ball;

        //paddle speeds and rectangles
        int paddleSpeed = 4;
        Rectangle p1, p2;

        //power up rectangles and random number gen
        Rectangle paddlePower, ballPower;
        Random randGen = new Random();
        int timer = 0; // timer for power up spawn interval

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 3;  // number of points needed to win game

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
                case Keys.K:
                    kKeyDown = true;
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
                case Keys.K:
                    kKeyDown = false;
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

            p1.Width = p2.Width = 10;    //height for both paddles set the same
            p1.Height = p2.Height = 60;  //width for both paddles set the same

            //p1 starting position
            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            //p2 starting position
            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            //ball dimensions
            ball.Width = 6;
            ball.Height = 6;

            //ball starting position
            ball.X = this.Width / 2 - ball.Width / 2;
            ball.Y = this.Height / 2 - ball.Height / 2;

            //power up dimensions
            ballPower.Width = 10;
            ballPower.Height = 10;
            paddlePower.Height = 10;
            paddlePower.Height = 10;
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {

            #region update ball position

            // TODO create code to move ball either left or right based on ballMoveRight and using BALL_SPEED
            if(ballMoveRight == true)
            {
                ball.X = ball.X + ballSpeed;
            }
            else
            {
                ball.X = ball.X - ballSpeed;
            }

            // TODO create code move ball either down or up based on ballMoveDown and using BALL_SPEED
            if(ballMoveDown == true)
            {
                ball.Y = ball.Y + ballSpeed;
            }
            else
            {
                ball.Y = ball.Y - ballSpeed;
            }
            #endregion

            #region update paddle positions

            if (aKeyDown == true && p1.Y > 0)
            {
                p1.Y = p1.Y - paddleSpeed;
            }
            
            if (zKeyDown == true && p1.Y < this.Height - p1.Height)
            {
                p1.Y = p1.Y + paddleSpeed;
            }
            
            if(kKeyDown == true && p2.Y > 0)
            {
                p2.Y = p2.Y - paddleSpeed;
            }

            if(mKeyDown == true && p2.Y < this.Height - p2.Height)
            {
                p2.Y = p2.Y + paddleSpeed;
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                ballMoveDown = true;
                collisionSound.Play();
            }
            else if(ball.Y > this.Height - ball.Height)
            {
                ballMoveDown = false;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles

            if(p1.IntersectsWith(ball))
            {
                ballMoveRight = true;
                collisionSound.Play();
            }

            if(p2.IntersectsWith(ball))
            {
                ballMoveRight = false;
                collisionSound.Play();
            }

            /*  ENRICHMENT
             *  Instead of using two if statments as noted above see if you can create one
             *  if statement with multiple conditions to play a sound and change direction
             */

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                scoreSound.Play();
                player2Score++;

                if(player2Score == gameWinScore)
                {
                    GameOver("PLAYER 2");
                }
                else
                {
                    if(ballMoveRight == false)
                    {
                        ballMoveRight = true;
                    }
                    else
                    {
                        ballMoveRight = false;
                    }

                    SetParameters();
                }
            }

            // right wall collision
            if(ball.X > this.Width - ball.Width)
            {
                scoreSound.Play();
                player1Score++;

                if (player1Score == gameWinScore)
                {
                    GameOver("PLAYER 1");
                }
                else
                {
                    if (ballMoveRight == false)
                    {
                        ballMoveRight = true;
                    }
                    else
                    {
                        ballMoveRight = false;
                    }

                    SetParameters();
                }
            }

            #endregion

            #region Power Ups

            //TODO  timer every 5 seconds
            //random power up positions
            timer++;

            if(timer > 160)
            {
                timer = 0;

                ballPower.X = randGen.Next(25, this.Width - 25);
                ballPower.Y = randGen.Next(10, this.Height - 10);
                paddlePower.X = randGen.Next(25, this.Width - 25);
                paddlePower.Y = randGen.Next(10, this.Height - 10);
            }

            //ball power makes ball and paddles faster
            //TODO paddle power makes paddles smaller
            if(ball.IntersectsWith(ballPower))
            {
                ballSpeed = ballSpeed + 2;
                paddleSpeed = paddleSpeed + 2;
            }

            if(ball.IntersectsWith(paddlePower))
            {
                p1.Height = p2.Height = 40;
            }


            #endregion Power Ups
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

            //game over logic
            gameUpdateLoop.Stop();

            startLabel.Visible = true;
            startLabel.Text = "GAME OVER! " + winner + " WINS!";
            this.Refresh();
 
            Thread.Sleep(2000);

            startLabel.Text = "Play Again?   Y / N";
            this.Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //draw paddles
            e.Graphics.FillRectangle(drawBrush, p1);
            e.Graphics.FillRectangle(drawBrush, p2);

            //draw ball
            e.Graphics.FillRectangle(drawBrush, ball);

            //draw power ups 
            e.Graphics.FillRectangle(redBrush, ballPower);
            e.Graphics.FillRectangle(blueBrush, paddlePower);

            //draw scores
            e.Graphics.DrawString(player1Score.ToString(), drawFont, drawBrush, 0, 0);
            e.Graphics.DrawString(player2Score.ToString(), drawFont, drawBrush, this.Width - 12, 0);
        }
    }
}
