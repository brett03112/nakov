﻿using System;
using System.Text;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SimpleIO;

/*
Class Overview: The SimpleIO class provides a set of static methods for reading input from the 
console and parsing it into various data types.

Method List:

    **NextToken():** Reads a token from the console input, skipping any leading and trailing whitespace.

    **NextInt():** Reads an integer from the console, skipping any leading and trailing whitespace.

    **NextDouble(bool acceptAnyDecimalSeparator):** Reads a double from the console, skipping any 
    leading and trailing whitespace. Accepts both '.' and ',' as decimal separators if 
    acceptAnyDecimalSeparator is true.

    **NextDecimal(bool acceptAnyDecimalSeparator):** Reads a decimal from the console, skipping any 
    leading and trailing whitespace. Accepts both '.' and ',' as decimal separators if 
    acceptAnyDecimalSeparator is true.

    **NextString():** Reads a quoted string from the console, supporting escaped quotes within the string.

    Create1DList(): Reads space-separated numbers from the console and returns them as a list 
    of decimals.

    **Create2DList():** Creates a 2D list by reading numbers from the console row by row, separated by 
    Enter key.

    **Create3DList():** Creates a 3D list by reading numbers from the console matrix by matrix, separated
    by two consecutive Enter keys.

    **Note that the class has several TODO comments indicating that additional methods are planned for 
    future releases, such as reading from various sources, writing to files and streams, and 
    performing mathematical and string operations.
*/
public class SimpleIO
{
    /// <summary>
    /// Reads a token from the console input, skipping any leading
    /// and trailing whitespace.
    /// </summary>
    /// <returns>The token read from the console.</returns>
    public static string NextToken()
    {
        StringBuilder tokenChars = new StringBuilder();
        bool tokenFinished = false;
        bool skipWhiteSpaceMode = true;
        while (!tokenFinished)
        {
            int nextChar = Console.Read();
            if (nextChar == -1)
            {
                // End of stream reached
                tokenFinished = true;
            }
            else
            {
                char ch = (char)nextChar;
                if (char.IsWhiteSpace(ch))
                {
                    // Whitespace reached (' ', '\r', '\n', '\t') -->
                    // skip it if it is a leading whitespace
                    // or stop reading anymore if it is trailing
                    if (!skipWhiteSpaceMode)
                    {
                        tokenFinished = true;
                        if (ch == '\r' && (Environment.NewLine == "\r\n"))
                        {
                            // Reached '\r' in Windows --> skip the next '\n'
                            Console.Read();
                        }
                    }
                }
                else
                {
                    // Character reached --> append it to the output
                    skipWhiteSpaceMode = false;
                    tokenChars.Append(ch);
                }
            }
        }

        string token = tokenChars.ToString();
        return token;
    }


    /// <summary>
    /// Reads an integer from the console, skipping any leading
    /// and trailing whitespace.
    /// </summary>
    /// <returns>The integer read from the console.</returns>
    public static int NextInt()
    {
        string token = SimpleIO.NextToken();
        return int.Parse(token);
    }


    /// <summary>
    /// Reads a double from the console, skipping any leading
    /// and trailing whitespace. If <paramref name="acceptAnyDecimalSeparator"/>
    /// is <c>true</c>, both '.' and ',' are accepted as decimal separators.
    /// </summary>
    /// <param name="acceptAnyDecimalSeparator">
    /// Specifies whether to accept any decimal separator
    /// ("." and ",") or the system's default separator only.
    /// </param>
    /// <returns>The double read from the console.</returns>
    public static double NextDouble(bool acceptAnyDecimalSeparator = true)
    {
        string token = SimpleIO.NextToken();
        if (acceptAnyDecimalSeparator)
        {
            token = token.Replace(',', '.');
            double result = double.Parse(token, CultureInfo.InvariantCulture);
            return result;
        }
        else
        {
            double result = double.Parse(token);
            return result;
        }
    }


    /// <summary>
    /// Reads a decimal from the console, skipping any leading
    /// and trailing whitespace. If <paramref name="acceptAnyDecimalSeparator"/>
    /// is <c>true</c>, both '.' and ',' are accepted as decimal separators.
    /// </summary>
    /// <param name="acceptAnyDecimalSeparator">
    /// Specifies whether to accept any decimal separator
    /// ("." and ",") or the system's default separator only.
    /// </param>
    /// <returns>The decimal read from the console.</returns>
    public static decimal NextDecimal(bool acceptAnyDecimalSeparator = true)
    {
        string token = SimpleIO.NextToken();
        if (acceptAnyDecimalSeparator)
        {
            token = token.Replace(',', '.');
            decimal result = decimal.Parse(token, CultureInfo.InvariantCulture);
            return result;
        }
        else
        {
            decimal result = decimal.Parse(token);
            return result;
        }
    }

    /// <summary>
    /// Reads a quoted string from the console. The string must begin and end with quotation marks (").
    /// Supports escaped quotes within the string using backslash (\").
    /// </summary>
    /// <returns>The string read from the console without the surrounding quotes.</returns>
    /// <exception cref="FormatException">Thrown when the input is not properly quoted.</exception>
    public static string NextString()
    {
        StringBuilder result = new StringBuilder();
        bool foundOpenQuote = false;
        bool escape = false;

        // Skip whitespace and find opening quote
        while (!foundOpenQuote)
        {
            int nextChar = Console.Read();
            if (nextChar == -1)
                throw new FormatException("End of input reached before finding opening quote");

            char ch = (char)nextChar;
            if (ch == '"')
                foundOpenQuote = true;
            else if (!char.IsWhiteSpace(ch))
                throw new FormatException("Expected opening quote");
        }

        // Read until closing quote
        while (true)
        {
            int nextChar = Console.Read();
            if (nextChar == -1)
                throw new FormatException("End of input reached before finding closing quote");

            char ch = (char)nextChar;

            if (escape)
            {
                result.Append(ch);
                escape = false;
                continue;
            }

            if (ch == '\\')
            {
                escape = true;
                continue;
            }

            if (ch == '"')
                break;

            result.Append(ch);
        }

        return result.ToString();
    }

    /// <summary>
    /// Reads space-separated numbers from the console and returns them as a list.
    /// Input ends when two consecutive spaces are detected.
    /// Supports various numeric types (decimal, int, long, short, double, float).
    /// </summary>
    /// <returns>A list containing the numbers read from the console.</returns>
    public static List<decimal> Create1DList()
    {
        var numbers = new List<decimal>();
        StringBuilder currentNumber = new StringBuilder();
        bool lastWasSpace = false;

        while (true)
        {
            int nextChar = Console.Read();
            if (nextChar == -1)
                break;

            char ch = (char)nextChar;

            if (char.IsWhiteSpace(ch))
            {
                if (currentNumber.Length > 0)
                {
                    string numStr = currentNumber.ToString().Replace(',', '.');
                    numbers.Add(decimal.Parse(numStr, CultureInfo.InvariantCulture));
                    currentNumber.Clear();
                }

                if (lastWasSpace && ch == ' ')
                    break;

                lastWasSpace = (ch == ' ');
            }
            else
            {
                lastWasSpace = false;
                currentNumber.Append(ch);
            }
        }

        // Add the last number if there is one
        if (currentNumber.Length > 0)
        {
            string numStr = currentNumber.ToString().Replace(',', '.');
            numbers.Add(decimal.Parse(numStr, CultureInfo.InvariantCulture));
        }

        return numbers;
    }

    /// <summary>
    /// Creates a 2D list by reading numbers from the console row by row.
    /// Each row is space-separated numbers, and rows are separated by Enter key.
    /// Input ends when two consecutive Enter keys are pressed.
    /// </summary>
    /// <returns>A list of lists containing the numbers read from the console.</returns>
    public static List<List<decimal>> Create2DList()
    {
        var result = new List<List<decimal>>();
        bool lastWasEnter = false;
        var buffer = new List<byte>();

        while (true)
        {
            // Check for Enter key
            int nextChar = Console.Read();
            if (nextChar == '\r' || nextChar == '\n')
            {
                if (lastWasEnter)
                    break;

                lastWasEnter = true;

                // Skip \n if we just read \r in Windows
                if (nextChar == '\r' && Environment.NewLine == "\r\n")
                    Console.Read();

                continue;
            }

            // Put back the non-Enter character we just read
            buffer.Clear();
            buffer.Add((byte)nextChar);
            var ms = new MemoryStream(buffer.ToArray());
            Console.SetIn(new StreamReader(ms));

            // Read a row using Create1DList
            var row = Create1DList();
            if (row.Count > 0)
            {
                result.Add(row);
                lastWasEnter = false;
            }
        }

        return result;
    }

    /// <summary>
    /// Creates a 3D list by reading numbers from the console matrix by matrix.
    /// Each row is space-separated numbers, rows are separated by Enter key,
    /// and matrices are separated by two consecutive Enter keys.
    /// Input ends when three consecutive Enter keys are pressed.
    /// </summary>
    /// <returns>A list of lists of lists containing the numbers read from the console.</returns>
    public static List<List<List<decimal>>> Create3DList()
    {
        var result = new List<List<List<decimal>>>();
        var currentMatrix = new List<List<decimal>>();
        int consecutiveEnters = 0;
        var buffer = new List<byte>();

        while (true)
        {
            // Check for Enter key
            int nextChar = Console.Read();
            if (nextChar == '\r' || nextChar == '\n')
            {
                consecutiveEnters++;

                // Skip \n if we just read \r in Windows
                if (nextChar == '\r' && Environment.NewLine == "\r\n")
                    Console.Read();

                if (consecutiveEnters == 3)
                    break;

                if (consecutiveEnters == 2 && currentMatrix.Count > 0)
                {
                    result.Add(new List<List<decimal>>(currentMatrix));
                    currentMatrix.Clear();
                }

                continue;
            }

            // Put back the non-Enter character we just read
            buffer.Clear();
            buffer.Add((byte)nextChar);
            var ms = new MemoryStream(buffer.ToArray());
            Console.SetIn(new StreamReader(ms));

            // Read a row using Create1DList
            var row = Create1DList();
            if (row.Count > 0)
            {
                currentMatrix.Add(row);
                consecutiveEnters = 0;
            }
        }

        // Add the last matrix if there is one
        if (currentMatrix.Count > 0)
        {
            result.Add(new List<List<decimal>>(currentMatrix));
        }

        return result;
    }

    //TODO: Add more methods for reading arrays, jagged arrays, etc.
    //TODO: Add methods for reading files, writing to files, etc.
    //TODO: Add methods for reading from streams, writing to streams, etc.
    //TODO: Add methods for reading from sockets, writing to sockets, etc.
    //TODO: Add methods for reading from databases, writing to databases, etc.
    //TODO: Add methods for reading from APIs, writing to APIs, etc.
    //TODO: Add methods for reading from user input, writing to a user output file, etc.
    //TODO: Add methods for reading from a user input stream, writing to a user output stream, etc.
    //TODO: Add methods for reading from a user input socket, writing to a user output socket, etc.
    //TODO: Add methods for reading from a user input database, writing to a user output database, etc.
    //TODO: Add methods for reading from a user input API, writing to a user output API, etc.
    //TODO: Add various math functions for linear algebra, statistics, numerical integration, differentiation and optimization, etc.
    //TODO: Add various string functions for text processing, regular expressions, etc.
    //TODO: Add various date and time functions for working with dates and times, etc.
    //TODO: Add various graphics functions for drawing and rendering graphics, etc.
    //TODO: Add various audio functions for playing and recording audio, etc.
    //TODO: Add various video functions for playing and recording video, etc.
}
