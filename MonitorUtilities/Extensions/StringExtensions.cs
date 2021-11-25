namespace MonitorUtilities.Extensions
{
    public static class StringExtensions
    {
        public static string SubstringBetweenBoth(this string str, char symbol)
        {
            int startIndex = str.IndexOf(symbol) + 1;
            int length = str.LastIndexOf(symbol) - startIndex;
            return str.Substring(startIndex, length);
        }
    }
}
