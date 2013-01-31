using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Snake
{
    /// <summary>
    ///     Class represnting a classic game of Snake
    ///     The aim of the game is to direct a Snake towards food which will add to the length of the snake
    ///     The game is over if the Snake hits any wall or itself
    /// 
    ///     Ryan Harrison 2013
    ///     www.raharrison.co.uk
    /// </summary>
    internal class SnakeGame
    {
        /// <summary>
        ///     The size of each segment of the Snake
        /// </summary>
        private const int SIZE = 10;

        /// <summary>
        ///     Brush used when painting the body of the Snake
        /// </summary>
        private readonly Brush bodyBrush = new SolidBrush(Color.LimeGreen);

        /// <summary>
        ///     Brush used when painting the food that the Snake must eat
        /// </summary>
        private readonly Brush foodBrush = new SolidBrush(Color.Yellow);

        /// <summary>
        ///     The component from the parent to display the game in
        /// </summary>
        private readonly PictureBox gamePanel;

        /// <summary>
        ///     Assosiates a normlised number (1,2,3) with the amount of time to sleep in between tick of the game
        /// </summary>
        private readonly Dictionary<int, int> gameSpeeds;

        /// <summary>
        ///     Timer to sequence each tick of the game. Upon firing the game is updated and painted
        /// </summary>
        private readonly Timer gameTimer;

        /// <summary>
        ///     The list of Sectors representing the current grid of play
        ///     Each sector of the snake, and the food, are locations in the grid
        /// </summary>
        private readonly List<Sector> grid;

        /// <summary>
        ///     Brush used when painting the head of the Snake
        /// </summary>
        private readonly Brush headBrush = new SolidBrush(Color.Red);

        /// <summary>
        ///     Brush used when painting information about the current game of Snake
        /// </summary>
        private readonly Brush infoBrush = new SolidBrush(Color.White);

        /// <summary>
        ///     Font used when displaying information about the current game of Snake
        /// </summary>
        private readonly Font infoFont = new Font("Verdana", 16, FontStyle.Bold);

        /// <summary>
        ///     Random number generator for use when changing the location of food when the Snake has eaten some
        /// </summary>
        private readonly Random random;

        /// <summary>
        ///     Brush used when painting the current players score
        /// </summary>
        private readonly Brush scoreBrush = new SolidBrush(Color.Gray);

        /// <summary>
        ///     Font used when displaying the current players score
        /// </summary>
        private readonly Font scoreFont = new Font("Verdana", 12, FontStyle.Bold);

        /// <summary>
        ///     The Snake used in the current game
        /// </summary>
        private readonly Snake snake;

        /// <summary>
        ///     List to hold the queue of directions the Snake must turn in
        ///     Solves the problem of when the user wants the snake to move in two directions on one tick of the game
        /// </summary>
        private readonly List<Direction> turnQueue;

        /// <summary>
        ///     A sector representing the food the Snake must eat to grow
        /// </summary>
        private Sector food;

        /// <summary>
        ///     Booleans to denote whether or not the game has started, ended or is paused
        /// </summary>
        private bool gameEnded;

        /// <summary>
        ///     Booleans to denote whether or not the game has started, ended or is paused
        /// </summary>
        private bool gamePaused;

        /// <summary>
        ///     The current speed of the game, can be changed by the user in game
        /// </summary>
        private int gameSpeed;

        /// <summary>
        ///     Booleans to denote whether or not the game has started, ended or is paused
        /// </summary>
        private bool gameStarted;

        /// <summary>
        ///     The current players score
        /// </summary>
        private int score;

        /// <summary>
        ///     Construct a new game of Snake with the host component to paint the game in
        /// </summary>
        /// <param name="host">The PictureBox used to display the game</param>
        public SnakeGame(PictureBox host)
        {
            // Set up events on the panel to allow painting
            gamePanel = host;
            gamePanel.Paint += GamePanelPaint;

            random = new Random(Environment.TickCount);

            grid = new List<Sector>();
            turnQueue = new List<Direction>();
            gameSpeeds = new Dictionary<int, int>();

            // Fill the game speeds and set the initial speed
            FillSpeeds(gameSpeeds);
            gameSpeed = 4;

            // Initially the game has not been started
            gameStarted = false;

            // Initialise the grid of segments and the Snake. The initial location of the Snake is (200, 200)
            InitGrid(grid);
            snake = new Snake(SIZE, new Point(200, 200));

            // Start a new game of Snake
            NewGame(true);

            // Initalise the timer and set up events
            // The interval between ticks is the value associated with the current game speed
            gameTimer = new Timer();
            gameTimer.Interval = gameSpeeds[gameSpeed];
            gameTimer.Tick += GameTimerTick;

            // Start the timer to start updating and displaying the game on each tick
            gameTimer.Start();
        }

        /// <summary>
        ///     Start a new game of Snake
        ///     If this new game is the first, then the Snake is initially not moving
        /// </summary>
        /// <param name="startingGame">Is this the first game of snake that has been played</param>
        private void NewGame(bool startingGame)
        {
            // If the new game is the first that has been started, initially the snake is not moving
            if (startingGame)
            {
                snake.MovingDirection = Direction.NotMoving;
            }
            else
            {
                // Otherwise the Snake is moving right and the game has started
                snake.MovingDirection = Direction.Right;
                if (!gameStarted)
                    gameStarted = true;
            }

            // Reset the Snake to prepare for a new game
            snake.Reset();

            // Find a location for the food
            food = GetRandomSector();

            // Reset the values of all fields to start a new game
            turnQueue.Clear();
            score = 0;
            gamePaused = false;
            gameEnded = false;
        }

        /// <summary>
        ///     Method fired when gamePanel.Invalidate() is called
        ///     Draws the current Snake, food and information to the PictureBox
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Information about the paint event</param>
        private void GamePanelPaint(object sender, PaintEventArgs e)
        {
            lock (this)
            {
                // Get a Bitmap for the PictureBox and Graphics object to allow painting onto the Bitmap
                var image = new Bitmap(gamePanel.Width, gamePanel.Height);
                Graphics g = Graphics.FromImage(image);

                try
                {
                    // Enable anti-aliasing to make the game look better
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    // Fill the image with a black background
                    g.FillRectangle(new SolidBrush(Color.Black), 0, 0, gamePanel.Width, gamePanel.Height);

                    // Draw the snake, food and information about the game
                    DrawSnake(g);
                    DrawFood(g);
                    DrawInfo(g);

                    // Paint prompts the user depending on the current state of the game
                    if (!gameStarted)
                        DrawGameBegin(g);
                    if (gameEnded)
                        DrawGameEnded(g);
                    if (gamePaused)
                        DrawGamePaused(g);

                    // Paint the Bitmap onto the PictureBox
                    e.Graphics.DrawImage(image, 0, 0);
                }
                finally
                {
                    e.Dispose();
                    g.Dispose();
                }
            }
        }

        /// <summary>
        ///     Method fired when the timer has ticked
        ///     Updates the Snake and checks if the food is hit or if the game is over
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Information about the event</param>
        private void GameTimerTick(object sender, EventArgs e)
        {
            // Only apply game logic when the game is not paused or ended
            if (!gamePaused && !gameEnded)
            {
                // Apply the most recent queued Direction
                if (turnQueue.Count >= 1)
                {
                    // If the new direction is compatible, change the moving direction of the snake
                    if (IsCompatible(turnQueue[0]))
                    {
                        snake.MovingDirection = turnQueue[0];
                    }

                    // Remove the queued Direction from the list
                    turnQueue.RemoveAt(0);
                }

                // Move the Snake
                snake.Move();

                // If the Snake has hit the food
                if (HasHitFood())
                {
                    // Relocate the food, make the Snake grow by 5 segments and increase the score
                    food = GetRandomSector();
                    snake.Grow(5);
                    score += 10;
                }

                // If the game is over and the snake is moving then the game has ended
                if (IsGameOver() && snake.MovingDirection != Direction.NotMoving)
                {
                    gameEnded = true;
                }
            }

            // Paint the current game state onto the PictureBox by firing the Paint event
            gamePanel.Invalidate();
        }

        /// <summary>
        ///     Draw a prompt to the user to begin the game
        /// </summary>
        /// <param name="g">Graphics object to use when painting</param>
        private void DrawGameBegin(Graphics g)
        {
            g.DrawString("Hit Space to begin", infoFont, infoBrush, gamePanel.Width / 2 - 120, gamePanel.Height / 2 - 20);
        }

        /// <summary>
        ///     Draw a prompt to the user when the game has been paused
        /// </summary>
        /// <param name="g">Graphics object to use when painting</param>
        private void DrawGamePaused(Graphics g)
        {
            g.DrawString("Game Paused", infoFont, infoBrush, gamePanel.Width / 2 - 100, gamePanel.Height / 2 - 20);
        }

        /// <summary>
        ///     Draw a prompt to the user when the game has ended
        /// </summary>
        /// <param name="g">Graphics object to use when painting</param>
        private void DrawGameEnded(Graphics g)
        {
            g.DrawString("Game Over!", infoFont, infoBrush, gamePanel.Width / 2 - 70, gamePanel.Height / 2 - 30);
            g.DrawString("Hit Space to try again", infoFont, infoBrush, gamePanel.Width / 2 - 130, gamePanel.Height / 2);
        }

        /// <summary>
        ///     Draw a prompt to the user consisting of the current players score and the current game speed
        /// </summary>
        /// <param name="g">Graphics object to use when painting</param>
        private void DrawInfo(Graphics g)
        {
            g.DrawString("Score: " + score + "    Speed: " + gameSpeed, scoreFont, scoreBrush, 2, 2);
        }

        /// <summary>
        ///     Draw the food that the Snake must eat
        /// </summary>
        /// <param name="g">Graphics object to use when painting</param>
        private void DrawFood(Graphics g)
        {
            g.FillEllipse(foodBrush, food.X, food.Y, food.Width, food.Height);
        }

        /// <summary>
        ///     Paint each Sector of the Snake object
        /// </summary>
        /// <param name="g">Graphics object to use when painting</param>
        private void DrawSnake(Graphics g)
        {
            // First of all paint the head sector
            Sector head = snake.GetHeadSector();
            g.FillEllipse(headBrush, head.X, head.Y, head.Width, head.Height);

            // Then paint each sector of the Snake after the head
            for (int n = 1; n < snake.Length; n++)
            {
                Sector s = snake.GetSectorAt(n);
                g.FillEllipse(bodyBrush, s.X, s.Y, s.Width, s.Height);
            }
        }

        /// <summary>
        ///     Get a random sector in the grid with a distinct location not present in any sector of the Snake
        /// </summary>
        /// <returns>A random sector of the grid not in the Snake</returns>
        private Sector GetRandomSector()
        {
            Sector randomSector = null;
            bool foundSuitableSector = false;

            while (!foundSuitableSector)
            {
                // Get a random sector
                randomSector = grid[random.Next(0, grid.Count)];

                // need to ensure that the random sector is not a part of the Snake
                for (int i = 0; i < snake.Length; i++)
                {
                    if (!Sector.Equals(snake.GetSectorAt(i), randomSector))
                    {
                        foundSuitableSector = true;
                    }
                    else
                    {
                        foundSuitableSector = false;
                        break;
                    }
                }
            }

            return randomSector;
        }

        /// <summary>
        ///     Fill the associations between numbers and time to sleep in between ticks
        /// </summary>
        /// <param name="speeds">The Dictionary of speeds to populate</param>
        private static void FillSpeeds(IDictionary<int, int> speeds)
        {
            speeds.Add(1, 150);
            speeds.Add(2, 100);
            speeds.Add(3, 80);
            speeds.Add(4, 40);
            speeds.Add(5, 20);
            speeds.Add(6, 1);
        }

        /// <summary>
        ///     Initialise the grid of sectors representing the current game area
        /// </summary>
        /// <param name="gameGrid">The list of Sectors to populate</param>
        private void InitGrid(ICollection<Sector> gameGrid)
        {
            int x = gamePanel.Width - SIZE;
            int y = gamePanel.Height - SIZE;

            for (int i = 0; i <= x; i += SIZE)
            {
                for (int j = 0; j <= y; j += SIZE)
                {
                    var gridSector = new Sector(i, j, SIZE, SIZE);
                    gameGrid.Add(gridSector);
                }
            }
        }

        /// <summary>
        ///     Determine whether or not the Snake has hit itself
        ///     The Snake has hit itself if the location of the head is the same as any other sector of the Snake
        /// </summary>
        /// <returns>True if the Snake has hit itself, false otherwise</returns>
        private bool HasHitSelf()
        {
            Sector head = snake.GetHeadSector();

            // Determine whether or not the head is at the same location as any other sector of the Snake
            for (int i = 1; i < snake.Length; i++)
            {
                if (Sector.Equals(head, snake.GetSectorAt(i)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Determines whether or not the Snake has hit the current food object
        ///     The Snake has hit the food if the location of the head sector is the same as that of the food
        /// </summary>
        /// <returns>True of the Snake has hit the food, false otherwise</returns>
        private bool HasHitFood()
        {
            return Sector.Equals(snake.GetHeadSector(), food);
        }

        /// <summary>
        ///     Determines whether or not the game is over
        ///     The game is over if the Snake has hit any of the walls or itself
        /// </summary>
        /// <returns>True if the game is over, false otherwise</returns>
        private bool IsGameOver()
        {
            Sector headSector = snake.GetHeadSector();

            // Left side
            if (headSector.X < 0 && snake.MovingDirection == Direction.Left)
            {
                return true;
            }
            // Top side
            if (headSector.Y < 0 && snake.MovingDirection == Direction.Up)
            {
                return true;
            }
            // Right side
            if (headSector.X > gamePanel.Width - SIZE && snake.MovingDirection == Direction.Right)
            {
                return true;
            }
            // Bottom side
            if (headSector.Y > gamePanel.Height - SIZE && snake.MovingDirection == Direction.Down)
            {
                return true;
            }
            // Snake hit itself
            return HasHitSelf();
        }

        /// <summary>
        ///     Determines if a Direction is valid when considering the current moving Direction
        /// </summary>
        /// <param name="newDirection">The new Direction to test</param>
        /// <returns>True if the new Direction can be considered valid, false otherwise</returns>
        private bool IsCompatible(Direction newDirection)
        {
            // Can't be the same as the current Direction
            if (snake.MovingDirection == newDirection)
                return false;

            // Can't be opposite Directions
            if (snake.MovingDirection == Direction.Left && newDirection == Direction.Right)
                return false;

            if (snake.MovingDirection == Direction.Right && newDirection == Direction.Left)
                return false;

            if (snake.MovingDirection == Direction.Up && newDirection == Direction.Down)
                return false;

            if (snake.MovingDirection == Direction.Down && newDirection == Direction.Up)
                return false;

            return true;
        }

        /// <summary>
        ///     Handle a key press from the user
        /// </summary>
        /// <param name="key">The Key that the user has pressed</param>
        public void HandleKeyPress(Keys key)
        {
            // Start a new game if space is pressed
            if (key == Keys.Space)
            {
                NewGame(false);
            }

            // Only handle other presses if a game has been started
            if (gameStarted)
            {
                switch (key)
                {
                    case Keys.Left:
                        SetDirection(Direction.Left);
                        break;
                    case Keys.Down:
                        SetDirection(Direction.Down);
                        break;
                    case Keys.Right:
                        SetDirection(Direction.Right);
                        break;
                    case Keys.Up:
                        SetDirection(Direction.Up);
                        break;
                    case Keys.Add: // Increase the speed of the game
                        if (gameSpeed + 1 <= gameSpeeds.Count)
                        {
                            gameTimer.Interval = gameSpeeds[++gameSpeed];
                        }
                        break;
                    case Keys.Subtract: // Decrease the speed of the game
                        if (gameSpeed - 1 > 0)
                        {
                            gameTimer.Interval = gameSpeeds[--gameSpeed];
                        }
                        break;
                    case Keys.P: // Pause the game
                        if (!gameEnded)
                            gamePaused = !gamePaused;
                        break;
                }
            }
        }

        /// <summary>
        ///     Queue a change in direction of the Snake
        /// </summary>
        /// <param name="newDirection">The new Direction of the Snake</param>
        public void SetDirection(Direction newDirection)
        {
            // Only queue the new direction if the game is not paused or ended
            if (!gamePaused && !gameEnded)
            {
                // If there is a direction already queued, only add the new direction if it is different to the
                // last one queued
                if (turnQueue.Count > 0)
                {
                    if (turnQueue[turnQueue.Count - 1] != newDirection)
                    {
                        turnQueue.Add(newDirection);
                        return;
                    }
                }

                // Otherwise we can just queue the new direction
                turnQueue.Add(newDirection);
            }
        }
    }
}