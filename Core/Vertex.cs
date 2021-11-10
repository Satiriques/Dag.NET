using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core
{
    public class Vertex<T>
    {
        public T Value { get; init; }
        internal List<Vertex<T>> Childs { get; } = new();
        internal List<Vertex<T>> Parents { get; } = new();

        public ImmutableArray<Vertex<T>> GetChilds()
        {
            return Childs.ToImmutableArray();
        }

        public ImmutableArray<Vertex<T>> GetParents()
        {
            return Parents.ToImmutableArray();
        }
    }
}