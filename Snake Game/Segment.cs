using System;
using System.Collections.Generic;
using System.Text;

namespace Snake_Game
{
	// Individual pixels in the snake's body are modelled as Segment objects
	public class Segment
	{
		public int xPos { get; set; }   // x-coordinate of the snake's position
		public int yPos { get; set; }   // y-coordinate of the snake's position

		// Initialize the position using the specified coordinates
		public Segment(int xval, int yval)
		{
			this.xPos = xval;
			this.yPos = yval;
		}
	}
}
