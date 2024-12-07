using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using yagg_vhf;

internal static class ResultsParserHelpers
{

    public static string GetNLines(string text, int count)
    {
        var sb = new StringBuilder();
        using (var sr = new StringReader(text))
        {
            while (--count >= 0)
                sb.AppendLine(sr.ReadLine());
        }
        return sb.ToString();
    }


    public static bool IsSplitter(string line, int minLength)
    {
        if (line.Length < 0)
            return false;
        foreach (var ch in line)
            if (ch != '=')
                return false;

        return line.Length >= minLength;
    }
}