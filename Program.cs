using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace guassian_elimination_calculator
{
    class augmentedMatrix
    {
        public Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
        public string checkEq { get; set; }
        public float[,] matrix;
        public float[] values;

        // make a dimensions variable that can be used in the set zero 
        public int rowNum { get; set; }
        public int colNum { get; set; }

        public void initilise()
        {
            matrix = new float[rowNum, colNum];
            values = new float[rowNum];
        }
        public void run()
        {
            display();

            for (int i = 0; i < colNum; i++)
            {
                set_one(i, i);

                // check if there are any zeros to change 
                if (i + 1 != colNum)
                {
                    set_zero(i + 1, i);
                }
            }

            solve();
            check();
        }

        public void set_one(int x, int y)
        {
            float selectedElement = matrix[y, x];

            if (selectedElement != 1)
            {
                float multiplier = 1 / selectedElement;

                for (int i = 0; i < rowNum; i++)
                {
                    matrix[y, i] = matrix[y, i] * multiplier;
                }

                values[y] = values[y] * multiplier;
            }
        }

        public void set_zero(int row, int column)
        {
            // multiply the one with the negated version of the nxt
            // pos of the one in the correct column is in place a,a

            for (int i = rowNum - 1; i >= row; i--) // iterate down the column
            {
                float nextElement = matrix[i, column];
                float multiplier = 0 - nextElement;
                matrix[i, column] = matrix[i, column] + multiplier;

                for (int z = 0; z < colNum; z++) // iterate across the row
                {
                    float aboveVal = matrix[row - 1, z];
                    float newAdd = aboveVal * multiplier;

                    if (z == column)
                    {
                        continue;
                    }

                    matrix[i, z] = matrix[i, z] + newAdd;
                }

                values[i] = values[i] + (values[row - 1] * multiplier);
            }
        }

        public void solve()
        {
            // when in a form that has the final term = a value

            keyValuePairs[keyValuePairs.Keys.Last()] = values.Last();

            for (int i = rowNum - 2; i >= 0; i--)
            {
                float currentValue = values[i];
                string unknown = "";
                float unknownValue = 0;
                int pos = 0;

                for (int j = colNum - 1; j >= 0; j--) // j = 0 j < colnum
                {
                    if (matrix[i, j] != 0)
                    {
                        if (keyValuePairs.ElementAt(j).Value != -999)
                        {
                            currentValue += matrix[i, j] * (0 - keyValuePairs.ElementAt(j).Value);
                        }
                        else
                        {
                            unknown = keyValuePairs.ElementAt(j).Key;
                            unknownValue = matrix[i, j];
                            pos = j;
                        }
                    }
                }

                currentValue = currentValue / unknownValue;
                keyValuePairs[unknown] = currentValue;
            }
        }

        public void check()
        {
            var symbols = new List<char> { '=', '+', '-' };
            var listOfInts = new List<float>();
            var listOfOps = new List<char>();
            string output = "";
            string original = checkEq;

            foreach (char item in checkEq)
            {
                if (symbols.Contains(item))
                {
                    listOfOps.Add(item);

                    if (item == '=')
                    {
                        original = original.Substring(1, original.Length - 1);
                        listOfInts.Add(Convert.ToInt32(original));
                        break;
                    }

                    if (output != "")
                    {
                        try
                        {
                            listOfInts.Add(Convert.ToInt32(output));
                            output = "";
                        }
                        catch { }
                    }
                    original = original.Substring(output.Length + 1, original.Length - output.Length - 1);
                    continue;
                }

                string addition = item.ToString();

                if (keyValuePairs.Keys.Contains(addition))
                {
                    if (output != "")
                    {
                        listOfInts.Add(Convert.ToInt32(output));
                        listOfInts.Add(keyValuePairs[addition]);
                        output = "";
                        listOfOps.Add('x');
                    }
                    else
                    {
                        listOfInts.Add(keyValuePairs[addition]);
                    }
                    original = original.Substring(1, original.Length - 1);

                    continue;
                }

                output += addition;
                original = original.Substring(1, original.Length - 1);
            }
            listOfInts.RemoveAt(listOfInts.Count - 1);

            // evaluating the equation to give an answer

            float finalAnswer = Convert.ToInt32(checkEq.Substring(checkEq.IndexOf('=') + 1, checkEq.Length - 1 - checkEq.IndexOf('=')));

            for (int i = 0; i < listOfOps.Count; i++)
            {
                if (listOfOps[i] == 'x')
                {
                    listOfInts[i] = listOfInts[i] * listOfInts[i + 1];
                    listOfInts.RemoveAt(i + 1);
                    listOfOps.RemoveAt(i);
                    i--;
                }
            }

            while (listOfOps.Count > 0)
            {
                float results = 0;

                switch (listOfOps[0])
                {
                    case '+':
                        results = listOfInts[0] + listOfInts[1];
                        break;
                    case '-':
                        results = listOfInts[0] - listOfInts[1];
                        break;
                    case '=':
                        if (Math.Round(listOfInts[0], 2) == Math.Round(finalAnswer, 2))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Valid solutions found\n");
                            Console.ResetColor();
                            return;
                        }
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No solutions calculated");
                        Console.ResetColor();
                        return;
                }

                listOfInts[0] = results;
                listOfInts.RemoveAt(1);
                listOfOps.RemoveAt(0);
            }
        }

        public void display()
        {   
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    int lengthItem = Math.Round(matrix[i, j], 1).ToString().Length;

                    Console.Write(Math.Round(matrix[i, j], 1));

                    for (int z = 0; z < 7 - lengthItem; z++)
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('|');
                Console.Write(values[i]);
                Console.WriteLine();
            }
            Console.CursorTop++;
        }

        public void display_answers()
        {
            foreach (string item in keyValuePairs.Keys)
            {
                Console.WriteLine($"{item} = {keyValuePairs[item]}");
            }
        }
    }


    internal class Program
    {
        static void find(augmentedMatrix M1, string equation, int eqNum)
        {
            string pattern = @"-?\d*[a-zA-Z]+|-?\d+";

            MatchCollection matchPattern = Regex.Matches(equation, pattern);

            int counter = 0;
            foreach (Match match in matchPattern)
            {
                string numString = "";
                float result = 0;
                bool negative = false;

                bool num = float.TryParse(match.Value[0].ToString(), out result);

                if (match.Value[0] == '-')
                {
                    numString += "-";
                    num = float.TryParse(match.Value[1].ToString(), out result);
                    negative = true;
                }

                // check for the coefficient
                if (num && counter != matchPattern.Count - 1)
                {
                    numString += result.ToString();
                    int i = 1;

                    if (negative)
                    {
                        i = 2;
                    }
                    while (true)
                    {
                        bool checkNext = float.TryParse(match.Value[i].ToString(), out result);
                        if (!checkNext)
                        {
                            if (numString == "-" || numString == "−")
                            {
                                numString += "1";
                            }
                            break;
                        }

                        numString += result.ToString();
                        i++;
                    }

                    float numToAdd = float.Parse(numString);
                    List<string> values = M1.keyValuePairs.Keys.ToList();
                    string unknown = match.Value;
                    int index = values.IndexOf(unknown.Substring(unknown.Length - 1, 1));
                    if (index == -1)
                    {
                        index = counter;
                    }
                    M1.matrix[eqNum - 1, index] = numToAdd;

                    string variable = match.Value.Substring(i, match.Value.Length - i);
                    if (negative)
                    {
                        variable = match.Value.Substring(i, match.Value.Length - i);
                    }

                    if (!M1.keyValuePairs.ContainsKey(variable))
                    {
                        M1.keyValuePairs.Add(variable, -999);
                    }
                }
                else if (counter == matchPattern.Count - 1)
                {
                    M1.values[eqNum - 1] = float.Parse(match.Value);
                }
                else
                {
                    string variable = match.Value;
                    List<string> values = M1.keyValuePairs.Keys.ToList();
                    int index = values.IndexOf(variable.Substring(variable.Length-1,1));
                    if (index == -1)
                    {
                        index = counter;
                    }

                    if (negative)
                    {
                        M1.matrix[eqNum - 1, index] = -1;
                    }
                    else
                    {
                        M1.matrix[eqNum - 1, index] = 1;
                    }

                    if (negative)
                    {
                        variable = variable.Substring(1, variable.Length - 1);
                    }
                    if (!M1.keyValuePairs.ContainsKey(variable))
                    {
                        M1.keyValuePairs.Add(variable, -999);
                    }
                }
                counter++;
            }
        }
        static void Main(string[] args)
        {
            augmentedMatrix M1 = new augmentedMatrix();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Enter number of unknowns: ");
            Console.ResetColor();

            M1.rowNum = int.Parse(Console.ReadLine());
            M1.colNum = M1.rowNum;
            M1.initilise();


            Console.WriteLine("\nWould you like to\nRead from text file (0)\nEnter from console (1)");
            int option = int.Parse(Console.ReadLine());

            if (option == 0)
            {
                Console.WriteLine("\nEnter the file path (0) or use default file 'entereq' located in debug (1) ?");
                int fileOption = int.Parse(Console.ReadLine());

                string file = "entereq.txt";
                if (fileOption == 0)
                {
                    Console.WriteLine("Enter path");
                    file = Console.ReadLine();
                }


                using (StreamReader read = new StreamReader(file))
                {
                    int counter = 1;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nReading equations\n");
                    Console.ResetColor();
                    while (!read.EndOfStream)
                    {
                        string equations = read.ReadLine();
                        equations = equations.Replace("−", "-");
                        equations = equations.Replace(" ", string.Empty);

                        if (counter == 1)
                        {
                            M1.checkEq = equations;
                        }

                        Console.WriteLine($"Eq {counter}: {equations}");

                        find(M1, equations, counter);
                        counter++;
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nInput the equations row by row\n");
                Console.ResetColor();
                // build the functionality to input simulaneous equations

                int counter = 1;
                for (int i = 0; i < M1.rowNum; i++)
                {
                    Console.Write($"Eq {counter}: ");
                    string equation = Console.ReadLine().ToLower();
                    equation = equation.Replace("−", "-");
                    equation = equation.Replace(" ", string.Empty);

                    if (counter == 1)
                    {
                        M1.checkEq = equation;
                    }

                    find(M1, equation, counter);
                    counter++;
                }
                Console.WriteLine();
            }

            M1.run();
            M1.display();
            M1.display_answers();


            Console.ReadKey();
        }
    }
}
