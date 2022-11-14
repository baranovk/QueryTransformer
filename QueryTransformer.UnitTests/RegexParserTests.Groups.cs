using Regex.Engine;
using Regex.Engine.Rules;

namespace QueryTransformer.UnitTests
{
    public partial class RegexParserTests
    {
        #region Public Methods

        [Test]
        public void PatternParser_ShouldRecognizeGroupPattern()
        {
            var template = @"(\s+)";
            var pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<GroupRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.EqualTo(1));
        }

        [Test]
        public void PatternParser_ShouldRecognizeGroupPatternWithMultipleQuantity()
        {
            var template = @"(\s+)+";
            var pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<GroupRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.Null);
        }

        [Test]
        public void PatternParser_ShouldRecognizeGroupPatternWithExplicitMultipleQuantity()
        {
            var template = @"(\s+){1,20}";
            var pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);
            Assert.That(pattern.First(), Is.InstanceOf<GroupRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.EqualTo(20));
        }

        [Test]
        public void PatternParser_ShouldRecognizeMultipleGroupsPatterns()
        {
            var template = @"([A-Z])+(\s+){1,20}";
            var pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);

            Assert.That(pattern.First(), Is.InstanceOf<GroupRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.Null);

            Assert.That(pattern.ElementAt(1), Is.InstanceOf<GroupRule>());
            Assert.That(pattern.ElementAt(1).Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.ElementAt(1).Quantity.To, Is.EqualTo(20));
        }

        [Test]
        public void PatternParser_ShouldRecognizeMultipleGroupsWithModificatorsPatterns()
        {
            var template = @"^([A-Z])+(\s+){1,20}$";
            var pattern = PatternParser.Parse(template);

            Assert.That(pattern.Any(), Is.True);

            Assert.That(pattern.First(), Is.InstanceOf<GroupRule>());
            Assert.That(pattern.First().Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.First().Quantity.To, Is.Null);
            Assert.That(pattern.First().Modificators, Is.Not.Null);
            Assert.That(pattern.First().Modificators.Value.HasFlag(Modificators.FromStartOnly), Is.True);

            Assert.That(pattern.ElementAt(1), Is.InstanceOf<GroupRule>());
            Assert.That(pattern.ElementAt(1).Quantity.From, Is.EqualTo(1));
            Assert.That(pattern.ElementAt(1).Quantity.To, Is.EqualTo(20));
            Assert.That(pattern.ElementAt(1).Modificators, Is.Not.Null);
            Assert.That(pattern.ElementAt(1).Modificators.Value.HasFlag(Modificators.AtEndOnly), Is.True);
        }

        #endregion
    }
}
