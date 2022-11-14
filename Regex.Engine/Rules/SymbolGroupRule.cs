namespace Regex.Engine.Rules
{
    public class SymbolGroupRule : PatternRule
    {
        #region Fields

        private readonly HashSet<char> _symbols = new();
        private SymbolGroupMode _mode = SymbolGroupMode.Inclusive;

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public void AddSymbol(char symbol)
        {
            _symbols.Add(symbol);
        }

        public void SetMode(SymbolGroupMode mode)
        {
            _mode = mode;
        }

        public override bool Match(char? character)
            => character.HasValue
            && (_mode == SymbolGroupMode.Inclusive ? _symbols.Contains(character.Value) : !_symbols.Contains(character.Value));

        #endregion
    }

    public enum SymbolGroupMode
    {
        Inclusive,
        Exclusive
    }
}
