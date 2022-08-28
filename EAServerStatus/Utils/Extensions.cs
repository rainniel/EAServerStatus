namespace EAServerStatus.Utils
{
    public static class Extensions
    {
        public static int ToInt(this string str)
        {
            str = str.Replace(",", "").Replace(".", "");
            if (int.TryParse(str, out int result))
                return result;
            return 0;
        }

        public static string GetSplitIndex(this string str, char delimeter, int index)
        {
            if (!string.IsNullOrEmpty(str) && index >= 0)
            {
                var text = str.Split(delimeter);
                return index < text.Length ? text[index] : "";
            }
            return "";
        }
    }
}
