using System;

namespace Solve_Sudoko     
{
    class Program
    {
        /* Algoritm is solved in three steps
        * 1. If there can only be one possible value assigned to the cell in question, that value will be given
        * 2. If there can only be one possible value on the given row, vertical/horizontal/3x3
        * 3. If the information from the three rows combined gives an incling to what values can not possible be in a given cell
        * 4. In case the puzzle has not been solved using the above methods the algroitm will brute force a solution
        */
        static bool debugMode = false;
        static void Main(string[] args)
        {
            Debug();

            string gameString = Input();

            string[,] gameLayout = Layout(new string[9, 9], gameString);

            string[,] solvedGame = Solve(gameLayout);

            string answerString = ConvertBackToString(solvedGame, false);

            Answer(answerString, solvedGame);
        }
        static void Debug()
        {
            Console.WriteLine("Want to debug?, j/n");
            if (Console.ReadLine() == "j")
            {
                debugMode = true; // If the user wants additional information
            }
        }
        /// <summary>
        /// Correctly collects the string for the game
        /// </summary>
        /// <returns></returns>
        static string Input()
        {
            Console.WriteLine("Type in the string of the Sudoku Game you want solved \nUse 0 for an empty cell. Should be 81 charachters");
            Console.WriteLine("Example input: 010020300004003050060000001005700060000800002070012000400005090000400805007000000");
            string tempInput = Console.ReadLine();
            while (tempInput.Length != 81)
            {
                Console.WriteLine("Incorect input, try again!");
                tempInput = Console.ReadLine();
            }
            return tempInput;
        }
        /// <summary>
        /// Converts the string into a two dimensional array, like the game
        /// </summary>
        /// <param name="Layout"></param>
        /// <param name="gameString"></param>
        /// <returns></returns>
        static string[,] Layout(string[,] layout, string gameString)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    layout[i, j] = gameString[i * 9 + j].ToString();
                }
            }
            PrintMatrix(layout, false); // Displayes the inputed soduko game

            return layout;
        }
        /// <summary>
        /// Solution to the soduko game
        /// </summary>
        /// <param name="Layout"></param>
        /// <returns></returns>
        static string[,] Solve(string[,] SolvedLayout)
        {
            // The actual work of solving the entire sudoku puzzle
            // All the additional information has been done
            // A two dimensional string array has been given - containing the grid

            // Information is given: Horizontally, Verticaly, In the minor 3x3 boxes

            // Replace 0:s with their potential replacements
            SolvedLayout = CurrentPossibleValues(SolvedLayout);

            if (debugMode)
            {
                PrintMatrix(SolvedLayout, false); // Display the progress - first level progress
                Console.ReadKey(); // While debugging
            }

            SolvedLayout = FillInGaps(SolvedLayout); // Where everything is solved ie solves the puzzle

            return SolvedLayout;
        }
        /// <summary>
        /// Where the main work is done, all the gaps are continously filled in
        /// After a number has been placed, the possible cell solutions is once again checked
        /// </summary>
        /// <param name="uppdatedMatrix"></param>
        /// <returns></returns>
        static string[,] FillInGaps(string[,] uppdatedMatrix)
        {
            // Can be the only thing in the cell, in which case it automatically becomes the input
            // It can be the only possible occurance in the 3x3, verticle 9, horizontal 9
            int previousLength = 1; // To determin wether the algoritm is stuck
            while (ConvertBackToString(uppdatedMatrix, false).Length != 81)
            {
                uppdatedMatrix = SolveLines(uppdatedMatrix);
                
                // Solves the first step - the instances where a line only contains one possible solution, 2

                if (ConvertBackToString(uppdatedMatrix, false).Length == previousLength) // Attempt more advanced solving tecniques, 3
                {
                    Console.WriteLine("Advanced solving tecniques required");
                    Console.ReadKey();
                    uppdatedMatrix = CheckForCombinedOccurances(uppdatedMatrix);
                    if (ConvertBackToString(uppdatedMatrix, false).Length == previousLength)
                    {
                        // If this step did not yeild results
                        uppdatedMatrix = BruteForceSolution(uppdatedMatrix); // Execute a brute force solution to the problem, since logic is not making progress
                    }
                    // Checks if values can be eliminated based of the overlapping from the three diffrent information sources
                }
                if (debugMode)
                {
                    PrintMatrix(uppdatedMatrix, false);
                    Console.ReadKey();
                }
                previousLength = ConvertBackToString(uppdatedMatrix, false).Length;
            }
            // Returns the finished matrix
            return uppdatedMatrix;
        }
        static string[,] CheckForCombinedOccurances(string[,] matrix)
        {
            // Check 1.1, 3
            string verticalRow;
            string horizontalRow;
            string threeByThree;
            for (int i = 0; i < 9; i++)
            {
                matrix = CurrentPossibleValues(matrix);
                PrintMatrix(matrix, false);
                verticalRow = FindRows(matrix, true, i);
                horizontalRow = FindRows(matrix, false, i);
                string twoValue = ThreeByThreeValue(i); // Convert the linear 0-8 for loop to a two dimensional cube value
                threeByThree = FindThreeByThree(matrix, int.Parse(twoValue.Remove(1)), int.Parse(twoValue.Substring(2)));
                Console.WriteLine(verticalRow);
                Console.WriteLine(horizontalRow);
                Console.WriteLine(threeByThree);
                Console.ReadLine();
            }
            return matrix;
        }
        static string[,] BruteForceSolution(string[,] matrix)
        {
            PrintMatrix(matrix, true); // Only display simple matrix upon error, ie no change
            Console.WriteLine("\nThe three first steps did not yeild sufficiant results\nBruteForce Initiated");
            Console.WriteLine(ConvertBackToString(matrix, true)); // Display the current game string, for debuging elsewhere
            Console.ReadKey();
            // Will continue to be worked on
            return matrix;
        }
        /// <summary>
        /// Converts the linear "for" function to a 2d value for each soduko 3x3
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        static string ThreeByThreeValue(int i)
        {
            string temp = "";
            switch (i)
            {
                case 0:
                    temp = "0,0";
                    break;
                case 1:
                    temp = "0,3";
                    break;
                case 2:
                    temp = "0,6";
                    break;
                case 3:
                    temp = "3,0";
                    break;
                case 4:
                    temp = "3,3";
                    break;
                case 5:
                    temp = "3,6";
                    break;
                case 6:
                    temp = "6,0";
                    break;
                case 7:
                    temp = "6,3";
                    break;
                case 8:
                    temp = "6,6";
                    break;
                default:
                    Console.WriteLine("ERROR"); // Remove maybe?
                    Console.ReadLine();
                    break;
            }
            return temp;
        }
        /// <summary>
        /// Combination for the checking of all the rows, 2
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        static string[,] SolveLines(string[,] matrix)
        {
            for (int i = 0; i < 9; i++) // Solves the lines
            {
                string possibleHorizontalNumbers = FindRows(matrix, false, i);
                matrix = CurrentPossibleValues(matrix);
                matrix = SimplyfyRows(matrix, possibleHorizontalNumbers, i, 0, 0); // Solves horizontal possibilites

                string possibleVerticalNumbers = FindRows(matrix, true, i);
                matrix = CurrentPossibleValues(matrix);
                matrix = SimplyfyRows(matrix, possibleVerticalNumbers, i, 0, 1); // Solves vertical posibilities

                string twoValue = ThreeByThreeValue(i);
                string threeByThree = FindThreeByThree(matrix, int.Parse(twoValue.Remove(1)), int.Parse(twoValue.Substring(2)));
                matrix = CurrentPossibleValues(matrix);
                matrix = SimplyfyRows(matrix, threeByThree, int.Parse(twoValue.Remove(1)), int.Parse(twoValue.Substring(2)), 3); // Solves the 3x3 possible solutions
            }
            return matrix;
        }
        /// <summary>
        /// Determins rows from the straight lines
        /// </summary>
        /// <param name="matrix">the game matrix</param>
        /// <param name="typeOfRow">wether it is a horizontal or vertical row</param>
        /// <param name="i"></param>
        /// <returns></returns>
        static string FindRows(string[,] matrix, bool typeOfRow, int i)
        {
            string row = "";
            for (int j = 0; j < 9; j++) // loops thorugh two entire lines
            {
                if (matrix[j, i].Length > 1 && typeOfRow) // Check vertical potential solutions
                {
                    row += matrix[j, i] + " ";
                }
                else if (matrix[i, j].Length > 1 && !typeOfRow) // Check horizontal potential solutions
                {
                    row += matrix[i, j] + " ";
                }
            }
            return row;
        }
        /// <summary>
        /// Determins a row of possible candidates in the 3x3
        /// </summary>
        /// <param name="matrix">game matrix</param>
        /// <param name="x">x coodrinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns></returns>
        static string FindThreeByThree(string[,] matrix, int x, int y)
        {
            string row = "";
            for (int s = x; s < x + 3; s++)
            {
                for (int g = y; g < y + 3; g++)
                {
                    if (matrix[s, g].Length > 1)
                    {
                        row += matrix[s, g] + " ";
                    }
                }
            }
            return row;
        }
        /// <summary>
        /// Solves the values for the cells by checking sole possible values from the given rows, 2
        /// </summary>
        /// <param name="matrix">the entire soduko game matrix</param>
        /// <param name="givenRow">the row which is examined</param>
        /// <param name="typeOfRow">what type of row that is being examined, horizontal, vertical or 3x3</param>
        /// <returns></returns>
        static string[,] SimplyfyRows(string[,] matrix, string row, int i, int threeByThree, int typeOfRow)
        {
            for (int j = 0; j < row.Length; j++) // Horizontal replacement
            {
                string tempValue = "";
                string tempRemainder = "";
                if (row[j] != ' ') // Is'nt empty
                {
                    tempValue = row[j].ToString(); // Assign the temporary value
                    tempRemainder = row.Remove(j) + row.Substring(j + 1);
                    // Creates a string with all the values exept the selected
                }
                if (!tempRemainder.Contains(tempValue))
                {
                    // if its unique the number needs to be added alone to the cell
                    // and the value needs to be removed from the other cells
                    int start = 0;
                    int end = 0;
                    for (int t = row.IndexOf(tempValue) - 1; t >= 0; t--)
                    {
                        if (row[t] == ' ')
                        {
                            start = t;
                            if (start != 0)
                            {
                                start += 1;
                            }
                            break;
                        }
                    }
                    for (int d = row.IndexOf(tempValue); d < row.Length; d++)
                    {
                        if (row[d] == ' ')
                        {
                            end = d;
                            break;
                        }
                    }
                    string toEliminate = row.Substring(start);
                    toEliminate = toEliminate.Remove(toEliminate.IndexOf(" ")); // Defines what is being replaced
                    row = row.Remove(start) + tempValue + row.Substring(end); // Not necessary, but to to display the row with the new value

                    string message = ""; // To display what is happening whilst debugging
                    if (typeOfRow == 0)
                    {
                        for (int e = 0; e < 9; e++)
                        {
                            if (matrix[i, e] == toEliminate)
                            {
                                matrix[i, e] = tempValue;
                                message = (i + 1) + "," + (e + 1) + ": Using the horizontal";
                            }
                        }
                    }
                    else if (typeOfRow == 1) // Vertical
                    {
                        for (int e = 0; e < 9; e++)
                        {
                            if (matrix[e, i] == toEliminate)
                            {
                                matrix[e, i] = tempValue;
                                message = (e + 1) + "," + (i + 1) + ": Using the vertical";
                            }
                        }
                    }
                    else // 3x3
                    {
                        for (int s = i; s < i + 3; s++)
                        {
                            for (int g = threeByThree; g < threeByThree + 3; g++)
                            {
                                if (matrix[s, g] == toEliminate)
                                {
                                    matrix[s, g] = tempValue;
                                    message = (s + 1) + "," + (g + 1) + ": Using the 3x3";
                                }
                            }
                        }
                    }
                    if (message.Length == 0)
                    {
                        Console.WriteLine(typeOfRow + " : " + row + "\n");
                        PrintMatrix(matrix, false); // In case of eror with displaying what was removed - ie the coordinates was not properly shown
                        Console.WriteLine();
                    }
                    if (debugMode)
                    {
                        Console.WriteLine("Check: " + message + ": " + toEliminate + " was replaced by: " + tempValue);
                        matrix = CurrentPossibleValues(matrix);
                    }
                }
            }
            return matrix;
        }
        /// <summary>
        /// Checks what values can be at each cell in the game - adds them
        /// </summary>
        /// <param name="possibleValues"></param>
        /// <returns></returns>
        static string[,] CurrentPossibleValues(string[,] possibleValues)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (possibleValues[i, j] == "0" || possibleValues[i, j].Length > 1)
                    {
                        possibleValues[i, j] = PossibleValues(possibleValues, i, j);
                    }
                }
            }
            return possibleValues;
        }
        /// <summary>
        /// Calculates what values can be in each given cell
        /// </summary>
        /// <param name="possibleValue"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        static string PossibleValues(string[,] possibleValue, int i, int j)
        {
            string valueAtGivenPlace;
            if (possibleValue[i, j] == "0")
            {
                valueAtGivenPlace = "123456789";
            }
            else
            {
                valueAtGivenPlace = possibleValue[i, j];
            }

            for (int a = 0; a < 9; a++)
            {
                if (valueAtGivenPlace.Contains(possibleValue[i, a])) // Checks the horizontal
                {
                    valueAtGivenPlace = RemoveNumbers(valueAtGivenPlace, possibleValue[i, a]);
                }
                if (valueAtGivenPlace.Contains(possibleValue[a, j])) // Checks the vertical
                {
                    valueAtGivenPlace = RemoveNumbers(valueAtGivenPlace, possibleValue[a, j]);
                }
            }
            // Below is the grind 3x3 values   
            int xCoordinate = VerticalBox(i);
            int yCoordinate = VerticalBox(j);
            for (int s = xCoordinate; s < xCoordinate + 3; s++)
            {
                for (int g = yCoordinate; g < yCoordinate + 3; g++)
                {
                    if (valueAtGivenPlace.Contains(possibleValue[s, g]))
                    {
                        valueAtGivenPlace = RemoveNumbers(valueAtGivenPlace, possibleValue[s, g]);
                    }
                }
            }
            if (valueAtGivenPlace.Length == 1 && debugMode)
            {
                Console.WriteLine($"Naked single cell at: {i}:{j}, {possibleValue[i, j]} was replaced by: {valueAtGivenPlace}");
            }
            return valueAtGivenPlace;
        }
        /// <summary>
        /// Determins the 3x3 start values
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        static int VerticalBox(int j)
        {
            if (j < 3)
            {
                j = 0;
            }
            else if (j < 6)
            {
                j = 3;
            }
            else // i < 9
            {
                j = 6;
            }
            return j;
        }
        /// <summary>
        /// Removes the number which can be found elswhere
        /// </summary>
        /// <param name="valueToBeModified"></param>
        /// <param name="thingToRemove"></param>
        /// <returns></returns>
        static string RemoveNumbers(string valueToBeModified, string thingToRemove)
        {
            string newString = "";
            foreach (char item in valueToBeModified)
            {
                if (item.ToString() == thingToRemove)
                {
                    // Do nothing
                }
                else
                {
                    newString += item;
                }
            }
            return newString;
        }
        /// <summary>
        /// Reverts back the compleded array to a solution string
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        static string ConvertBackToString(string[,] fullMatrix, bool Debug)
        {
            string completed = "";
            foreach (string item in fullMatrix)
            {
                if (Debug && item.Length > 1)
                {
                    completed += 0;
                }
                else
                {
                    completed += item;
                }
            }
            return completed;
        }
        /// <summary>
        /// Displays the entire matrix
        /// </summary>
        /// <param name="temp"></param>
        static void PrintMatrix(string[,] temp, bool simple)
        {
            Console.WriteLine(""); // Formatting
            string current = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (simple && temp[i, j].Length > 1)
                    {
                        current = "0";
                    }
                    else
                    {
                        current = temp[i, j];
                    }
                    if ((j+1) % 3 == 0 && j+1 != 9)
                    {
                        current += "|";
                    }
                    else
                    {
                        current += " ";
                    }
                    if (current.Length == 2 && current[0] != '0')
                    {
                        // If it is solved, one number and the space - display in green
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(current[0]);
                        current = current.Substring(1);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.Write(current);
                }
                Console.WriteLine();
                if ((i + 1) % 3 == 0 && i + 1 != 9)
                {
                    Console.WriteLine("-----------------");
                }                       
            }
            Console.WriteLine("");
        }
        /// <summary>
        /// Displays the answe, both string and matrix
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="finalArray"></param>
        static void Answer(string answer, string[,] finalArray)
        {
            Console.WriteLine("Your solution string is: " + answer);
            Console.WriteLine("Which looks like: ");
            PrintMatrix(finalArray, false);
            Console.WriteLine();
        }
    }
}