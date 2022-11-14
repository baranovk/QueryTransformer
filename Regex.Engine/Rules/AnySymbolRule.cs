namespace Regex.Engine.Rules
{
    public class AnySymbolRule : PatternRule
    {
        #region Public Methods

        public override bool Match(char? character)
        {
            if (Quantity.From.HasValue
                && 0 < Quantity.From.Value
                && _symbolSequenceLength < Quantity.From.Value)
            {
                return true;
            }

            return null == _nextRule || !_nextRule.Match(character);
        }

        #endregion
    }
}
