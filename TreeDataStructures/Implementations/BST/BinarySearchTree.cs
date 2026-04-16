using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.BST;

public class BinarySearchTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, BstNode<TKey, TValue>>
{
    protected override BstNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new BstNode<TKey, TValue>(key, value);
    }

    protected override void OnNodeAdded(BstNode<TKey, TValue> newNode)
    {
        // Для базового BST не требуется дополнительная логика при добавлении узла
    }

    protected override void OnNodeRemoved(BstNode<TKey, TValue>? parent, BstNode<TKey, TValue>? child, BstNode<TKey, TValue> deletedNode)
    {
        // Для базового BST не требуется дополнительная логика при удалении узла
    }
}
