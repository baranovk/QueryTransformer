namespace Regex.Engine.Rules
{
    public class GroupRule : PatternRule
    {
        #region Fields

        private readonly Pattern _pattern;

        #endregion

        #region Constructors

        public GroupRule(Pattern pattern)
        {
            _pattern = pattern;
        }

        #endregion

        #region Public Methods

        public override bool Match(IContext context)
        {
            if (!ValidateModificator(context, Rules.Modificators.FromStartOnly)) return false;

            while (true)
            {
                using (var checkpoint = context.CreateCheckpoint())
                {
                    // Try to match from current context position
                    var currentPosition = context.CurrentPosition;
                    var match = _pattern.FirstMatch(context);

                    if (match is not null && match.Position.Equals(currentPosition))
                    {
                        // We have a match!
                        // Commit match attempt and move to next input character
                        // to start next match attempt
                        _symbolSequenceLength++;
                        checkpoint.Commit();
                        context.Read(); // move to next character after last match
                        continue;
                    }
                }

                break;
            }

            // No more matches in current input - validate sequence length and modificators
            // and move to next pattern rule (if any)
            if (ValidateQuantity() && ValidateModificator(context, Rules.Modificators.AtEndOnly))
            {
                if (null == _nextRule || _nextRule.Match(context))
                {
                    Reset();
                    return true;
                }
            }

            return false; 
        }

        public override bool Match(char? character)
        {
            if(null == character) return false;
            var context = new Context(new char[] { '^', character.Value });
            return null != _pattern.FirstMatch(context);
        }

        #endregion
    }
}
