namespace QueryTransformer
{
    public static class PipelineExtensions
    {
        public static TOutput Step<TInput, TOutput>(this TInput input, IPipelineStep<TInput, TOutput> step)
        {
            return step.Execute(input);
        }
    }
}
