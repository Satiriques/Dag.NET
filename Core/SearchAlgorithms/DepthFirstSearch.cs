using System;
using System.Collections.Generic;
using System.Linq;
using Dag.Net.Core.Interfaces;

namespace Dag.Net.Core.SearchAlgorithms
{
    public class DepthFirstSearch<T> : ITraversalAlgorithm<T>
    {
        public HashSet<Vertex<T>> Explore(Dag<T> graph, T source, T destination, Func<Vertex<T>, IEnumerable<Vertex<T>>> selector)
        {
            var stack = new Stack<Vertex<T>>();
            var visited = new HashSet<Vertex<T>>();
            
            var sourceVertex = graph.GetVertex(source);
            
            stack.Push(sourceVertex);

            while (stack.Any())
            {
                var current = stack.Pop();
                if (visited.Any(x => x.Value.Equals(current.Value))) continue;
                
                visited.Add(current);
                foreach (var edge in selector(current))
                {
                    stack.Push(edge);
                }
            }
            return visited;
        }
    }
}