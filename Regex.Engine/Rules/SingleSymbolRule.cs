
namespace Regex.Engine.Rules
{
    public class SingleSymbolRule : PatternRule
    {
        #region Fields

        private readonly char _character;

        #endregion

        #region Constructors

        public SingleSymbolRule(char character)
        {
            _character = character;
        }      

        #endregion

        #region Public Methods

        public override bool Match(char? character) => _character == character;

        #endregion

        #region Protected Methods

        protected override bool ValidateQuantity() => Quantity.Validate(1);

        #endregion
    }
}
