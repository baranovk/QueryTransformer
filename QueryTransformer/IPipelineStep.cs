namespace QueryTransformer
{
    public interface IPipelineStep<TInput, TOutput>
    {
        TOutput Execute(TInput input);
    }
}
