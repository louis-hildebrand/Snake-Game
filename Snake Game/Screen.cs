using System;
using System.Collections.Generic;
using System.Text;

namespace Snake_Game
{
	// All dynamic displays in the game are formatted as Screen objects
	class Screen
	{
		// The character used to represent the border in newPixels
		public char bodyChar;
		// The border's dimensions - upper left corner (x1, y1), bottom right corner (x2, y2)
		public int x1 { get; set; }
		public int y1 { get; set; }
		public int x2 { get; set; }
		public int y2 { get; set; }
		// Save screen contents in arrays (save previous screen in oldPixels so as to avoid reprinting pixels that haven't changed)
		public char[,] newPixels { get; set; }
		public char[,] oldPixels { get; set; }
		// A list of messages to display to the user. The index of the message to display is stored in newPixels.
		public List<string> messages { get; set; }

		// Initialize the screen with the border at the specified coordinates and printed with the specified char
		public Screen(int leftXVal, int topYVal, int rightXVal, int botYVal, char c)
		{
			this.x1 = leftXVal;
			this.y1 = topYVal;
			this.x2 = rightXVal;
			this.y2 = botYVal;
			this.bodyChar = c;

			messages = new List<string>();

			newPixels = new char[Program.SCREEN_WIDTH, Program.SCREEN_HEIGHT];  // The char arrays cover the entire console (therefore width = width of the window, height = height of the window)
			oldPixels = new char[Program.SCREEN_WIDTH, Program.SCREEN_HEIGHT];  // (NOTE): the x-coordinates are stored in the array rows and the y-coordinates are stored in the columns so that standard coordinate notation (x, y) can be used to reference points.
			for (int y = 0; y < Program.SCREEN_HEIGHT; y++)
			{
				for (int x = 0; x < Program.SCREEN_WIDTH; x++)
				{
					oldPixels[x, y] = ' ';  // Initialize the old screen to be zero

					// Print the border in newPixels, otherwise initialize all entries to 'space'
					if (x <= x1 || x >= x2 || y <= y1 || y >= y2)
					{   // If on border, pixels are equal to border char (print white background so that the border looks solid)
						newPixels[x, y] = this.bodyChar;
					}
					else
					{
						newPixels[x, y] = ' ';
					}
				}
			}
		}

		// Receives info about the menu pointer and prints the screen accordingly (used in the main menu)
		public void PrintScreen(Pointer pointer, bool printDarkGrayBackground = false)      // If printDarkGrayBackground is TRUE, reset the console background colour to dark gray instead of black
		{
			int num;

			for (int y = 0; y < Program.SCREEN_HEIGHT; y++)
			{
				for (int x = 0; x < Program.SCREEN_WIDTH; x++)
				{

					if (newPixels[x, y] != oldPixels[x, y]) // Avoid re-printing characters that haven't changed from the previous screen
					{

						Console.SetCursorPosition(x, y);
						if (y == pointer.yPos && x > x1 && x < x2)
						{
							Console.ForegroundColor = pointer.color;            // Highlight the entire line that the pointer is on
						}
						else
						{
							Console.ForegroundColor = ConsoleColor.White;
						}

						if (newPixels[x, y] == bodyChar && newPixels[x, y] != ' ')  // Print a solid white pixel if it's the border (but not if the border character is ' ')
						{
							Console.BackgroundColor = ConsoleColor.White;
							Console.Write(' ');
							if (printDarkGrayBackground)
								Console.BackgroundColor = ConsoleColor.DarkGray;
							else
								Console.ResetColor();
						}
						else if (!int.TryParse(newPixels[x, y].ToString(), out num))    // If the char is not a number, print it as is. Otherwise, print the message in the list with the corresponding index
						{
							Console.Write(newPixels[x, y]);
						}
						else
						{
							Console.Write(messages[num]);
							x += (messages[num].Length - 1);
						}

						if (printDarkGrayBackground)
							Console.BackgroundColor = ConsoleColor.DarkGray;
						else
							Console.ResetColor();
					}
				}
			}

			for (int y = 0; y < Program.SCREEN_HEIGHT; y++) // Copy the current screen to oldPixels
			{
				for (int x = 0; x < Program.SCREEN_WIDTH; x++)
				{
					oldPixels[x, y] = newPixels[x, y];
				}
			}

			Console.ForegroundColor = ConsoleColor.White;   // Reset the font colour to white to that another method doesn't accidentally start printing in the wrong colour
		}

		// Receives info about the snake and the food and prints the screen accordingly (used in the game screen)
		public void PrintScreen(Snake snake, Food food)
		{
			int num;

			for (int y = 0; y < Program.SCREEN_HEIGHT; y++)
			{
				for (int x = 0; x < Program.SCREEN_WIDTH; x++)
				{

					if (newPixels[x, y] != oldPixels[x, y])     // Avoid re-printing characters that haven't changed from the previous screen
					{

						Console.SetCursorPosition(x, y);    // Print the snake's body with the appropriate color
						if (newPixels[x, y] == Snake.bodyChar)
						{
							Console.ForegroundColor = Snake.color;
							Console.Write(Snake.bodyChar);
							Console.ForegroundColor = ConsoleColor.White;
						}
						else if (newPixels[x, y] == Food.bodyChar)      // Print the food with the appropriate color
						{
							Console.ForegroundColor = Food.color;
							Console.Write(Food.bodyChar);
							Console.ForegroundColor = ConsoleColor.White;
						}
						else if (newPixels[x, y] == Cone.bodyChar)      // Print the cones with the appropriate color
						{
							Console.ForegroundColor = Cone.color;
							Console.Write(Cone.bodyChar);
							Console.ForegroundColor = ConsoleColor.White;
						}
						else if (newPixels[x, y] == bodyChar && newPixels[x, y] == bodyChar)    // Print a solid white pixel if it's the border (but not if the border character is ' ')
						{
							Console.BackgroundColor = ConsoleColor.White;
							Console.Write(' ');
							Console.ResetColor();
						}
						else if (!int.TryParse(newPixels[x, y].ToString(), out num))    // If the char is not a number, print it as is. Otherwise, print the message in the list with the corresponding index.
						{
							Console.Write(newPixels[x, y]);
						}
						else
						{
							Console.Write(messages[num]);
							x += (messages[num].Length - 1);
						}
					}
				}
			}

			for (int y = 0; y < Program.SCREEN_HEIGHT; y++)     // Copy the current screen to oldPixels
			{
				for (int x = 0; x < Program.SCREEN_WIDTH; x++)
				{
					oldPixels[x, y] = newPixels[x, y];
				}
			}
		}

		// Clears the old screen between (x1, y1) and (x2, y2) (used when it is necessary to print the entire screen again)
		public void ClearOldPixels(int x1 = 0, int y1 = 0, int x2 = Program.SCREEN_WIDTH - 1, int y2 = Program.SCREEN_HEIGHT - 1)
		{
			for (int y = y1; y <= y2; y++)
			{
				for (int x = x1; x <= x2; x++)
				{
					oldPixels[x, y] = ' ';
				}
			}
		}

		// Prints the screen between (x1, y1) and (x2, y2), regardless of the relative contents of newPixels and oldPixels (used in the main menu)
		public void PrintFullScreen(Pointer pointer, int x1 = 0, int y1 = 0, int x2 = Program.SCREEN_WIDTH - 1, int y2 = Program.SCREEN_HEIGHT - 1, bool printDarkGrayBackground = false)
		{
			for (int y = y1; y <= y2; y++)
			{
				for (int x = x1; x <= x2; x++)
				{
					Console.SetCursorPosition(x, y);
					Console.Write(' ');
				}
			}
			ClearOldPixels(x1, y1, x2, y2);
			PrintScreen(pointer, printDarkGrayBackground);
		}

		// Prints the screen between (x1, y1) and (x2, y2), regardless of the relative contents of newPixels and oldPixels (used in the game screen)
		public void PrintFullScreen(Snake snake, Food food, int x1 = 0, int y1 = 0, int x2 = Program.SCREEN_WIDTH - 1, int y2 = Program.SCREEN_HEIGHT - 1)
		{
			for (int y = y1; y <= y2; y++)
			{
				for (int x = x1; x <= x2; x++)
				{
					Console.SetCursorPosition(x, y);
					Console.Write(' ');
				}
			}
			ClearOldPixels(x1, y1, x2, y2);
			PrintScreen(snake, food);
		}

		// Displays a message at the specified location and stores it to the list of messages
		public void PrintMessage(string msg, int xval, int yval)
		{
			Console.SetCursorPosition(xval, yval);  // Print the message at the given coordinates
			Console.Write(msg);

			messages.Add(msg);                      // Add it to the list

			for (int x = xval; x < xval + msg.Length; x++)
			{   // Add the index of the message to the screen array so that the message re-appears each time the full screen is printed
				newPixels[x, yval] = Program.IntToChar(messages.Count - 1);
			}
		}
	}
}
