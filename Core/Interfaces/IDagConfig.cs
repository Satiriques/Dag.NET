namespace Dag.Net.Core.Interfaces
{
    public interface IDagConfig<T>
    {
        public ITraversalAlgorithm<T> TraversalAlgorithm { get; init; }
    }
}