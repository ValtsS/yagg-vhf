using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace yagg_vhf.Contract
{
    internal static class Transfomer
    {
        public static string GetFirstWord(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            for (int i = 0; i < input.Length;i ++)
                if (char.IsWhiteSpace(input[i]))
                    return input.Substring(0, i).Trim();

            return input.TrimEnd();
        }
    }
}
