using Regex.Engine;

namespace QueryTransformer
{
    public class RegexReplaceStep : IPipelineStep<char[], char[]>
    {
        #region Fields

        private readonly RegexTransformer _transformer;

        #endregion

        #region Constructors

        public RegexReplaceStep(string pattern, string replacement)
{
            _transformer = new RegexTransformer(pattern, replacement);
        }

        public RegexReplaceStep(string pattern, Func<string, string> replace)
{
            _transformer = new RegexTransformer(pattern, replace);
        }

        #endregion

        #region Public Methods

        public char[] Execute(char[] input)
        {
            var str = new string(input);
            var context = new Context(input);
            _transformer.Process(context);
            return context.Result;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
