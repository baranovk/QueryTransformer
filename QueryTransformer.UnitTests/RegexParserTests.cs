using Regex.Engine;
using Regex.Engine.Rules;

namespace QueryTransformer.UnitTests
{
    public partial class RegexParserTests
    {
        #region Public Methods

        [Test]
        public void PatternParser_ShouldRecognizeSpaceSymbol()
        {
            var template = @"\s";
            var pattern = PatternParser.Parse(template);
            
            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<SpaceRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.EqualTo(1));

            template = @"\s?";
            pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<SpaceRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(0));
            Assert.That(pattern.First().Quantity.To, Is.EqualTo(1));

            template = @"\s*";
            pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<SpaceRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(0));
            Assert.That(pattern.First().Quantity.To, Is.Null);

            template = @"\s+";
            pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<SpaceRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.Null);

            template = @"\s{1,2}";
            pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<SpaceRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.EqualTo(2));

            template = @"\s{,2}";
            pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<SpaceRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.EqualTo(2));

            template = @"\s{1,}";
            pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<SpaceRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.Null);

            template = @"\s{5}";
            pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<SpaceRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(5));
            Assert.That(pattern.First().Quantity.To, Is.EqualTo(5));
        }        

        #endregion
    }
}
