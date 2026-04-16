using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Interfaces;

namespace TreeDataStructures.Core;

public abstract class BinarySearchTreeBase<TKey, TValue, TNode>(IComparer<TKey>? comparer = null)
    : ITree<TKey, TValue>
    where TNode : Node<TKey, TValue, TNode>
{
    protected TNode? Root;
    public IComparer<TKey> Comparer { get; protected set; } = comparer ?? Comparer<TKey>.Default;

    public int Count { get; protected set; }

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys => InOrder().Select(e => e.Key).ToList();
    public ICollection<TValue> Values => InOrder().Select(e => e.Value).ToList();


    public virtual void Add(TKey key, TValue value)
    {
        TNode node = CreateNode(key, value);
        if (Root == null)
        {
            Root = node;
            OnNodeAdded(node);
            Count++;
            return;
        }

        TNode? current = Root;
        bool nodeAdded = false;

        while (current != null)
        {
            if (Comparer.Compare(key, current.Key) < 0)
            {
                if (current.Left != null)
                {
                    current = current.Left;
                } else
                {
                    current.Left = node;
                    nodeAdded = true;
                    break;
                }
            } else if (Comparer.Compare(key, current.Key) > 0)
            {
                if (current.Right != null)
                {
                    current = current.Right;
                } else
                {
                    current.Right = node;
                    nodeAdded = true;
                    break;
                }
            } else
            {
                current.Value = node.Value;
                break;
            }
        }
        if (nodeAdded)
        {
            node.Parent = current;
            Count++;
            OnNodeAdded(node);
        }
    }


    public virtual bool Remove(TKey key)
    {
        TNode? node = FindNode(key);
        if (node == null) { return false; }

        RemoveNode(node);
        return true;
    }


    protected virtual void RemoveNode(TNode node)
    {
        if (node.Left == null && node.Right == null)
        {
            Transplant(node, null);
            this.Count--;
            OnNodeRemoved(node.Parent, null, node);
            return;
        }

        if (node.Left == null)
        {
            Transplant(node, node.Right);
            this.Count--;
            OnNodeRemoved(node.Parent, node.Right, node);
            return;
        }

        if (node.Right == null)
        {
            Transplant(node, node.Left);
            this.Count--;
            OnNodeRemoved(node.Parent, node.Left, node);
            return;
        }

        TNode replacement = FindExtreme(node.Left, FindNodeMode.Rightmost);
        node.Key = replacement.Key;
        node.Value = replacement.Value;

        Transplant(replacement, replacement.Left);

        this.Count--;
        OnNodeRemoved(replacement.Parent, replacement.Left, replacement);
    }

    protected enum FindNodeMode { Leftmost, Rightmost }

    protected TNode FindExtreme(TNode node, FindNodeMode mode)
    {
        TNode prev = node;
        TNode? cur = node;
        while (cur != null)
        {
            prev = cur;
            cur = (mode == FindNodeMode.Leftmost) ? cur.Left : cur.Right;
        }
        return prev;
    }

    public virtual bool ContainsKey(TKey key) => TryGetValue(key, out _);

    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        TNode? node = FindNode(key);
        if (node != null)
        {
            value = node.Value;
            return true;
        }
        value = default;
        return false;
    }

    public TValue this[TKey key]
    {
        get => TryGetValue(key, out TValue? val) ? val : throw new KeyNotFoundException();
        set => Add(key, value);
    }


    #region Hooks

    protected virtual void OnNodeAdded(TNode newNode) { }

    protected virtual void OnNodeRemoved(TNode? parent, TNode? child, TNode deletedNode) { }

    #endregion


    #region Helpers
    protected abstract TNode CreateNode(TKey key, TValue value);


    protected TNode? FindNode(TKey key)
    {
        TNode? current = Root;
        while (current != null)
        {
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0) { return current; }
            current = cmp < 0 ? current.Left : current.Right;
        }
        return null;
    }

    protected void RotateLeft(TNode x)
    {
        if (x == null || x == Root || x.Parent == null) return;
        
        TNode? tmp = x.Left;
        x.Left = x.Parent;
        x.Parent.Right = tmp;
        tmp?.Parent = x.Parent;

        Transplant(x.Parent, x);
        x.Left.Parent = x;
    }

    protected void RotateRight(TNode y)
    {
        if (y == null || y == Root || y.Parent == null) return;

        TNode? tmp = y.Right;
        y.Right = y.Parent;
        y.Parent.Left = tmp;
        tmp?.Parent = y.Parent;

        Transplant(y.Parent, y);
        y.Right.Parent = y;
    }

    protected void RotateBigLeft(TNode x)
    {
        RotateRight(x);
        RotateLeft(x);
    }
    
    protected void RotateBigRight(TNode y)
    {
        RotateLeft(y);
        RotateRight(y);
    }
    
    protected void RotateDoubleLeft(TNode x)
    {
        for (int i = 0; i < 2; i++)
            RotateLeft(x);
    }
    
    protected void RotateDoubleRight(TNode y)
    {
        for (int i = 0; i < 2; i++)
            RotateRight(y);
    }
    
    protected void Transplant(TNode u, TNode? v)
    {
        if (u.Parent == null)
        {
            Root = v;
        }
        else if (u.IsLeftChild)
        {
            u.Parent.Left = v;
        }
        else
        {
            u.Parent.Right = v;
        }
        v?.Parent = u.Parent;
    }
    #endregion
    
    public IEnumerable<TreeEntry<TKey, TValue>> InOrder()
    {
        var result = new List<TreeEntry<TKey, TValue>>();
        CollectInOrder(Root, result, 0);
        return result;
    }

    private void CollectInOrder(TNode? node, List<TreeEntry<TKey, TValue>> result, int depth)
    {
        if (node == null) return;
        CollectInOrder(node.Left, result, depth + 1);
        result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
        CollectInOrder(node.Right, result, depth + 1);
    }

    public IEnumerable<TreeEntry<TKey, TValue>> PreOrder()
    {
        var result = new List<TreeEntry<TKey, TValue>>();
        CollectPreOrder(Root, result, 0);
        return result;
    }

    private void CollectPreOrder(TNode? node, List<TreeEntry<TKey, TValue>> result, int depth)
    {
        if (node == null) return;
        result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
        CollectPreOrder(node.Left, result, depth + 1);
        CollectPreOrder(node.Right, result, depth + 1);
    }

    public IEnumerable<TreeEntry<TKey, TValue>> PostOrder()
    {
        var result = new List<TreeEntry<TKey, TValue>>();
        CollectPostOrder(Root, result, 0);
        return result;
    }

    private void CollectPostOrder(TNode? node, List<TreeEntry<TKey, TValue>> result, int depth)
    {
        if (node == null) return;
        CollectPostOrder(node.Left, result, depth + 1);
        CollectPostOrder(node.Right, result, depth + 1);
        result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
    }

    public IEnumerable<TreeEntry<TKey, TValue>> InOrderReverse()
    {
        var result = new List<TreeEntry<TKey, TValue>>();
        CollectInOrderReverse(Root, result, 0);
        return result;
    }

    private void CollectInOrderReverse(TNode? node, List<TreeEntry<TKey, TValue>> result, int depth)
    {
        if (node == null) return;
        CollectInOrderReverse(node.Right, result, depth + 1);
        result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
        CollectInOrderReverse(node.Left, result, depth + 1);
    }

    public IEnumerable<TreeEntry<TKey, TValue>> PreOrderReverse()
    {
        var result = new List<TreeEntry<TKey, TValue>>();
        CollectPreOrderReverse(Root, result, 0);
        return result;
    }

    private void CollectPreOrderReverse(TNode? node, List<TreeEntry<TKey, TValue>> result, int depth)
    {
        if (node == null) return;
        CollectPreOrderReverse(node.Right, result, depth + 1);
        CollectPreOrderReverse(node.Left, result, depth + 1);
        result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
    }

    public IEnumerable<TreeEntry<TKey, TValue>> PostOrderReverse()
    {
        var result = new List<TreeEntry<TKey, TValue>>();
        CollectPostOrderReverse(Root, result, 0);
        return result;
    }

    private void CollectPostOrderReverse(TNode? node, List<TreeEntry<TKey, TValue>> result, int depth)
    {
        if (node == null) return;
        result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
        CollectPostOrderReverse(node.Right, result, depth + 1);
        CollectPostOrderReverse(node.Left, result, depth + 1);
    }
    
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrder().Select(e => new KeyValuePair<TKey, TValue>(e.Key, e.Value)).GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public void Clear() { Root = null; Count = 0; }
    public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("Not enough space in array");
        }

        foreach (TreeEntry<TKey, TValue> item in InOrder())
        {
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(item.Key, item.Value);
        }
    }
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
}
