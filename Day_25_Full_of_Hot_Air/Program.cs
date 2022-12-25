// 08:14

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

void P1()
{
    // Verify conversion
    /*
    for (Int64 i = -10000; i <= 10000; i++)
    {
        string snafu = DecimalToSnafu(i);
        Int64 dec = SnafuToDecimal(snafu);
        if (dec != i)
            throw new Exception();
    }
    */

    List<Int64> decimalValues = inputList.Select(x => SnafuToDecimal(x)).ToList();
    Int64 sum = decimalValues.Sum();
    Console.WriteLine(DecimalToSnafu(sum));
    Console.ReadLine();
}

void P2()
{
    Console.WriteLine("Well done!");
    Console.ReadLine();
}

P1();
P2();

string DecimalToSnafu(Int64 d)
{
    // asssume +ve
    int maxPow;
    for (maxPow = 0; 2 * Math.Pow(5, maxPow) < Math.Abs(d); maxPow++) ;
    string snafu = "";

    Int64 remainder = d;
    for (int pow = maxPow; pow >= 0; pow--)
    {
        Int64 val = (Int64)Math.Pow(5, pow);

        int column = (int)(remainder / val); // integer division
        int altColumn;
        if (remainder < 0)
            altColumn = column - 1;
        else
            altColumn = column + 1;
        if (altColumn < -3 || altColumn > 3)
            throw new Exception();

        Int64 diff = Math.Abs(Math.Abs(remainder) - Math.Abs(column * val));
        Int64 altDiff = Math.Abs(Math.Abs(altColumn * val) - Math.Abs(remainder));

        if (altDiff < diff)
            column = altColumn;

        remainder = remainder - (column * val);
        switch (column)
        {
            case -2: snafu += "="; break;
            case -1: snafu += "-"; break;
            case 0: snafu += "0"; break;
            case 1: snafu += "1"; break;
            case 2: snafu += "2"; break;
            default: throw new Exception();
        }
    }
    return snafu;
}

Int64 SnafuToDecimal(string s)
{
    Int64 decimalVal = 0;
    for (int i = 0; i < s.Length; i++)
    {
        int pow = s.Length - 1 - i;
        int column;
        switch (s[i])
        {
            case '2': column = 2; break;
            case '1': column = 1; break;
            case '0': column = 0; break;
            case '-': column = -1; break;
            case '=': column = -2; break;
            default: throw new Exception();
        }
        decimalVal += column * (Int64)Math.Pow(5, pow);
    }
    return decimalVal;
}