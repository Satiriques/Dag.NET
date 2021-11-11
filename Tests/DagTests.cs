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
        public void DfsTest()
        {
            var dfs = new DepthFirstSearch<string>();
            var graph = new Dag<string>();

            Assert.IsTrue(graph.AddEdge("a", "b").Successful);
            Assert.IsTrue(graph.AddEdge("b", "d").Successful);
            Assert.IsTrue(graph.AddEdge("e", "d").Successful);

            Assert.AreEqual(3, dfs.Explore(graph, "a", "d", x => x.GetChilds()).Count);
        }

        [Test]
        public void FindPathTest()
        {
            var bfs = new BreathFirstSearch<string>();
            var graph = new Dag<string>();

            // b ---|
            //      v
            // a -> b
            // |
            // |--> c
            graph.AddEdge("a", "b");
            graph.AddEdge("a", "c");
            graph.AddEdge("e", "b");

           var result = bfs.Explore(graph, "a", null, x => x.GetChilds());
           
           Assert.AreEqual(3, result.Count);
           Assert.That(result.Select(x=>x.Value), Is.EquivalentTo(new[] {"a", "b", "c"}));
        }

        [Test]
        public void TaggingTest()
        {
            var bfs = new BreathFirstSearch<string>();
            var graph = new Dag<string>();

            VehicleSetup(graph);

            var result = bfs.Explore(graph, "honda", null, x => x.GetParents());
            
            Assert.AreEqual(3, result.Count);
            Assert.That(result.Select(x=>x.Value), Is.EquivalentTo(new[] {"honda", "car", "vehicle"}));
        }
        
        [Test]
        public void TaggingTestDfs()
        {
            var dfs = new DepthFirstSearch<string>();
            var graph = new Dag<string>();

            VehicleSetup(graph);

            var result = dfs.Explore(graph, "honda", null, x => x.GetParents());
            
            Assert.AreEqual(3, result.Count);
            Assert.That(result.Select(x=>x.Value), Is.EquivalentTo(new[] {"honda", "car", "vehicle"}));
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
            Assert.IsTrue(hondaVertex.GetParents().Any(x=>x.Value == car2Vertex.Value));
            
            Assert.IsNotNull(car2Vertex);
            Assert.AreEqual(1, car2Vertex.GetChilds().Length);
            Assert.AreEqual(1, car2Vertex.GetParents().Length);
            Assert.IsTrue(car2Vertex.GetChilds().Any(x=>x.Value == hondaVertex.Value));
            Assert.IsTrue(car2Vertex.GetParents().Any(x=>x.Value == vehicleVertex.Value));
            
            Assert.IsNotNull(motoVertex);
            Assert.IsEmpty(motoVertex.GetChilds());
            Assert.AreEqual(1, motoVertex.GetParents().Length);
            Assert.IsTrue(motoVertex.GetParents().Any(x=>x.Value == vehicleVertex.Value));
            
            Assert.IsNotNull(vehicleVertex);
            Assert.IsEmpty(vehicleVertex.GetParents());
            Assert.AreEqual(2, vehicleVertex.GetChilds().Length);
            Assert.IsTrue(vehicleVertex.GetChilds().Any(x=>x.Value == motoVertex.Value));
            Assert.IsTrue(vehicleVertex.GetChilds().Any(x=>x.Value == car2Vertex.Value));
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
            Assert.IsTrue(graph.AddEdge("vehicle", "car").Successful);
            Assert.IsTrue(graph.AddEdge("vehicle", "moto").Successful);
            Assert.IsTrue(graph.AddEdge("car", "honda").Successful);
        }
    }
}