using System.Windows.Forms;

namespace Snake
{
    /// <summary>
    ///     Form to house the game of Snake
    ///     Ryan Harrison 2013
    ///     www.raharrison.co.uk
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        ///     The game of Snake to play
        /// </summary>
        private readonly SnakeGame game;

        /// <summary>
        ///     Initialise the form and game of Snake, passing in the PictureBox to display the game in
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            game = new SnakeGame(Canvas);
        }

        /// <summary>
        ///     Fired whenever the user presses a key on the keyboard
        /// </summary>
        /// <param name="sender">Sender of the even</param>
        /// <param name="e">Information about the key event</param>
        private void MainFormKeyDown(object sender, KeyEventArgs e)
        {
            // Let the game handle the key presses
            game.HandleKeyPress(e.KeyCode);
        }
    }
}