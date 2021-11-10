# Dag.Net

Simple C# implementation of a Directed Acyclic Graph.

## Graph

### Usage

```c#
using Dag.Net.Core;

//...

var dag = new Dag<string>();

// Adds an edge and the two vertices automatically. 
// vehicle -> car
dag.AddEdge("vehicle", "car");
// vehicle -> moto
dag.AddEdge("vehicle", "moto");
```

## Search Algorithms

Comes with two basics search algorithms that can be used to explorer the graph.

### Breath First Search (BFS)

#### Usage

```c#

var bfs = new BreathFirstSearch<string>();
var graph = new Dag<string>();

graph.AddEdge("a", "b");
graph.AddEdge("b", "d");
graph.AddEdge("e", "d");

var visitedVertices = bfs.Explore(graph, "a", "d", x => x.GetChilds();
```

#### Implementation

You can also implement the interface ISearchAlgorithm<T> to implement your own.