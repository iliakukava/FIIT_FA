using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Implementations.BST;

namespace TreeDataStructures.Implementations.Splay;

public class SplayTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
{
    protected override BstNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    private void Splay(BstNode<TKey, TValue> node)
    {
        while (node.Parent != null)
        {
            BstNode<TKey, TValue> parent = node.Parent;

            if (parent.Parent == null)
            {
                if (node.IsLeftChild)
                {
                    RotateRight(parent);
                }
                else
                {
                    RotateLeft(parent);
                }
            }
            else
            {
                BstNode<TKey, TValue> grandparent = parent.Parent;

                if (node.IsLeftChild && parent.IsLeftChild)
                {
                    RotateRight(grandparent);
                    RotateRight(parent);
                }
                else if (node.IsRightChild && parent.IsRightChild)
                {
                    RotateLeft(grandparent);
                    RotateLeft(parent);
                }
                else if (node.IsRightChild && parent.IsLeftChild)
                {
                    RotateLeft(parent);
                    RotateRight(grandparent);
                }
                else
                {
                    RotateRight(parent);
                    RotateLeft(grandparent);
                }
            }
        }
    }

    protected override void OnNodeAdded(BstNode<TKey, TValue> newNode)
    {
        Splay(newNode);
    }

    protected override void OnNodeRemoved(BstNode<TKey, TValue>? parent, BstNode<TKey, TValue>? child, BstNode<TKey, TValue> deletedNode)
    {
        if (parent != null)
            Splay(parent);
    }

    public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        BstNode<TKey, TValue>? node = FindNode(key);
        if (node != null)
        {
            value = node.Value;
            Splay(node);
            return true;
        }
        value = default;
        return false;
    }
}
