using Regex.Engine;

namespace QueryTransformer.UnitTests
{
    public partial class RegexReplaceTests
    {
        #region Public Methods

        [Test]
        public void RegexTransformer_ShouldReplaceGroupPattern()
        {
            var template = @"\(\s*("".+"")\s*\)";
            var input = "(\"123\" )";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, "(...)");

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("(...)"));
        }

        [Test]
        public void RegexTransformer_ShouldReplaceMultipleGroupPatterns()
        {
            var template = @"(\("".+""\))+";
            var input = "(\"123\")(\"555\")";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, "(...)");

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("(...)"));
        }

        [Test]
        public void RegexTransformer_ShouldReplaceParametersInBracesPattern()
        {
            var template = @"\(("".+""\,*\s*)+\)";
            var input = "(\"123\", \"555\")";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, "(...)");

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("(...)"));
        }

        #endregion
    }
}
