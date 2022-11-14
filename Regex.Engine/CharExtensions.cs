namespace Regex.Engine
{
    public static class CharExtensions
    {
        public static bool IsNumber(this char c)
        {
            //return '0' <= c && c <= '9';
            return char.IsDigit(c);
        }

        public static bool IsSpace(this char? c)
        {
            return ' ' == c;
        }
    }
}
