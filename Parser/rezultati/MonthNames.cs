using System.Globalization;

namespace yagg_vhf.Parser.rezultati
{
    internal class MonthNames : Dictionary<string, int>
    {
        public MonthNames()
        {
            for (int i = 1; i <= 12; i++)
            {
                string monthName = new DateTime(2010, i, 1)
                    .ToString("MMMM", CultureInfo.InvariantCulture);

                this[monthName.ToUpper()] = i;
            }
        }
    }
}
