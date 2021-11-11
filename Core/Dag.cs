using System;
using System.Collections.Generic;
using System.Linq;
using Dag.Net.Core.Interfaces;

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
            if (!ValidateCycling(parent, child))
                return new ValidationResult {Successful = false, Message = "Added a cycle."};

            var parentVertex = _vertices.GetValueOrDefault(parent) ?? new Vertex<T> {Value = parent};
            var childVertex = _vertices.GetValueOrDefault(child) ?? new Vertex<T> {Value = child};

            parentVertex.Childs.Add(childVertex);
            childVertex.Parents.Add(parentVertex);

            _vertices[parent] = parentVertex;
            _vertices[child] = childVertex;

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

        private bool ValidateCycling(T parent, T child)
        {
            if (!_vertices.ContainsKey(parent) || !_vertices.ContainsKey(child))
                return true;

            return !_dagConfig.TraversalAlgorithm.Explore(this, parent, child, v => v.Parents).Any();
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
    }
}