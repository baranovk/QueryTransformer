namespace Regex.Engine
{
    public class Match
    {
        #region Constructors

        public Match(int position, char[] value)
        {
            Position = position;
            Value = value;
        }

        public Match(int position, char value)
        {
            Position = position;
            Value = new char[] { value };
        }

        #endregion

        #region Properties

        public int Position { get; private set; }

        public char[] Value { get; private set; }

        #endregion

        #region Public Methods

        public void Set(int position, char c)
        {
            // throw exception if Value.Length > 1
            Position = position;
            Value[0] = c;
        }

        public bool Equals(Match obj)
        {
            if (Position != obj.Position) return false;

            for (int i = 0; i < obj.Value.Length; i++)
            {
                if (i >= Value.Length || Value[i] != obj.Value[i]) return false;
            }

            return true;
        }

        #endregion

        #region Operators

        public static bool operator == (Match? x, Match? y)
        {
            if (x is null && y is null) return true;
            if (x is null && y is not null) return false;
            if (x is not null && y is null) return false;
            return x!.Equals(y!);
        }

        public static bool operator !=(Match? x, Match? y)
        {
            if (x is null && y is null) return false;
            if (x is null && y is not null) return true;
            if (x is not null && y is null) return true;
            return !x!.Equals(y!);
        }

        // TODO: override GetHashCode()

        #endregion
    }
}
