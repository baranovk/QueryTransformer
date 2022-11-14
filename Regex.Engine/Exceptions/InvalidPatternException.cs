namespace Regex.Engine.Exceptions
{
    public class InvalidPatternException : Exception
    {
        #region Constructors

        public InvalidPatternException(int position, string template)
            : base($"Invalid template \"{template}\" - starting from position {position}")
        {

        }

        #endregion
    }
}
