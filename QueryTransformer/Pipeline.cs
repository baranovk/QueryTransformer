namespace QueryTransformer
{
    public class Pipeline<TInput, TOutput> : IPipelineStep<TInput, TOutput>
    {
        private Func<TInput, TOutput> _pipelineSteps;

        public Pipeline(Func<TInput, TOutput> pipelineSteps)
        {
            _pipelineSteps = pipelineSteps;
        }

        public TOutput Execute(TInput input)
        {
            return _pipelineSteps(input);
        }
    }
}
