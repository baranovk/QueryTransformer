
namespace Regex.Engine.Rules
{
    public abstract class PatternRule
    {
        #region Fields

        protected PatternRule? _nextRule;
        protected int _symbolSequenceLength;

        #endregion

        #region Properties

        public virtual Quantificator Quantity { get; set; }

        public Modificators? Modificators { get; set; }

        #endregion

        #region Public Methods

        public virtual bool Match(IContext context)
        {
            if(!ValidateModificator(context, Rules.Modificators.FromStartOnly)) return false;

            if (!Match(context.CurrentChar))
            {
                // If current pattern rule is optional and it doesn't match:
                // - if it's a last rule in a chain - success, we have a match
                // - else - move to the next rule in a chain 
                return IsCurrentRuleOptional() && (null == _nextRule || (null != _nextRule && _nextRule.Match(context)));
            }

            // We have a match for current character:
            // - increment sequence length for current pattern rule
            // - confirm match
            _symbolSequenceLength++;
            context.ConfirmMatch();

            // We need to move forward through input characters trying to match whole pattern rule
            // but we can failure somewhere in the middle.
            // For such case we need to create a checkpoint to have a possibility to return context
            // pointer inside input string to init position.
            using (var checkpoint = context.CreateCheckpoint())
            {
                // iterate through input characters while we can match with current pattern rule
                while (context.CanRead() && Match(context.NextChar) && Quantity.Satisfy(++_symbolSequenceLength))
                {
                    context.Read();
                    context.ConfirmMatch();
                }

                // No more input characters can be matched - we need to validate matched characters sequence 
                // length against pattern rule quantificator and validate if match is at the end of the input
                // and pattern rule has "AtEndOnly" modificator
                if (ValidateQuantity() && ValidateModificator(context, Rules.Modificators.AtEndOnly))
                {
                    // If this rule is last rule in the chain - we have a match
                    if (null == _nextRule)
                    {
                        checkpoint.Commit();
                        Reset();
                        return true;
                    }

                    var nextRuleMatch = false;
                    
                    using (var innerCheckpoint = context.CreateCheckpoint())
                    {
                        // If we have any rules left in the chain - we need to move context
                        // pointer to the next character (after last matched) to start matching
                        // process for next rule.
                        // But we can meet the situation in which there are only optional rules ahead
                        // so they can pass next character through the chain even if they can not match it.
                        // And we need to be sure that there was a rule ahead in the chain that matched next
                        // character. Otherwise we need to return context pointer inside input string to
                        // last matched position (before we call context.Read()) and move to next pattern rule
                        // (or pattern rule chain) if any
                        // Example:
                        // pattern = @"\(("".+""\,*\s*)+\)"
                        // input = "("123", "555")"
                        // Every parameter in braces has optional ',' and ' ' characters after it
                        // "123" has these characters ahead, but "555" doesn't
                        // So, when we match "555" we move to next character (')') and need to check it
                        // with subsequent rules. Rules for optional ',' and ' ' don't match it, so we 
                        // need to return context pointer to character '"' and pass context to next rule in 
                        // a chain (last rule for \) character) 
                        context.Read();
                        var lastCharMatchPosition = context.LastCharMatch?.Position;

                        if (_nextRule.Match(context))
                        {
                            if (lastCharMatchPosition != context.LastCharMatch?.Position) 
                            { 
                                // we have a match with some non-optional rule ahead of current rule
                                // confirm match attempts
                                innerCheckpoint.Commit(); 
                            }

                            nextRuleMatch = true;                            
                        }
                    }

                    if (nextRuleMatch)
                    {
                        checkpoint.Commit();
                        Reset();
                        return true;
                    }
                }
            }

            Reset();
            return false;
        }

        public void ChainWith(PatternRule nextRule)
        {
            _nextRule = nextRule;
        }

        public abstract bool Match(char? character);

        #endregion

        #region Protected Methods        

        protected virtual bool ValidateQuantity() => Quantity.Validate(_symbolSequenceLength);

        protected virtual void Reset()
        {
            _symbolSequenceLength = 0;
        }

        protected bool ValidateModificator(IContext context, Modificators modificator)
        {
            if (!Modificators.HasValue || !Modificators.Value.HasFlag(modificator)) return true;

            switch (modificator)
            {
                case Rules.Modificators.FromStartOnly:
                    return context.IsFirstChar;
                case Rules.Modificators.AtEndOnly:
                    return null == _nextRule && !context.CanRead();
            };

            return true;
        }

        protected bool IsCurrentRuleOptional()
        {
            return 0 == Quantity.From || (!Quantity.From.HasValue && 0 == Quantity.To);
        }

        #endregion
    }
}
