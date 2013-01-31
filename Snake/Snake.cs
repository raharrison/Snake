using System.Collections.Generic;
using System.Drawing;

namespace Snake
{
    /// <summary>
    ///     Class representing a Snake
    ///     Each Snake consists of a series of Sectors
    ///     The Snake can grow and move as needed
    /// 
    ///     Ryan Harrison 2013
    ///     www.raharrison.co.uk
    /// </summary>
    internal class Snake
    {
        /// <summary>
        ///     The list of Sectors that make up a Snake
        /// </summary>
        private readonly List<Sector> segments;

        /// <summary>
        ///     The default size of each Sector
        /// </summary>
        private readonly int size;

        /// <summary>
        ///     The default position of the Snake
        /// </summary>
        private Point defaultPos;

        /// <summary>
        ///     Construct a new Snake object with the specified segment size
        ///     Intitially the default position is (100, 100)
        /// </summary>
        /// <param name="size"></param>
        public Snake(int size) : this(size, new Point(100, 100))
        {
        }

        /// <summary>
        ///     Construct a new Snake object with specified Sector size and default position
        /// </summary>
        /// <param name="size">The size of each segment of the Snake</param>
        /// <param name="defaultPos">The default position of the Snake</param>
        public Snake(int size, Point defaultPos)
        {
            this.size = size;
            this.defaultPos = defaultPos;
            segments = new List<Sector>();
        }

        /// <summary>
        ///     Property for the Direction the Snake is moving in
        /// </summary>
        public Direction MovingDirection { get; set; }

        /// <summary>
        ///     The current length of the Snake (number of sectors)
        /// </summary>
        public int Length
        {
            get { return segments.Count; }
        }

        /// <summary>
        ///     Clear the Snake of all exisiting Sectors, reset the position to the default and add five Sector
        /// </summary>
        public void Reset()
        {
            segments.Clear();
            segments.Add(new Sector(defaultPos.X, defaultPos.Y, size, size));
            Grow(5);
        }

        /// <summary>
        ///     Add speicifed number of Sectors to the Snake
        /// </summary>
        /// <param name="howMany">The number of new Sectors to add to the end</param>
        public void Grow(int howMany)
        {
            while (howMany > 0)
            {
                // Add to the end of the list the same segment as is at the current end of the list
                segments.Add(new Sector(segments[segments.Count - 1]));
                howMany--;
            }
        }

        /// <summary>
        ///     Move the Snake in the current MovingDirection
        /// </summary>
        public void Move()
        {
            // Copy the current position of each segment to the Sector before it (up the the head)
            // Simulates each Sector of the snake following the head of the Snake
            for (int n = Length - 1; n >= 1; n--)
            {
                segments[n] = segments[n - 1].Clone() as Sector;
            }

            // Move the head Sector in the current MovingDirection
            switch (MovingDirection)
            {
                case Direction.Left:
                    segments[0].X -= size;
                    break;
                case Direction.Right:
                    segments[0].X += size;
                    break;
                case Direction.Up:
                    segments[0].Y -= size;
                    break;
                case Direction.Down:
                    segments[0].Y += size;
                    break;
            }
        }

        /// <summary>
        ///     Get the head of the Snake
        /// </summary>
        /// <returns>The head Sector of the Snake if present</returns>
        public Sector GetHeadSector()
        {
            return segments.Count >= 1 ? segments[0] : null;
        }

        /// <summary>
        ///     Get the Sector of the Snake at the specified index
        /// </summary>
        /// <param name="index">The index of the Sector to retrieve</param>
        /// <returns>The Sector of the Snake at index</returns>
        public Sector GetSectorAt(int index)
        {
            return segments[index];
        }
    }
}