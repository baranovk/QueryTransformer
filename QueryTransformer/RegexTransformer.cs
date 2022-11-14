using Regex.Engine;

namespace QueryTransformer
{
    public class RegexTransformer : TextTransformer
    {
        #region Fields

        private readonly Pattern _pattern;
        private readonly string? _replacement;
        private readonly Func<string, string>? _replace;

        #endregion

        #region Constructors

        public RegexTransformer(string pattern, string replacement)
        {
            _pattern = PatternParser.Parse(pattern);
            _replacement = replacement;
        }

        public RegexTransformer(string pattern, Func<string, string> replace)
        {
            _pattern = PatternParser.Parse(pattern);
            _replace = replace;
        }

        public override void Process(IContext context)
        {
            if (null != _replace)
            {
                _pattern.ReplaceMatches(context, _replace);
            }
            else
            {
                _pattern.ReplaceMatches(context, _replacement ?? string.Empty);
            }
        }

        #endregion
    }
}
