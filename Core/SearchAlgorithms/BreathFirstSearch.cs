using System;
using System.Collections.Generic;
using System.Linq;
using Dag.Net.Core.Interfaces;

namespace Dag.Net.Core.SearchAlgorithms
{
    public class BreathFirstSearch<T> : ITraversalAlgorithm<T>
    {
        /// <summary>
        ///     test
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="selector">Determines what to iterate when exploring the graph</param>
        /// <returns></returns>
        public HashSet<Vertex<T>> Explore(Dag<T> graph, T source, T destination,
            Func<Vertex<T>, IEnumerable<Vertex<T>>> selector)
        {
            var queue = new Queue<Vertex<T>>();
            var visited = new HashSet<Vertex<T>>();

            queue.Enqueue(graph.GetVertex(source));
            visited.Add(graph.GetVertex(source));

            while (queue.Any())
            {
                var current = queue.Dequeue();
                if (current.Value.Equals(destination)) return visited;

                foreach (var item in selector(current))
                {
                    if (visited.Contains(item)) continue;

                    queue.Enqueue(item);
                    visited.Add(item);
                }
            }

            return visited;
        }
    }
}