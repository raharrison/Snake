using System;
using System.Drawing;

namespace Snake
{
    /// <summary>
    ///     Class representing a Sector of a snake
    ///     Each sector is a Rectangle holding x, y, width and height values
    ///     Sector objects can be cloned
    /// 
    ///     Ryan Harrison 2013
    ///     www.raharrison.co.uk
    /// </summary>
    internal class Sector : ICloneable
    {
        /// <summary>
        ///     The Rectangle representing this Sector
        /// </summary>
        private Rectangle rect;

        /// <summary>
        ///     Construct a new Sector object with specified x, y, width and height
        ///     Initialises the Rectangle field using the parameters
        /// </summary>
        /// <param name="x">The X position of the Sector</param>
        /// <param name="y">The Y position of the Sector</param>
        /// <param name="width">The Width of the Sector</param>
        /// <param name="height">The Height of the Sector</param>
        public Sector(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
        }

        /// <summary>
        ///     Construct a new Sector object from an existing Sector
        /// </summary>
        /// <param name="sector">The existing Sector object</param>
        public Sector(Sector sector)
        {
            rect = sector.rect;
        }

        /// <summary>
        ///     Property for the the X coordinate of the Sector
        /// </summary>
        public int X
        {
            get { return rect.X; }
            set { rect.X = value; }
        }

        /// <summary>
        ///     Property for the Y coordinate of the Sector
        /// </summary>
        public int Y
        {
            get { return rect.Y; }
            set { rect.Y = value; }
        }

        /// <summary>
        ///     Property for the width of the sector
        /// </summary>
        public int Width
        {
            get { return rect.Width; }
            set { rect.Width = value; }
        }

        /// <summary>
        ///     Property for the height of the Sector
        /// </summary>
        public int Height
        {
            get { return rect.Height; }
            set { rect.Height = value; }
        }

        /// <summary>
        ///     Clones the current Sector object
        /// </summary>
        /// <returns>A memberwise clone of the current Sector</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///     Set the location of the Sector
        /// </summary>
        /// <param name="x">The new X location</param>
        /// <param name="y">The new Y location</param>
        public void SetLocation(int x, int y)
        {
            rect.X = x;
            rect.Y = y;
        }

        /// <summary>
        ///     Determines if two Sector objects can be considered equal.
        ///     Two Sectors are equal if both the x and y coordinates are the same
        /// </summary>
        /// <param name="a">The first Sector to test</param>
        /// <param name="b">The second Sector to test</param>
        /// <returns>True if both Sectors are equal, false otherwise</returns>
        public static bool Equals(Sector a, Sector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
    }
}