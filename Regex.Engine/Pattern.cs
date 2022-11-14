using System.Collections;
using Regex.Engine.Rules;

namespace Regex.Engine
{
    public class Pattern : IEnumerable<PatternRule>
    {
        #region Fields

        private readonly LinkedList<PatternRule> _rules = new();

        #endregion

        #region Public Methods

        public void Add(PatternRule rule)
        {
            if (_rules.Any())
            {
                _rules.Last().ChainWith(rule);
            }

            _rules.AddLast(rule);
        }

        public void ReplaceMatches(IContext context, string replacement)
        {
            if (null == _rules.First) return;

            while (context.Read())
            {
                if (_rules.First.Value.Match(context))
                {
                    foreach (var c in replacement)
                    {
                        context.UpdateResult(c);
                    }
                }
                else
                {
                    context.UpdateResult(context.CurrentChar!.Value);
                }
            }
        }

        public void ReplaceMatches(IContext context, Func<string, string> replace)
        {
            if (null == _rules.First) return;
            var ctxWrapper = new ContextWrapper(context);

            while (ctxWrapper.Read())
            {
                if (_rules.First.Value.Match(ctxWrapper))
                {
                    // context wrapper remembers possible match sequence in internal buffer
                    // if we have a match - we can read it from wrapper's buffer and place it
                    // to result buffer
                    var match = ctxWrapper.ReadLastMatch();
                    var replacement = replace(new string(match)) ?? string.Empty;

                    foreach (var c in replacement)
                    {
                        ctxWrapper.UpdateResult(c);
                    }
                }
                else
                {
                    // context wrapper remembers possible match sequence in internal buffer
                    // if match didn't happened - need to clear buffer for next possible match
                    ctxWrapper.DropMatchMemo();
                    ctxWrapper.UpdateResult(ctxWrapper.CurrentChar!.Value);
                }
            }
        }

        public Match? FirstMatch(IContext context)
        {
            if (null == _rules.First) return null;
            if (!context.CurrentChar.HasValue && !context.Read()) return null;
        
            var ctxWrapper = new ContextWrapper(context);
            int matchPosition;

            do
            {
                matchPosition = ctxWrapper.CurrentPosition;

                if (_rules.First.Value.Match(ctxWrapper))
                {
                    return new Match(matchPosition, ctxWrapper.ReadLastMatch());
                }
                else
                {
                    ctxWrapper.DropMatchMemo();
                }
            } 
            while (ctxWrapper.Read());

            return null;
        }

        public IEnumerator<PatternRule> GetEnumerator() => _rules.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _rules.GetEnumerator();

        #endregion
    }
}
