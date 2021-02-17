/* TESTED BY
 * Samantha La Rosa (friend/classmate)
 * Alexandre Daigneault (friend/classmate)
 */

/* UPDATE LOG
 * Last updated: 14/12/2019
 * 
 * 25/11/2019 - Completed main menu
 * 		      - Started update log
 * 28/11/2019 - Created snake class (simple movement is now working)
 * 29/11/2019 - Added food mechanic
 *            - Added difficulty selection menu to allow user to set snake speed
 *            - Prevented user from "commiting suicide" by attempting to turn the snake back towards itself
 *            - Fixed bug with unreachable food due to discrete movement
 * 30/11/2019 - Removed difficulty selection menu (unnecessary code and too difficult to find three speeds that are reasonable at high fps)
 *            - Added exit option for the game and for the main menu
 *            - Added more thorough comments throughout the program
 * 05/12/2019 - Added code to set console dimensions at the beginning of the program
 *            - Cleaned up some sections of code
 *            - Wrote INSTRUCTIONS section for main menu
 *            - Added high score tracker
 * 11/12/2019 - More code clean-up (e.g. renamed some variables for clarity, added comments)
 * 12/12/2019 - Added cone objects (additional obstacles in field of play)
 *			  - Added settings menu to allow user to select the speed (really, the refresh rate) and the number of cones
 * 13/12/2019 - Made cursor hidden when not in use
 * 			  - Adjusted default settings, added ability for user to select initial snake length
 *			  - Set a different background colour for the GAME OVER message
 *			  - Fixed bug where cones spawn inside screen border
 *			  - Modified PrintFullScreen method to minimize unnecessary printing
 * 14/12/2019 - Added new game modes!
 *			  - Added dark gray background for INSTRUCTIONS and SETTINGS screens
 *			  - More basic optimization (e.g. re-print scoreboard only when snake eats)
 */


using System;
using System.Collections.Generic;

namespace Snake_Game
{
	class Program
	{
		public const int DEFAULT_REFRESH_RATE = 25;             // Default value of REFRESH_RATE
		public static int REFRESH_RATE = DEFAULT_REFRESH_RATE;  // Frames per second
		public const int DEFAULT_NUM_CONES = 0;                 // Default value of NUM_CONES
		public const int MAX_NUM_CONES = 100;                   // Max value of NUM_CONES
		public static int NUM_CONES = DEFAULT_NUM_CONES;        // Number of cones to place in the field
		public const int DEFAULT_START_LEN = 25;                // Default value of START_LEN
		public const int MAX_START_LEN = 120;                   // Max value of START_LEN
		public static int START_LEN = DEFAULT_START_LEN;        // Initial length of the snake
		public static int xVelocity = 2;                        // "Pixels" / frame (twice as large as vertical velocity since each "pixel" in the console is taller than it is wide)
		public static int yVelocity = 1;                        // "Pixels" / frame
		public const int SCREEN_WIDTH = 148;                    // Width of the console
		public const int SCREEN_HEIGHT = 37;                    // Height of the console
		public static int highScore = 0;                        // User's highest score in the current session

		public static void Main()
		{
			Console.WindowWidth = SCREEN_WIDTH;     // Not available on Repl.it
			Console.WindowHeight = SCREEN_HEIGHT;   // Not available on Repl.it

			Console.CursorVisible = false;          // Hide the cursor from the user

			// Set up main menu objects
			Screen MainMenu = new Screen(0, 0, SCREEN_WIDTH - 1, SCREEN_HEIGHT - 1, ' ');
			Pointer MainMenuPointer = new Pointer(SCREEN_WIDTH / 2 - 2, 15, MainMenu, ConsoleColor.Green, 15, 25);

			Console.ForegroundColor = ConsoleColor.Black;   // Initially hide the messages because the whole screen gets re-printed later

			// Static messages
			MainMenu.PrintMessage("SNAKE", SCREEN_WIDTH / 2 - 3, 5);
			MainMenu.PrintMessage("Developed by Louis Hildebrand", SCREEN_WIDTH / 2 - 15, 7);
			// Options
			MainMenu.PrintMessage("GAME MODE: CLASSIC", SCREEN_WIDTH / 2, 15);
			MainMenu.PrintMessage("GAME MODE: MONTRÉAL CENTRE-VILLE", SCREEN_WIDTH / 2, 17);
			MainMenu.PrintMessage("GAME MODE: NEED FOR SPEED", SCREEN_WIDTH / 2, 19);
			MainMenu.PrintMessage("GAME MODE: CUSTOM", SCREEN_WIDTH / 2, 21);
			MainMenu.PrintMessage("INSTRUCTIONS", SCREEN_WIDTH / 2, 23);
			MainMenu.PrintMessage("EXIT", SCREEN_WIDTH / 2, 25);

			Console.ForegroundColor = ConsoleColor.White;

			ConsoleKeyInfo cki = new ConsoleKeyInfo();      // Create ConsoleKeyInfo object to take user keystrokes
			do
			{
				MainMenu.PrintFullScreen(MainMenuPointer);

				// Allow user to scroll up and down between options in the menu. If they press ENTER, break and move to the corresponding screen.
				do
				{
					cki = Console.ReadKey(true);
					switch (cki.Key)
					{
						case ConsoleKey.UpArrow:
							MainMenuPointer.Move(2, MainMenu);  // If user presses up arrow, move the pointer up and highlight the new line (or move down to lowest option if pointer is already at the top)
							break;
						case ConsoleKey.DownArrow:
							MainMenuPointer.Move(-2, MainMenu); // If user presses down arrow, move the pointer down and highlight the new line (or move down to highest option if pointer is already at the bottom)
							break;
						default:
							break;
					}

					MainMenu.PrintScreen(MainMenuPointer);
				} while (cki.Key != ConsoleKey.Enter);

				// Clear the console and move to the appropriate screen according to the last position of the pointer
				Console.Clear();
				switch (MainMenuPointer.yPos)
				{
					case 15:    // GAME MODE: CLASSIC (all default values: medium speed, moderate starting length, no cones)
						highScore = Game(Program.DEFAULT_REFRESH_RATE, Program.DEFAULT_NUM_CONES, Program.DEFAULT_START_LEN);
						break;
					case 17:    // GAME MODE: MONTRÉAL CENTRE-VILLE (slow speed, very long starting length, A LOT of cones)
						highScore = Game(Program.DEFAULT_REFRESH_RATE - 15, Program.MAX_NUM_CONES, Program.MAX_START_LEN);
						break;
					case 19:    // GAME MODE: NEED FOR SPEED (high speed, a few cones, default starting length)
						highScore = Game(Program.DEFAULT_REFRESH_RATE + 10, 10, Program.DEFAULT_START_LEN);
						break;
					case 21:    // GAME MODE: CUSTOM (let user set game parameters  in Settings() and use those in Game())
						Settings();
						highScore = Game(Program.REFRESH_RATE, Program.NUM_CONES, Program.START_LEN);
						break;
					case 23:    // INSTRUCTIONS
								// Print dark gray background
						Console.BackgroundColor = ConsoleColor.DarkGray;
						for (int y = 1; y < SCREEN_HEIGHT - 1; y++)
						{
							for (int x = 1; x < SCREEN_WIDTH - 1; x++)
							{
								Console.SetCursorPosition(x, y);
								Console.Write(' ');
							}
						}
						// Print instructions (need to use Console.SetCursorPosition() at each step so that the gray background doesn't leak out all the way to the left side of the console)
						Console.SetCursorPosition(2, 2);
						Console.Write("INSTRUCTIONS");
						Console.SetCursorPosition(2, 3);
						Console.Write("------------");
						Console.SetCursorPosition(2, 5);
						Console.Write("You control a big, hungry snake:      ");
						// Print image of the snake
						Console.ForegroundColor = Snake.color;
						if (Program.START_LEN <= 100)   // Display only the first 100 segments if snake is too wide for the screen
							Console.Write(new String(Snake.bodyChar, Program.START_LEN));
						else
							Console.Write(new String(Snake.bodyChar, 100));
						Console.ForegroundColor = ConsoleColor.White;
						Console.SetCursorPosition(2, 6);
						Console.Write("Guide it to the food [");
						// Print an image of the food
						Console.ForegroundColor = Food.color; Console.Write(Food.bodyChar); Console.ForegroundColor = ConsoleColor.White;
						Console.Write("] using the arrow keys or WASD.");
						Console.SetCursorPosition(2, 7);
						Console.Write("Be careful! If the snake collides with a wall [");
						// Print a solid white pixel for the wall
						Console.BackgroundColor = ConsoleColor.White;
						Console.Write(' ');
						Console.BackgroundColor = ConsoleColor.DarkGray;
						Console.Write("], a traffic cone [");
						// Print an image of a traffic cone
						Console.ForegroundColor = Cone.color; Console.Write(Cone.bodyChar); Console.ForegroundColor = ConsoleColor.White;
						Console.Write("], or its own tail, the game will be lost.");

						// Explanations of game modes
						Console.SetCursorPosition(2, 10);
						Console.Write("GAME MODE: CLASSIC | Moderate speed, moderate starting length, and no obstacles other than the walls and your own tail.");
						Console.SetCursorPosition(2, 12);
						Console.Write("GAME MODE: MONTRÉAL CENTRE-VILLE | Try to navigate through an area jam-packed with traffic cones. Of course, there will be no speeding");
						Console.SetCursorPosition(2, 13);
						Console.Write("with all this construction!");
						Console.SetCursorPosition(2, 15);
						Console.Write("GAME MODE: NEED FOR SPEED | Channel your inner Usain Bolt with this high-speed game mode (but still keep an eye out for those cones)!");
						Console.SetCursorPosition(2, 17);
						Console.Write("GAME MODE: CUSTOM | Set your own parameters!");

						Console.SetCursorPosition(2, 21);
						Console.WriteLine("Press any key to return to the main menu.");
						Console.ResetColor();
						Console.ReadKey(true);
						break;
					default:    // Note: the option 21 is included in default since nothing happens besides the program exiting the loop
						break;
				}

				// Once the user is done with that particular screen, clear the console and return to the main menu
				Console.Clear();
			} while (MainMenuPointer.yPos != 25);

			// Print end screen
			Console.SetCursorPosition(SCREEN_WIDTH / 2 - 3, 5);
			Console.Write("SNAKE");
			Console.SetCursorPosition(SCREEN_WIDTH / 2 - 15, 7);
			Console.Write("Developed by Louis Hildebrand");
			Console.SetCursorPosition(SCREEN_WIDTH / 2 - 58, 11);
			Console.Write("Special thanks to Alexandre Daigneault and Samantha La Rosa for testing the game and for their valuable suggestions!");
			Console.SetCursorPosition(SCREEN_WIDTH/2 - 10, 30);
			Console.Write("Press any key to exit");
			Console.ReadKey();
		}


		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//      USER-DEFINED METHODS
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		// Menu to set parameters before custom game
		public static void Settings()
		{
			ConsoleKeyInfo cki = new ConsoleKeyInfo();  // Declare a new variable to take keystrokes inside this method

			// Initialize menu objects
			Screen SettingsMenu = new Screen(1, 0, SCREEN_WIDTH - 2, SCREEN_HEIGHT - 1, ' ');
			Pointer SettingsPointer = new Pointer(2, 15, SettingsMenu, ConsoleColor.Green, 9, 15);

			// Print dark gray background
			Console.BackgroundColor = ConsoleColor.DarkGray;
			for (int y = 1; y < SCREEN_HEIGHT - 1; y++)
			{
				for (int x = 1; x < SCREEN_WIDTH - 1; x++)
				{
					Console.SetCursorPosition(x, y);
					Console.Write(' ');
				}
			}

			// Header and instructions
			SettingsMenu.PrintMessage("SETTINGS", 2, 2);
			SettingsMenu.PrintMessage("--------", 2, 3);
			SettingsMenu.PrintMessage("Use the arrow keys to choose a parameter and press ENTER to modify it. The square brackets indicate default values for the classic game mode.", 2, 5);
			SettingsMenu.PrintMessage("(NOTE): Selecting slow speed will lead to lower frame rate.", 2, 31);

			// Options
			SettingsMenu.PrintMessage("Snake Speed [medium]", 4, 9);
			SettingsMenu.PrintMessage("Number of Cones [" + Program.DEFAULT_NUM_CONES + "]", 4, 11);
			SettingsMenu.PrintMessage("Starting Length [" + Program.DEFAULT_START_LEN + "]", 4, 13);
			SettingsMenu.PrintMessage("START GAME", 4, 15);

			// Print most recent values for all options	
			// Snake speed
			Console.SetCursorPosition(27, 9);
			Console.Write(new String(' ', SCREEN_WIDTH - 28));  // Clear previous value
			Console.SetCursorPosition(27, 9);
			switch (Program.REFRESH_RATE)
			{
				case Program.DEFAULT_REFRESH_RATE - 15:
					Console.Write("slow");
					break;
				case Program.DEFAULT_REFRESH_RATE:
					Console.Write("medium");
					break;
				case Program.DEFAULT_REFRESH_RATE + 10:
					Console.Write("fast");
					break;
				default:
					break;
			}
			// Number of cones
			Console.SetCursorPosition(27, 11);
			Console.Write(new String(' ', SCREEN_WIDTH - 28));  // Clear previous value
			Console.SetCursorPosition(27, 11);
			Console.Write(Program.NUM_CONES);
			// Starting length
			Console.SetCursorPosition(27, 13);
			Console.Write(new String(' ', SCREEN_WIDTH - 28));  // Clear previous value
			Console.SetCursorPosition(27, 13);
			Console.Write(Program.START_LEN);

			SettingsMenu.PrintFullScreen(SettingsPointer, 2, 9, 24, 9, true);   // Reprint only the first option so that the option gets highlighted and the pointer appears [coordinates (2, 9) to (24, 9)]
			do
			{
				// Allow user to scroll between options until they press ENTER
				do
				{
					cki = Console.ReadKey(true);
					switch (cki.Key)
					{
						case ConsoleKey.UpArrow:
							SettingsPointer.Move(2, SettingsMenu);
							break;
						case ConsoleKey.DownArrow:
							SettingsPointer.Move(-2, SettingsMenu);
							break;
						default:
							break;
					}
					SettingsMenu.PrintScreen(SettingsPointer, true);    // Second argument is true so that PrintScreen() keeps dark gray background
				} while (cki.Key != ConsoleKey.Enter);

				// See which option the user has highlighted, then move the cursor and receive the user's input for its new value
				switch (SettingsPointer.yPos)
				{
					case 9:     // Snake speed (i.e. refresh rate)
						Console.SetCursorPosition(27, 9);
						Console.Write('_' + new String(' ', SCREEN_WIDTH - 29));    // Add a '_' before all the spaces to show the user that they need to input something there
						Console.SetCursorPosition(27, 9);       // Set the cursor to the right of the option, clear the current value, and wait for user input
						switch (GetString(new List<string> { "SLOW", "MEDIUM", "FAST" }, 2, 6).ToUpper())   // Use GetString() method to receive and validate user input and then make it uppercase
						{
							case "SLOW":
								Program.REFRESH_RATE = Program.DEFAULT_REFRESH_RATE - 15;
								break;
							case "MEDIUM":
								Program.REFRESH_RATE = Program.DEFAULT_REFRESH_RATE;
								break;
							case "FAST":
								Program.REFRESH_RATE = Program.DEFAULT_REFRESH_RATE + 10;
								break;
							default:
								break;
						}
						break;
					case 11:    // Number of cones
						Console.SetCursorPosition(27, 11);
						Console.Write('_' + new String(' ', SCREEN_WIDTH - 29));    // Add a '_' before all the spaces to show the user that they need to input something there
						Console.SetCursorPosition(27, 11);  // Set the cursor to the right of the option, clear the current value, and wait for user input
						Program.NUM_CONES = GetInt(0, Program.MAX_NUM_CONES, 2, 6); // Use GetInt() method to receive and validate user input
						break;
					case 13:    // Starting length
						Console.SetCursorPosition(27, 13);
						Console.Write('_' + new String(' ', SCREEN_WIDTH - 29));    // Add a '_' before all the spaces to show the user that they need to input something there
						Console.SetCursorPosition(27, 13);  // Set the cursor to the right of the option, clear the current value, and wait for user input
						Program.START_LEN = GetInt(5, Program.MAX_START_LEN, 2, 6); // Use GetInt() method to receive and validate user input
						break;
					default:
						break;
				}
			} while (SettingsPointer.yPos != 15);

			Console.ResetColor();   // Reset the background colour to black (using ConsoleColor.Black doesn't seem to work - at least not in Repl.it)
		}

		// Plays the game itself
		public static int Game(int refreshRate, int numCones, int snakeLength)
		{
			// Set the game parameters based on the arguments
			Program.REFRESH_RATE = refreshRate;
			Program.NUM_CONES = numCones;
			Program.START_LEN = snakeLength;

			int pixelsToMove = 0;           // Number of pixels for the snake to move from a given frame to the next (initialized to zero to circumvent "unassigned variable" errors)
			int score = 0;                  // Final score = # of pieces of food eaten
											// Since the length increases by exactly one for each food: final score = final length - initial length of snake (+1 because the tail gets deleted during the final move)
			Random rand = new Random(); // Used to randomly generate the position of the cones
			int START_X_VAL = 5 + Program.START_LEN;
			int START_Y_VAL = SCREEN_HEIGHT / 2;

			// Initialize all relevant objects
			Screen GameScreen = new Screen(1, 2, SCREEN_WIDTH - 2, SCREEN_HEIGHT - 1, 'X');
			Snake snake = new Snake(START_X_VAL, START_Y_VAL, GameScreen);
			int yVal;   // Temporarily contains the ranomly generated y-coordinate for each cone's positions (need to check if this value is equal to starting y-coordinate)
						// Randomly place the required number of cones
			for (int i = 0; i < Program.NUM_CONES; i++)
			{
				do
				{
					yVal = rand.Next(3, SCREEN_HEIGHT - 2);
				} while (yVal == START_Y_VAL);  // Don't print cones on the starting line

				new Cone(rand.Next(2, SCREEN_WIDTH - 2), yVal, GameScreen);
			}
			Food food = new Food();
			food.Spawn(GameScreen, snake);

			GameScreen.PrintFullScreen(snake, food);

			// Initialize scoreboard
			Console.SetCursorPosition(0, 0);
			Console.BackgroundColor = ConsoleColor.DarkGray;    // Print dark gray background
			for (int y = 0; y < 2; y++)
			{
				for (int x = 0; x <= 22; x++)
				{
					Console.SetCursorPosition(x, y);
					Console.Write(' ');
				}
			}
			Console.ForegroundColor = ConsoleColor.White;
			Console.SetCursorPosition(0, 0);
			Console.WriteLine("SCORE: 0");
			Console.WriteLine("OLD HIGH SCORE: {0}", Program.highScore);

			// Print initial messages (but don't save them to the list)
			// First make space on screen by printng dark gray rectangle
			Console.BackgroundColor = ConsoleColor.DarkGray;
			for (int y = 11; y <= 15; y++)
			{
				for (int x = SCREEN_WIDTH / 2 - 34; x <= SCREEN_WIDTH / 2 + 34; x++)
				{
					Console.SetCursorPosition(x, y);
					Console.Write(' ');
				}
			}
			// Then print messages
			Console.SetCursorPosition(SCREEN_WIDTH / 2 - 12, 12);
			Console.Write("Press any key to start...");
			Console.SetCursorPosition(SCREEN_WIDTH / 2 - 32, 14);
			Console.Write("Once the game starts, press CTRL + X to return to the main menu");

			Console.ResetColor();   // Reset background colour to default
			Console.ReadKey(true);

			// Once the game starts, clear messages from the screen
			GameScreen.PrintFullScreen(snake, food, SCREEN_WIDTH / 2 - 34, 11, SCREEN_WIDTH / 2 + 34, 15);  // Print screen between (SCREEN_WIDTH/2 - 34, 11) and (SCREEN_WIDTH/2 + 34, 15)

			ConsoleKeyInfo cki = new ConsoleKeyInfo();      // Create ConsoleKeyInfo object to receive user keystrokes
			while (!snake.dead)
			{
				// If the user presses a key, update the snake's velocity 
				if (Console.KeyAvailable)
				{
					cki = Console.ReadKey(true);
					switch (cki.Key)
					{
						case ConsoleKey.UpArrow:        // If the user presses the up arrow or 'W', the snake moves up
						case ConsoleKey.W:
							snake.SetVelocity(0, yVelocity);
							break;
						case ConsoleKey.DownArrow:      // If the user presses the down arrow or 'S', the snake moves down
						case ConsoleKey.S:
							snake.SetVelocity(0, -yVelocity);
							break;
						case ConsoleKey.RightArrow:     // If the user presses the right arrow or 'D', the snake moves to the right
						case ConsoleKey.D:
							snake.SetVelocity(xVelocity, 0);
							break;
						case ConsoleKey.LeftArrow:      // If the use presses the left arrow or 'A', the snake moves to the left
						case ConsoleKey.A:
							snake.SetVelocity(-xVelocity, 0);
							break;
						case ConsoleKey.X:              // If the user presses CTRL + X, end the game
							if (cki.Modifiers == ConsoleModifiers.Control)
								snake.dead = true;
							snake.segmentList.RemoveAt(snake.segmentList.Count - 1);    // Delete the tail so that the score is displayed properly
							break;
						default:
							break;
					}
				}

				if (snake.v_x == 0)         // Moving vertically
					pixelsToMove = Math.Abs(snake.v_y);
				else if (snake.v_y == 0)    // Moving horizontally
					pixelsToMove = Math.Abs(snake.v_x);
				for (int i = 0; i < pixelsToMove; i++)
				{
					if (!snake.dead)
					{       // Avoid "index out of range" errors by stopping snake before it leaves screen
						snake.Move(GameScreen, food);
						food.Spawn(GameScreen, snake);
					}
				}

				GameScreen.PrintScreen(snake, food);    // Print the updated screen

				System.Threading.Thread.Sleep(1000 / REFRESH_RATE);     // Wait until it's time to print the next frame (assume the execution time for a single frame is negligible)
			}

			// Create a dark gray rectangle in the center of the screen for the GAME OVER message
			Console.BackgroundColor = ConsoleColor.DarkGray;
			for (int y = 11; y <= 17; y++)
			{
				for (int x = SCREEN_WIDTH / 2 - 23; x <= SCREEN_WIDTH / 2 + 23; x++)
				{
					Console.SetCursorPosition(x, y);
					Console.Write(' ');
				}
			}

			// Print GAME OVER message, display score, and prompt them to return to the main menu
			Console.SetCursorPosition(SCREEN_WIDTH / 2 - 5, 12);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("GAME  OVER");
			Console.ForegroundColor = ConsoleColor.White;

			System.Threading.Thread.Sleep(750);

			score = snake.segmentList.Count - Program.START_LEN + 1;
			if (score <= highScore)
			{
				Console.SetCursorPosition(SCREEN_WIDTH / 2 - 7, 14);
				Console.Write("Final Score: {0}", score);
			}
			else
			{
				Console.SetCursorPosition(SCREEN_WIDTH / 2 - 16, 14);
				Console.Write("Final Score: {0} (", score);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("NEW HIGH SCORE!");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(")");
			}

			System.Threading.Thread.Sleep(750);

			Console.SetCursorPosition(SCREEN_WIDTH / 2 - 19, 16);
			Console.Write("Press ENTER to return to the main menu");
			Console.ResetColor();
			while (true)
			{
				if (Console.ReadKey(true).Key == ConsoleKey.Enter)
					break;
			}

			// Return the current score if it's a new high score
			if (score > Program.highScore)
				return score;
			else
				return Program.highScore;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//		UTILITY FUNCTIONS
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Gets a string from the user. The string must be a member of the list allowedValues. If not, prints an error message at the specified coordinates and waits for a new input.
		public static string GetString(List<string> allowedValues, int errLeft, int errTop)
		{
			string userInput;
			int inputLeft = Console.CursorLeft, inputTop = Console.CursorTop;   // Save the initial cursor position so that it can return here later if necessary

			userInput = Console.ReadLine();
			while (!allowedValues.Contains(userInput.ToUpper()))
			{
				// Print error message in cyan
				Console.SetCursorPosition(errLeft, errTop);
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write("Error: invalid entry. The valid options are: ");
				for (int i = 0; i < allowedValues.Count; i++)
				{
					Console.Write("{0}, ", allowedValues[i]);
				}
				Console.SetCursorPosition(Console.CursorLeft - 2, Console.CursorTop);
				Console.Write(" "); // Clear the last comma

				// Return the cursor to the initial position and wait for new input
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition(inputLeft, inputTop);
				Console.Write('_' + new String(' ', SCREEN_WIDTH - inputLeft - 2));     // Clear the previous input and write '_' to show the user that they need to input something
				Console.SetCursorPosition(inputLeft, inputTop);
				userInput = Console.ReadLine();
			}

			// Clear error message
			Console.SetCursorPosition(errLeft, errTop);
			Console.Write(new String(' ', SCREEN_WIDTH - errLeft - 1));
			Console.SetCursorPosition(inputLeft, inputTop);

			return userInput;
		}

		// Gets an integer from the user. The integer must be greater than or equal to minVal and less than or equal to maxVal. If not, prints an error message at the specified coordinates and waits for a new input.
		public static int GetInt(int minVal, int maxVal, int errLeft, int errTop)
		{
			int userInput;
			int inputLeft = Console.CursorLeft, inputTop = Console.CursorTop;   // Save the initial cursor position so that it can return here later if necessary

			while (!int.TryParse(Console.ReadLine(), out userInput) || userInput < minVal || userInput > maxVal)
			{
				// Print error message in cyan
				Console.SetCursorPosition(errLeft, errTop);
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write("Error: invalid entry. The entry must be an integer between {0} and {1} (inclusive).", minVal, maxVal);

				// Return the cursor to the initial position and wait for new input
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition(inputLeft, inputTop);
				Console.Write('_' + new String(' ', SCREEN_WIDTH - inputLeft - 2));     // Clear the previous input and write '_' to show the user that they need to input something
				Console.SetCursorPosition(inputLeft, inputTop);
			}

			// Clear error message
			Console.SetCursorPosition(errLeft, errTop);
			Console.Write(new String(' ', SCREEN_WIDTH - errLeft - 1));
			Console.SetCursorPosition(inputLeft, inputTop);

			return userInput;
		}

		// Receives a number (formatted as an int) and returns the same number (formatted as a char).
		public static char IntToChar(int num)
		{
			return char.Parse(num.ToString());
		}

		// Receives a number (formatted as a char) and returns the same number (formatted as an int)
		public static int CharToInt(char c)
		{
			return int.Parse(c.ToString());
		}
	}
}