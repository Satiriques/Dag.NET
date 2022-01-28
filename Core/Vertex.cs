using System.Collections.Generic;
using System.Collections.Immutable;

namespace Dag.Net.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Vertex<T>
    {
        public T Value { get; internal set; }
        internal HashSet<Vertex<T>> Childs { get; } = new();
        internal HashSet<Vertex<T>> Parents { get; } = new();

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