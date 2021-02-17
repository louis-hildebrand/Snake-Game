using System;
using System.Collections.Generic;
using System.Text;

namespace Snake_Game
{
	// Obstacles within the field of play
	class Cone
	{
		public static ConsoleColor color = ConsoleColor.Yellow;         // Colour of the cones
		public static char bodyChar = 'A';                              // Character used to represent the cones

		public int xPos { get; set; }
		public int yPos { get; set; }

		public Cone(int xVal, int yVal, Screen screen)
		{
			this.xPos = xVal;
			this.yPos = yVal;
			screen.newPixels[xPos, yPos] = bodyChar;
		}
	}
}
