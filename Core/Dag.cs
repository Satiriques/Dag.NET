using System;
using System.Collections.Generic;
using System.Linq;
using Dag.Net.Core.Interfaces;
using Newtonsoft.Json;

// parent -> child
namespace Dag.Net.Core
{
    public class Dag<T>
    {
        private readonly IDagConfig<T> _dagConfig;

        /// <summary>
        ///     Collection with the actual data.
        /// </summary>
        private readonly Dictionary<T, Vertex<T>> _vertices = new();

        private readonly List<(T Parent, T Child)> _edges = new();

        /// <summary>
        /// </summary>
        public Dag(IDagConfig<T> dagConfig = null)
        {
            _dagConfig = dagConfig ?? new DagConfig<T>();
        }

        /// <summary>
        ///     Adds a new edge to the graph. If the child and/or parent doesn't already exists, they are automatically added
        ///     to the graph.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public ValidationResult AddEdge(T parent, T child)
        {
            if (parent.Equals(_lastAddEdgeValidation.Parent) && _lastAddEdgeValidation.Equals(child))
                return _lastAddEdgeValidation.Result;

            var result = ValidateAddEdge(parent, child);

            if (!result.Successful)
                return result;

            AddEdgeInternal(parent, child);

            return ValidationResult.Success;
        }

        private void AddEdgeInternal(T parent, T child)
        {
            var parentVertex = _vertices.GetValueOrDefault(parent) ?? new Vertex<T> {Value = parent};
            var childVertex = _vertices.GetValueOrDefault(child) ?? new Vertex<T> {Value = child};

            parentVertex.Childs.Add(childVertex);
            childVertex.Parents.Add(parentVertex);

            _vertices[parent] = parentVertex;
            _vertices[child] = childVertex;

            _edges.Add((parent, child));
        }

        private (ValidationResult Result, T Parent, T Child) _lastAddEdgeValidation;

        public ValidationResult ValidateAddEdge(T parent, T child)
        {
            if (parent.Equals(_lastAddEdgeValidation.Parent) && _lastAddEdgeValidation.Equals(child))
                return _lastAddEdgeValidation.Result;

            if (!ValidateCycling(parent, child))
            {
                return new ValidationResult {Successful = false, Message = "Added a cycle."};
            }

            if (!ValidateDuplicate(parent, child))
            {
                return new ValidationResult {Successful = false, Message = "Added a duplicate."};
            }
            
            return ValidationResult.Success;
        }

        /// <summary>
        ///     Removes an edge from two vertices. The direction doesn't matter in this case.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        public ValidationResult RemoveEdge(T vertex1, T vertex2)
        {
            if (vertex1 == null) throw new ArgumentNullException(nameof(vertex1));
            if (vertex2 == null) throw new ArgumentNullException(nameof(vertex2));

            var firstVertex = GetVertex(vertex1);
            var secondVertex = GetVertex(vertex2);

            if (firstVertex == null)
                return new ValidationResult {Successful = false, Message = $"Vertex {vertex1} was not found"};
            if (secondVertex == null)
                return new ValidationResult {Successful = false, Message = $"Vertex {vertex2} was not found"};

            firstVertex.Childs.Remove(secondVertex);
            firstVertex.Parents.Remove(secondVertex);

            secondVertex.Childs.Remove(firstVertex);
            secondVertex.Parents.Remove(firstVertex);

            return ValidationResult.Success;
        }

        public ValidationResult ReplaceVertex(T valueToBeReplaced, T newValue)
        {
            if (valueToBeReplaced == null) throw new ArgumentNullException(nameof(valueToBeReplaced));
            if (newValue == null) throw new ArgumentNullException(nameof(newValue));

            var vertexToBeReplaced = GetVertex(valueToBeReplaced);

            if (vertexToBeReplaced == null)
            {
                return new ValidationResult {Successful = false, Message = $"Vertex {valueToBeReplaced} was not found"};
            }

            if (GetVertex(newValue) != null)
            {
                return new ValidationResult
                    {Successful = false, Message = $"Vertex {newValue} was already in the graph"};
            }

            vertexToBeReplaced.Value = newValue;
            _vertices.Remove(valueToBeReplaced);
            _vertices.Add(newValue, vertexToBeReplaced);

            return ValidationResult.Success;
        }

        private bool ValidateDuplicate(T parent, T child)
        {
            return !_edges.Contains((parent, child));
        }

        private bool ValidateCycling(T parent, T child)
        {
            var result = false;
            
            if (!_vertices.ContainsKey(parent) || !_vertices.ContainsKey(child))
            {
                result = true;
            }
            else
            {
                var explored = _dagConfig.TraversalAlgorithm.Explore(this, parent, child, v => v.Parents);

                if (!explored.Any(x => x.Value.Equals(parent)) || !explored.Any(x => x.Value.Equals(child)))
                {
                    result = true;
                }

                // result = !_dagConfig.TraversalAlgorithm.Explore(this, parent, child, v => v.Parents).Any();
            }

            _lastAddEdgeValidation = (
                Result: new ValidationResult {Successful = result, Message = result ? null : "Added a cycle."},
                Parent: parent, Child: child);
            return result;
        }

        /// <summary>
        ///     Gets the vertex of the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>the vertex found, else null</returns>
        public Vertex<T> GetVertex(T value)
        {
            return _vertices.GetValueOrDefault(value);
        }

        // public string ToJson()
        // {
        //     return JsonConvert.SerializeObject(_edges, Formatting.Indented);
        // }

        // public bool TryLoadJson(string json)
        // {
        //     List<(T Parent, T Child)> edges = null;
        //     
        //     try
        //     {
        //         edges = JsonConvert.DeserializeObject<List<(T Parent, T Child)>>(json);
        //     }
        //     catch
        //     {
        //         return false;
        //     }
        //
        //     if (edges == null)
        //         return false;
        //
        //     _vertices.Clear();
        //     _edges.Clear();
        //     
        //     foreach (var (parent, child) in edges)
        //     {
        //         AddEdgeInternal(parent, child);
        //     }
        //     
        //     return true;
        // }

        public Dag<T> Copy()
        {
            var dag = new Dag<T>();

            foreach (var (parent, child) in _edges)
            {
                dag.AddEdgeInternal(parent, child);
            }

            return dag;
        }

        public IEnumerable<Vertex<T>> GetAllVertices()
        {
            return _vertices.Values;
        }
    }
}