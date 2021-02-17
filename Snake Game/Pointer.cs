using System;
using System.Collections.Generic;
using System.Text;

namespace Snake_Game
{
	// Pointers in the main menu and settings menu
	class Pointer
	{
		public int xPos { get; set; }   // x-coordinate of the position
		public int yPos { get; set; }   // y-coordinate of the position
		public int minY { get; set; }   // y-coordinate of the highest option the pointer will have to reach
		public int maxY { get; set; }   // y-coordinate of the lowest option the pointer will have to reach
		public ConsoleColor color { get; set; }

		public Pointer(int xVal, int yVal, Screen screen, ConsoleColor userColor, int minimum, int maximum)
		{
			this.xPos = xVal;
			this.yPos = yVal;
			this.minY = minimum;
			this.maxY = maximum;

			this.color = userColor;
			screen.newPixels[xPos, yPos] = '>';
		}

		// Moves the pointer up or down by the specified distance
		public void Move(int dist, Screen screen)
		{
			screen.newPixels[xPos, yPos] = ' ';     // Clear the old pointer position
			int num = Program.CharToInt(screen.newPixels[xPos + 2, yPos]);  // num is the index of the message the pointer was just highlighting (within the Screen list "messages"). The index is in newPixels[] (2 pixels to the right of the pointer)
			for (int x = xPos + 2; x < xPos + 2 + screen.messages[num].Length; x++) // Clear the message from oldPixels[] so that it gets re-printed (without highlighting)
			{
				screen.oldPixels[x, yPos] = ' ';
			}

			if (yPos == minY && dist == 2)  // If user is at highest option and presses up, loop around to lowest option
			{
				yPos = maxY;
			}
			else if (yPos == maxY && dist == -2)    // If user is at lowest option and presses down, loop around to highest value
			{
				yPos = minY;
			}
			else
			{
				yPos -= dist;
			}

			screen.newPixels[xPos, yPos] = '>';     // Add the pointer to newPixels[] in the new location
			num = Program.CharToInt(screen.newPixels[xPos + 2, yPos]);
			for (int x = xPos + 2; x < xPos + 2 + screen.messages[num].Length; x++) // Find the message on the line the pointer is now on and clear it from newPixels so that it gets re-printed with highlighting
			{
				screen.oldPixels[x, yPos] = ' ';
			}
		}
	}
}
