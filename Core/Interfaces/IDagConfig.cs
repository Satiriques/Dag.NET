using Core.Interfaces;

namespace Core
{
    public interface IDagConfig<T>
    {
        public ITraversalAlgorithm<T> TraversalAlgorithm { get; init; }
    }
}