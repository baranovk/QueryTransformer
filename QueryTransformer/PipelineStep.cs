namespace QueryTransformer
{
    public class PipelineStep<TInput, TOutput> : IPipelineStep<TInput, TOutput>
    {
        #region Fields

        private readonly Func<TInput, TOutput> _stepFunc;

        #endregion

        #region Constructors

        public PipelineStep(Func<TInput, TOutput> stepFunc)
        {
            _stepFunc = stepFunc;
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public TOutput Execute(TInput input) => _stepFunc(input);

        #endregion
    }
}
