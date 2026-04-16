using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.AVL;

public class AvlTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, AvlNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override AvlNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    protected override void OnNodeAdded(AvlNode<TKey, TValue> newNode)
    {
        Root = BalanceTree(Root);
    }

    protected override void OnNodeRemoved(AvlNode<TKey, TValue>? parent, AvlNode<TKey, TValue>? child, AvlNode<TKey, TValue> deletedNode)
    {
        Root = BalanceTree(Root);
    }

    private AvlNode<TKey, TValue>? BalanceTree(AvlNode<TKey, TValue>? node)
    {
        if (node == null) return null;

        node.Left = BalanceTree(node.Left);
        node.Right = BalanceTree(node.Right);

        UpdateHeight(node);
        return BalanceNode(node);
    }

    private AvlNode<TKey, TValue> BalanceNode(AvlNode<TKey, TValue> node)
    {
        int balance = GetBalance(node);

        if (balance > 1 && GetBalance(node.Left) >= 0)
            return RotateRightSimple(node);

        if (balance > 1 && GetBalance(node.Left) < 0)
        {
            node.Left = RotateLeftSimple(node.Left!);
            return RotateRightSimple(node);
        }

        if (balance < -1 && GetBalance(node.Right) <= 0)
            return RotateLeftSimple(node);

        if (balance < -1 && GetBalance(node.Right) > 0)
        {
            node.Right = RotateRightSimple(node.Right!);
            return RotateLeftSimple(node);
        }

        return node;
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

    private AvlNode<TKey, TValue> RotateRightSimple(AvlNode<TKey, TValue> y)
    {
        AvlNode<TKey, TValue> x = y.Left!;

        y.Left = x.Right;
        if (y.Left != null) y.Left.Parent = y;

        x.Right = y;
        x.Parent = y.Parent;
        y.Parent = x;

        UpdateHeight(y);
        UpdateHeight(x);

        return x;
    }

    private AvlNode<TKey, TValue> RotateLeftSimple(AvlNode<TKey, TValue> x)
    {
        AvlNode<TKey, TValue> y = x.Right!;

        x.Right = y.Left;
        if (x.Right != null) x.Right.Parent = x;

        y.Left = x;
        y.Parent = x.Parent;
        x.Parent = y;

        UpdateHeight(x);
        UpdateHeight(y);

        return y;
    }
}
