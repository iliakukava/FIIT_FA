using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.RedBlackTree;

public class RedBlackTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, RbNode<TKey, TValue>>
{
    protected override RbNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    private static bool IsBlack(RbNode<TKey, TValue>? node)
        => node == null || node.Color == RbColor.Black;

    private static bool IsRed(RbNode<TKey, TValue>? node)
        => node != null && node.Color == RbColor.Red;

    private static RbNode<TKey, TValue>? Sibling(RbNode<TKey, TValue> parent, RbNode<TKey, TValue>? node)
        => ReferenceEquals(parent.Left, node) ? parent.Right : parent.Left;

    protected override void OnNodeAdded(RbNode<TKey, TValue> newNode)
    {
        newNode.Color = RbColor.Red;
        InsertFixup(newNode);
        if (Root != null)
        {
            Root.Color = RbColor.Black;
        }
    }

    protected override void OnNodeRemoved(RbNode<TKey, TValue>? parent, RbNode<TKey, TValue>? child, RbNode<TKey, TValue> deletedNode)
    {
        if (deletedNode.Color == RbColor.Black)
        {
            DeleteFixup(parent, child);
        }

        if (Root != null)
        {
            Root.Color = RbColor.Black;
        }
    }

    private void InsertFixup(RbNode<TKey, TValue> node)
    {
        while (node.Parent != null && node.Parent.Color == RbColor.Red)
        {
            RbNode<TKey, TValue> parent = node.Parent;
            RbNode<TKey, TValue>? grandparent = parent.Parent;
            if (grandparent == null)
            {
                break;
            }

            if (parent.IsLeftChild)
            {
                RbNode<TKey, TValue>? uncle = grandparent.Right;
                if (IsRed(uncle))
                {
                    parent.Color = RbColor.Black;
                    uncle!.Color = RbColor.Black;
                    grandparent.Color = RbColor.Red;
                    node = grandparent;
                }
                else
                {
                    if (node.IsRightChild)
                    {
                        node = parent;
                        RotateLeft(node);
                        parent = node.Parent!;
                    }

                    parent.Color = RbColor.Black;
                    grandparent.Color = RbColor.Red;
                    RotateRight(grandparent);
                }
            }
            else
            {
                RbNode<TKey, TValue>? uncle = grandparent.Left;
                if (IsRed(uncle))
                {
                    parent.Color = RbColor.Black;
                    uncle!.Color = RbColor.Black;
                    grandparent.Color = RbColor.Red;
                    node = grandparent;
                }
                else
                {
                    if (node.IsLeftChild)
                    {
                        node = parent;
                        RotateRight(node);
                        parent = node.Parent!;
                    }

                    parent.Color = RbColor.Black;
                    grandparent.Color = RbColor.Red;
                    RotateLeft(grandparent);
                }
            }
        }
    }

    private void DeleteFixup(RbNode<TKey, TValue>? parent, RbNode<TKey, TValue>? node)
    {
        while (parent != null && node != Root && IsBlack(node))
        {
            if (ReferenceEquals(node, parent.Left))
            {
                RbNode<TKey, TValue>? sibling = parent.Right;

                if (IsRed(sibling))
                {
                    sibling!.Color = RbColor.Black;
                    parent.Color = RbColor.Red;
                    RotateLeft(parent);
                    sibling = parent.Right;
                }

                if (IsBlack(sibling?.Left) && IsBlack(sibling?.Right))
                {
                    if (sibling != null)
                    {
                        sibling.Color = RbColor.Red;
                    }

                    node = parent;
                    parent = node.Parent;
                }
                else
                {
                    if (IsBlack(sibling?.Right))
                    {
                        if (sibling?.Left != null)
                        {
                            sibling.Left.Color = RbColor.Black;
                        }

                        if (sibling != null)
                        {
                            sibling.Color = RbColor.Red;
                            RotateRight(sibling);
                        }

                        sibling = parent.Right;
                    }

                    if (sibling != null)
                    {
                        sibling.Color = parent.Color;
                    }
                    parent.Color = RbColor.Black;
                    if (sibling?.Right != null)
                    {
                        sibling.Right.Color = RbColor.Black;
                    }

                    RotateLeft(parent);
                    node = Root;
                    break;
                }
            }
            else
            {
                RbNode<TKey, TValue>? sibling = parent.Left;

                if (IsRed(sibling))
                {
                    sibling!.Color = RbColor.Black;
                    parent.Color = RbColor.Red;
                    RotateRight(parent);
                    sibling = parent.Left;
                }

                if (IsBlack(sibling?.Left) && IsBlack(sibling?.Right))
                {
                    if (sibling != null)
                    {
                        sibling.Color = RbColor.Red;
                    }

                    node = parent;
                    parent = node.Parent;
                }
                else
                {
                    if (IsBlack(sibling?.Left))
                    {
                        if (sibling?.Right != null)
                        {
                            sibling.Right.Color = RbColor.Black;
                        }

                        if (sibling != null)
                        {
                            sibling.Color = RbColor.Red;
                            RotateLeft(sibling);
                        }

                        sibling = parent.Left;
                    }

                    if (sibling != null)
                    {
                        sibling.Color = parent.Color;
                    }
                    parent.Color = RbColor.Black;
                    if (sibling?.Left != null)
                    {
                        sibling.Left.Color = RbColor.Black;
                    }

                    RotateRight(parent);
                    node = Root;
                    break;
                }
            }
        }

        if (node != null)
        {
            node.Color = RbColor.Black;
        }
    }
}
