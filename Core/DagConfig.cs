using System;
using Core.Interfaces;
using Core.SearchAlgorithms;

namespace Core
{
    public class DagConfig<T> : IDagConfig<T>
    {
        private readonly ITraversalAlgorithm<T> _traversalAlgorithm = new BreathFirstSearch<T>();

        public ITraversalAlgorithm<T> TraversalAlgorithm
        {
            get => _traversalAlgorithm;
            init => _traversalAlgorithm = value ??
                                          throw new ArgumentNullException(
                                              $"Property {nameof(TraversalAlgorithm)} cannot be null");
        }
    }
}