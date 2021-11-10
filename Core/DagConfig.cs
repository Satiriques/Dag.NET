using System;
using Dag.Net.Core.Interfaces;
using Dag.Net.Core.SearchAlgorithms;

namespace Dag.Net.Core
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