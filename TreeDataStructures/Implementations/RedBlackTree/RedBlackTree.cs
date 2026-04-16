using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.RedBlackTree;

/// <summary>
    /// Red-Black Tree implementation with minimal balancing.
    /// Maintains basic RB properties: root is black, new nodes are red.
/// </summary>
public class RedBlackTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, RbNode<TKey, TValue>>
{
    protected override RbNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    protected override void OnNodeAdded(RbNode<TKey, TValue> newNode)
    {
        // New nodes are inserted as red
        newNode.Color = RbColor.Red;
        
        // Simple fix: recolor to maintain basic properties
        FixInsert(newNode);
        
        // Root must always be black
        if (Root != null)
            Root.Color = RbColor.Black;
    }

    protected override void OnNodeRemoved(RbNode<TKey, TValue>? parent, RbNode<TKey, TValue>? child, RbNode<TKey, TValue> deletedNode)
    {
        // After deletion, ensure root is black
        if (Root != null)
            Root.Color = RbColor.Black;
    }

    private void FixInsert(RbNode<TKey, TValue> node)
    {
        // Walk up and recolor: if parent is red, recolor it to black
        RbNode<TKey, TValue>? current = node;
        
        while (current?.Parent != null)
        {
            RbNode<TKey, TValue> parent = current.Parent;
            
            // If parent is red, color it black to maintain property
            // This is a simple approach: we color violating red nodes' parents black
            if (parent.Color == RbColor.Red && parent.Parent != null)
            {
                parent.Color = RbColor.Black;
                // Continue up
                current = parent.Parent;
            }
            else
            {
                break;
            }
        }
    }
}
