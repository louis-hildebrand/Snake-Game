using System;
using System.Collections.Generic;
using System.Text;

namespace Snake_Game
{
	// Models the snake as a whole
	class Snake
	{
		public static ConsoleColor color = ConsoleColor.Green;  // Color of the snake's body
		public static char bodyChar = '@';                      // The character to represent the snake's body segments

		public int v_x { get; set; }                        // "pixels" / sec in the horizontal direction (+ = to the right)
		public int v_y { get; set; }                        // "pixels" / sec in the vertical direction   (+ = up)
		public bool dead { get; set; }                      // TRUE when the snake's head collides with another object
		public int initX { get; set; }                      // Initial x-coordinate of the snake's head
		public int initY { get; set; }                      // Initial y-coordinate of the snake's head
		public List<Segment> segmentList { get; set; }      // List of all segments in the snake's body
		public static List<char> obstacles = new List<char>();      // List of all objects that will end the game if the snake collides with them

		// Set the snake's head at the specified coordinates, add body segments, and set it moving to the right. Also create the list of obstacles.
		public Snake(int START_XVAL, int START_YVAL, Screen screen)
		{
			this.v_x = Program.xVelocity;
			this.v_y = 0;
			this.segmentList = new List<Segment>();

			initX = START_XVAL;
			initY = START_YVAL;
			for (int i = 0; i < Program.START_LEN; i++)
			{
				this.segmentList.Add(new Segment(START_XVAL - i, START_YVAL));
				screen.newPixels[START_XVAL - i, START_YVAL] = bodyChar;
			}

			// The obstacles are the walls, the cones, and the snake itself
			obstacles.Add(screen.bodyChar);
			obstacles.Add(Cone.bodyChar);
			obstacles.Add(Snake.bodyChar);
		}

		// Changes the snake's velocity in the x- and y-directions to the specified values
		public void SetVelocity(int velocityX, int velocityY)
		{
			if (!(v_x == -1 * velocityX && v_y == -1 * velocityY))  // Snake can't immediately reverse its direction of travel (this would cause it to die immediately)
			{
				v_x = velocityX;
				v_y = velocityY;
			}
		}

		// Moves the snake one "pixel" in whatever direction it was already moving
		public void Move(Screen screen, Food food)
		{
			int delta_x = 0;    // The change in the x-coordinate of the snake's head (initialized to zero just to circumvent "unassigned variable" error)
			int delta_y = 0;    // The change in the y-coordinate of the snake's head (initialized to zero just to circumvent "unassigned variable" error)
			int headXVal;       // The new x-coordinate of the snake's head
			int headYVal;       // the new y-coordinate of the snake's head

			if (v_x == 0 && v_y > 0)
			{
				delta_x = 0;
				delta_y = 1;
			}
			else if (v_x == 0 && v_y < 0)
			{
				delta_x = 0;
				delta_y = -1;
			}
			else if (v_x > 0 && v_y == 0)
			{
				delta_x = 1;
				delta_y = 0;
			}
			else if (v_x < 0 && v_y == 0)
			{
				delta_x = -1;
				delta_y = 0;
			}

			headXVal = segmentList[0].xPos + delta_x;
			headYVal = segmentList[0].yPos - delta_y;

			// Check if the next square is food.
			if (screen.newPixels[headXVal, headYVal] == Food.bodyChar)
			{
				food.eaten = true;
				// Re-print score
				Console.BackgroundColor = ConsoleColor.DarkGray;    // Keep dark gray background
				if (segmentList.Count - Program.START_LEN + 1 > Program.highScore)
					Console.ForegroundColor = ConsoleColor.Green;
				if (segmentList.Count - Program.START_LEN + 1 == Program.highScore + 1)
				{
					Console.SetCursorPosition(0, 0);
					Console.Write("SCORE: {0}", segmentList.Count - Program.START_LEN + 1); // Add one since the snake hasn't grown yet
				}
				else
				{
					Console.SetCursorPosition(7, 0);
					Console.Write(segmentList.Count - Program.START_LEN + 1);   // Add one since the snake hasn't grown yet
				}
				Console.ResetColor();
			}
			else    // If next square isn't food, delete the tail
			{
				screen.newPixels[segmentList[segmentList.Count - 1].xPos, segmentList[segmentList.Count - 1].yPos] = ' ';
				segmentList.RemoveAt(segmentList.Count - 1);
			}

			// Check for collisions
			// Note: the collision checking is deliberately done after deleting the tail so that the snake's head can pass right next to its tail
			if (obstacles.Contains(screen.newPixels[headXVal, headYVal]))
			{
				dead = true;
			}

			// If the snake isn't dead, create new head segment in front of previous head
			if (!dead)
			{
				segmentList.Insert(0, new Segment(segmentList[0].xPos + delta_x, segmentList[0].yPos - delta_y));
				screen.newPixels[headXVal, headYVal] = bodyChar;
			}
		}
	}
}
