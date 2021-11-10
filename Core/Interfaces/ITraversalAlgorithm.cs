using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITraversalAlgorithm<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        HashSet<Vertex<T>> Explore(Dag<T> graph, T source, T destination,
            Func<Vertex<T>, IEnumerable<Vertex<T>>> selector);
    }
}