using System.Diagnostics;
using System.Linq;
using Dag.Net.Core.SearchAlgorithms;
using Dag.Net.Core;
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

            graph.AddEdgeSuccessfully("a", "b");
            graph.AddEdgeUnsuccessfully("b", "a");
        }

        [Test]
        public void BfsTest()
        {
            var bfs = new BreathFirstSearch<string>();
            var graph = new Dag<string>();

            graph.AddEdgeSuccessfully("a", "b");
            graph.AddEdgeSuccessfully("b", "d");
            graph.AddEdgeSuccessfully("e", "d");

            Assert.AreEqual(3, bfs.Explore(graph, "a", "d", x => x.GetChilds()).Count);
        }

        [Test]
        public void DfsTest()
        {
            var dfs = new DepthFirstSearch<string>();
            var graph = new Dag<string>();

            graph.AddEdgeSuccessfully("a", "b");
            graph.AddEdgeSuccessfully("b", "d");
            graph.AddEdgeSuccessfully("e", "d");

            Assert.AreEqual(3, dfs.Explore(graph, "a", "d", x => x.GetChilds()).Count);
        }

        [Test]
        public void FindPathTest()
        {
            var bfs = new BreathFirstSearch<string>();
            var graph = new Dag<string>();

            // e ---|
            //      v
            // a -> b
            // |
            // |--> c
            graph.AddEdgeSuccessfully("a", "b");
            graph.AddEdgeSuccessfully("a", "c");
            graph.AddEdgeSuccessfully("e", "b");

            var result = bfs.Explore(graph, "a", null, x => x.GetChilds());

            Assert.AreEqual(3, result.Count);
            Assert.That(result.Select(x => x.Value), Is.EquivalentTo(new[] {"a", "b", "c"}));
        }
        
        [Test]
        public void FindReversePathTest()
        {
            var bfs = new BreathFirstSearch<string>();
            var graph = new Dag<string>();

            // e ---|
            //      v
            // a -> b
            // |
            // |--> c
            graph.AddEdgeSuccessfully("a", "b");
            graph.AddEdgeSuccessfully("a", "c");
            graph.AddEdgeSuccessfully("e", "b");

            var result = bfs.Explore(graph, "c", null, x => x.GetParents());

            Assert.AreEqual(2, result.Count);
            Assert.That(result.Select(x => x.Value), Is.EquivalentTo(new[] {"c", "a"}));
        }

        [Test]
        public void TaggingTest()
        {
            var bfs = new BreathFirstSearch<string>();
            var graph = new Dag<string>();

            VehicleSetup(graph);

            var result = bfs.Explore(graph, "honda", null, x => x.GetParents());

            Assert.AreEqual(3, result.Count);
            Assert.That(result.Select(x => x.Value), Is.EquivalentTo(new[] {"honda", "car", "vehicle"}));
        }

        [Test]
        public void MultiplePathsTest()
        {
            var graph = new Dag<string>();
            
            graph.AddEdgeSuccessfully("a", "b");
            graph.AddEdgeSuccessfully("a", "c");
            graph.AddEdgeSuccessfully("b", "d");
            graph.AddEdgeSuccessfully("c", "d");

        }

        [Test]
        public void TaggingTestDfs()
        {
            var dfs = new DepthFirstSearch<string>();
            var graph = new Dag<string>();

            VehicleSetup(graph);

            var result = dfs.Explore(graph, "honda", null, x => x.GetParents());

            Assert.AreEqual(3, result.Count);
            Assert.That(result.Select(x => x.Value), Is.EquivalentTo(new[] {"honda", "car", "vehicle"}));
        }

        [Test]
        public void UnrelatedVertexTest()
        {
            var graph = new Dag<string>();

            graph.AddEdgeSuccessfully("a", "b");
            graph.AddEdgeSuccessfully("c", "d");
        }

        [Test]
        public void RemoveVertexTest()
        {
            var graph = new Dag<string>();

            graph.AddEdgeSuccessfully("a", "b");
            graph.AddEdgeSuccessfully("b", "c");
            graph.AddEdgeSuccessfully("c", "d");

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

        [Test]
        public void ReplaceVertexTest()
        {
            var graph = new Dag<string>();

            VehicleSetup(graph);

            Assert.IsTrue(graph.ReplaceVertex("car", "car2").Successful);

            var hondaVertex = graph.GetVertex("honda");
            var vehicleVertex = graph.GetVertex("vehicle");
            var car2Vertex = graph.GetVertex("car2");
            var motoVertex = graph.GetVertex("moto");

            Assert.IsNull(graph.GetVertex("car"));

            Assert.IsNotNull(hondaVertex);
            Assert.AreEqual(1, hondaVertex.GetParents().Length);
            Assert.IsEmpty(hondaVertex.GetChilds());
            Assert.IsTrue(hondaVertex.GetParents().Any(x => x.Value == car2Vertex.Value));

            Assert.IsNotNull(car2Vertex);
            Assert.AreEqual(1, car2Vertex.GetChilds().Length);
            Assert.AreEqual(1, car2Vertex.GetParents().Length);
            Assert.IsTrue(car2Vertex.GetChilds().Any(x => x.Value == hondaVertex.Value));
            Assert.IsTrue(car2Vertex.GetParents().Any(x => x.Value == vehicleVertex.Value));

            Assert.IsNotNull(motoVertex);
            Assert.IsEmpty(motoVertex.GetChilds());
            Assert.AreEqual(1, motoVertex.GetParents().Length);
            Assert.IsTrue(motoVertex.GetParents().Any(x => x.Value == vehicleVertex.Value));

            Assert.IsNotNull(vehicleVertex);
            Assert.IsEmpty(vehicleVertex.GetParents());
            Assert.AreEqual(2, vehicleVertex.GetChilds().Length);
            Assert.IsTrue(vehicleVertex.GetChilds().Any(x => x.Value == motoVertex.Value));
            Assert.IsTrue(vehicleVertex.GetChilds().Any(x => x.Value == car2Vertex.Value));
        }

        [Test]
        public void InvalidReplaceVertexTest()
        {
            var graph = new Dag<string>();

            VehicleSetup(graph);

            Assert.IsFalse(graph.ReplaceVertex("car", "moto").Successful);
        }

        private static void VehicleSetup(Dag<string> graph)
        {
            graph.AddEdgeSuccessfully("vehicle", "car");
            graph.AddEdgeSuccessfully("vehicle", "moto");
            graph.AddEdgeSuccessfully("car", "honda");
        }

        [Test]
        public void AddDuplicateTest()
        {
            var graph = new Dag<string>();

            graph.AddEdgeSuccessfully("a", "b");
            graph.AddEdgeUnsuccessfully("a", "b");
        }

        [Test]
        public void ManualValidationTest()
        {
            var graph = new Dag<string>();
            
            graph.AddEdgeSuccessfully("a", "b");
            Assert.IsFalse(graph.ValidateAddEdge("a", "b").Successful);
            Assert.IsFalse(graph.ValidateAddEdge("b", "a").Successful);
            
            Assert.IsTrue(graph.ValidateAddEdge("b", "c").Successful);
            // we make sure that it's not actually added
            Assert.IsNull(graph.GetVertex("c"));
        }

        // [Test]
        // public void ToJsonTest()
        // {
        //     var graph = new Dag<string>();
        //
        //     VehicleSetup(graph);
        //
        //     var json = graph.ToJson();
        // }

        [Test]
        public void CopyTest()
        {
            var graph = new Dag<string>();

            VehicleSetup(graph);

            var newGraph = graph.Copy();
            
            Assert.IsTrue(graph.GetAllVertices().Count() == newGraph.GetAllVertices().Count());

            var zippedVertices = graph.GetAllVertices().Zip(newGraph.GetAllVertices());

            foreach (var (first, second) in zippedVertices)
            {
                Assert.AreEqual(first.Value, second.Value);
                Assert.IsFalse(ReferenceEquals(first, second));
            }
        }
    }
}