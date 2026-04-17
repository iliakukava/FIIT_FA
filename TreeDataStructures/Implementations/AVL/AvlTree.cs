using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.AVL;

public class AvlTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, AvlNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override AvlNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    protected override void OnNodeAdded(AvlNode<TKey, TValue> newNode)
    {
        RebalanceUpwards(newNode);
    }

    protected override void OnNodeRemoved(AvlNode<TKey, TValue>? parent, AvlNode<TKey, TValue>? child, AvlNode<TKey, TValue> deletedNode)
    {
        RebalanceUpwards(parent);
    }

    private void RebalanceUpwards(AvlNode<TKey, TValue>? node)
    {
        AvlNode<TKey, TValue>? current = node;
        while (current != null)
        {
            UpdateHeight(current);

            int balance = GetBalance(current);
            if (balance > 1)
            {
                if (GetBalance(current.Left) < 0)
                {
                    RotateLeft(current.Left!);
                    UpdateHeight(current.Left!);
                }

                RotateRight(current);
                UpdateHeight(current);
                if (current.Parent != null)
                {
                    UpdateHeight(current.Parent);
                }
            }
            else if (balance < -1)
            {
                if (GetBalance(current.Right) > 0)
                {
                    RotateRight(current.Right!);
                    UpdateHeight(current.Right!);
                }

                RotateLeft(current);
                UpdateHeight(current);
                if (current.Parent != null)
                {
                    UpdateHeight(current.Parent);
                }
            }

            current = current.Parent;
        }
    }

    private static void UpdateHeight(AvlNode<TKey, TValue> node)
    {
        int leftHeight = node.Left?.Height ?? 0;
        int rightHeight = node.Right?.Height ?? 0;
        node.Height = 1 + Math.Max(leftHeight, rightHeight);
    }

    private static int GetBalance(AvlNode<TKey, TValue>? node)
    {
        if (node == null) return 0;
        return (node.Left?.Height ?? 0) - (node.Right?.Height ?? 0);
    }
}
