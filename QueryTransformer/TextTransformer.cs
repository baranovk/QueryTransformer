using Regex.Engine;

namespace QueryTransformer
{
    public abstract class TextTransformer
    {
        #region Fields

        protected TextTransformer? _nextTransformer;

        #endregion

        #region Public Methods

        public abstract void Process(IContext context);

        #endregion
    }
}
