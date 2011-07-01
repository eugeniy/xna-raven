using System;
using Microsoft.Xna.Framework;

namespace Raven
{
    public class Cell
    {
        public Point Position { get; set; }

        public Point[] Neighbors
        {
            get
            {
                return new Point[] {
                    new Point(Position.X, Position.Y + 1),
                    new Point(Position.X + 1, Position.Y),
                    new Point(Position.X, Position.Y - 1),
                    new Point(Position.X - 1, Position.Y)
                };
            }
        }

        public Point[] Corners
        {
            get
            {
                return new Point[] {
                    new Point(Position.X + 1, Position.Y + 1),
                    new Point(Position.X + 1, Position.Y),
                    new Point(Position.X, Position.Y),
                    new Point(Position.X, Position.Y + 1)
                };
            }
        }
    }
}
