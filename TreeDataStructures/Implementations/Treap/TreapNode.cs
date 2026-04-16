using System.Threading;
using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.Treap;

public class TreapNode<TKey, TValue>(TKey key, TValue value)
    : Node<TKey, TValue, TreapNode<TKey, TValue>>(key, value)
{
    private static int s_nextPriority = int.MaxValue;
    public int Priority { get; set; } = Interlocked.Decrement(ref s_nextPriority);
}