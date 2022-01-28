using Dag.Net.Core;
using NUnit.Framework;

namespace Tests
{
    public static class GraphExtensions
    {
        public static void AddEdgeSuccessfully<T>(this Dag<T> graph, T parent, T child)
        {
            var result = graph.AddEdge(parent, child);
            Assert.IsTrue(result.Successful, result.Message);
        }

        public static void AddEdgeUnsuccessfully<T>(this Dag<T> graph, T parent, T child)
        {
            Assert.IsFalse(graph.AddEdge(parent, child).Successful);
        }
    }
}