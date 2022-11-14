using QueryTransformer;
using Regex.Engine;

namespace QueryTransformer.UnitTests
{
    public partial class RegexReplaceTests
    {
        [Test]
        public void RegexTransformer_ShouldHandlePatternsWithSpaceCharacters()
        {
            var template = @"\s*IN\s+";
            var input = "IN   ";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, "in");

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("in"));
        }

        [Test]
        public void RegexTransformer_ShouldHandlePatternsWithSpaceCharactersAndReplacementDelegate()
        {
            var template = @"\s*IN\s+";
            var input = "IN   ";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, (match) => "in");

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("in"));

            input = "AAA IN   ";
            context = new Context(input.ToCharArray());

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("AAAin"));
        }

        [Test]
        public void RegexTransformer_ShouldReplaceUppercaseCharactersToLowercase()
        {
            var template = @"[A-Z]+";
            var input = "SELECT";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, (match) => match.ToLower());

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("select"));

            template = @"[A-Z012345]+";
            input = "SELECT";
            context = new Context(input.ToCharArray());
            handler = new RegexTransformer(template, (match) => match.ToLower());

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("select"));

            template = @"\s*[A-Z012345]+\s+";
            input = " SELECT   ";
            context = new Context(input.ToCharArray());
            handler = new RegexTransformer(template, (match) => match.ToLower().Trim());

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("select"));
        }

        [Test]
        public void RegexTransformer_ShouldHandleParameterPattern()
        {
            var template = @"=\s*"".+""";
            var input = "= \"ABC\"";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, "=\"?\"");

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("=\"?\""));
        }

        [Test]
        public void RegexTransformer_ShouldHandleParametersInBracesPattern()
        {
            var template = @"\(\s*""[^""]+""\s*\)";
            var input = "( \"ABC\"  )";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, "(...)");

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("(...)"));
        }

        [Test]
        public void RegexTransformer_ShouldHandlePatternsWithExcessSpaceCharacters()
        {
            var input = @"select     *   from table";
            var template = @"\s+";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, " ");

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("select * from table"));
        }

        [Test]
        public void RegexTransformerWithFunc_ShouldHandlePatternsWithExcessSpaceCharacters()
        {
            var input = @"select     *   from table";
            var template = @"\s+";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, match => " ");

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("select * from table"));
        }

        [Test]
        public void RegexTransformer_ShouldHandleExcessSpaceCharactersAtInputStart()
        {
            var input = @"  select  *  from table";
            var template = @"^\s+";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, string.Empty);

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("select  *  from table"));
        }

        [Test]
        public void RegexTransformer_ShouldHandleExcessSpaceCharactersAtInputEnd()
        {
            var input = @"select  *  from table    ";
            var template = @"\s+$";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, string.Empty);

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("select  *  from table"));
        }

        [Test]
        public void RegexTransformer_ShouldTransformMultiLineInputToSingleLine()
        {
            var input 
            = @"select 
* from table";
            var template = @"\r\n";
            var context = new Context(input.ToCharArray());
            var handler = new RegexTransformer(template, string.Empty);

            handler.Process(context);
            Assert.That(new string(context.Result.ToArray()), Is.EqualTo("select * from table"));
        }
    }
}
