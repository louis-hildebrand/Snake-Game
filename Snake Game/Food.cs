using System;
using System.Collections.Generic;
using System.Text;

namespace Snake_Game
{
	class Food
	{
		public static ConsoleColor color = ConsoleColor.Red;    // Colour of the food
		public static char bodyChar = 'Q';                      // Character used to represent the food

		public int xPos { get; set; }   // x-coordinate of the food's position
		public int yPos { get; set; }   // y-coordinate of the food's position
		public bool eaten { get; set; } // TRUE when the snake eats the food by moving its head over it

		// Create the food object and set eaten = true so that a new one can spawn
		public Food()
		{
			this.eaten = true;
		}

		// If the food has been eaten, randomly generate a new piece
		public void Spawn(Screen screen, Snake snake)
		{
			Random rand = new Random();

			if (eaten)
			{
				do
				{
					xPos = rand.Next(5, Program.SCREEN_WIDTH - 5);   // Choose random spot for food (at least 4 spaces away from walls)
					yPos = rand.Next(5, Program.SCREEN_HEIGHT - 5);
				} while (Snake.obstacles.Contains(screen.newPixels[xPos, yPos]) || !(xPos % Program.xVelocity == snake.initX % Program.xVelocity && yPos % Program.yVelocity == snake.initY % Program.yVelocity));
				// Check that the food doesn't spawn on an existing object. 
				// Also, the snake moves in discrete steps (2 horizontally per frame). If the snake is on an even numbered column and the food spawns on an odd-numbered column, it is unreachable. If this happens, generate a new piece.

				screen.newPixels[xPos, yPos] = bodyChar;
				eaten = false;
			}
		}
	}
}
