using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface ITraversalAlgorithm<T>
    {
        HashSet<Vertex<T>> Explore(Dag<T> graph, T source, T destination,
            Func<Vertex<T>, IEnumerable<Vertex<T>>> selector);
    }
}