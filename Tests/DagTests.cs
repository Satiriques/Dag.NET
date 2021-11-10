using System.Linq;
using Core;
using Core.SearchAlgorithms;
using NUnit.Framework;

namespace Tests
{
    public class DagTests
    {
        [Test]
        public void StructureTest()
        {
            var graph = new Dag<string>();

            graph.AddEdge("mom", "child");
            graph.AddEdge("dad", "child");
        }

        [Test]
        public void CyclicTest()
        {
            var graph = new Dag<string>();

            Assert.IsTrue(graph.AddEdge("a", "b").Successful);
            Assert.IsFalse(graph.AddEdge("b", "a").Successful);
        }

        [Test]
        public void BfsTest()
        {
            var bfs = new BreathFirstSearch<string>();
            var graph = new Dag<string>();

            Assert.IsTrue(graph.AddEdge("a", "b").Successful);
            Assert.IsTrue(graph.AddEdge("b", "d").Successful);
            Assert.IsTrue(graph.AddEdge("e", "d").Successful);

            Assert.AreEqual(3, bfs.Explore(graph, "a", "d", x => x.GetChilds()).Count);
        }

        [Test]
        public void UnrelatedVertexTest()
        {
            var graph = new Dag<string>();

            Assert.IsTrue(graph.AddEdge("a", "b").Successful);
            Assert.IsTrue(graph.AddEdge("c", "d").Successful);
        }

        [Test]
        public void RemoveVertexTest()
        {
            var graph = new Dag<string>();

            Assert.IsTrue(graph.AddEdge("a", "b").Successful);
            Assert.IsTrue(graph.AddEdge("b", "c").Successful);
            Assert.IsTrue(graph.AddEdge("c", "d").Successful);

            Assert.IsTrue(graph.RemoveEdge("b", "c").Successful);

            var vertexA = graph.GetVertex("a");
            var vertexB = graph.GetVertex("b");
            var vertexC = graph.GetVertex("c");
            var vertexD = graph.GetVertex("d");

            Assert.IsEmpty(vertexA.GetParents());
            Assert.AreEqual(1, vertexA.GetChilds().Length);
            Assert.IsTrue(vertexA.GetChilds().Any(x => x.Value == vertexB.Value));

            Assert.IsEmpty(vertexB.GetChilds());
            Assert.AreEqual(1, vertexB.GetParents().Length);
            Assert.IsTrue(vertexB.GetParents().Any(x => x.Value == vertexA.Value));

            Assert.IsEmpty(vertexC.GetParents());
            Assert.AreEqual(1, vertexC.GetChilds().Length);
            Assert.IsTrue(vertexC.GetChilds().Any(x => x.Value == vertexD.Value));

            Assert.IsEmpty(vertexD.GetChilds());
            Assert.AreEqual(1, vertexD.GetParents().Length);
            Assert.IsTrue(vertexD.GetParents().Any(x => x.Value == vertexC.Value));
        }
    }
}