namespace Regex.Engine.Rules
{
    public class SpaceRule : PatternRule
    {
        #region Fields


        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public override bool Match(char? character) => character.IsSpace();

        #endregion
    }
}
