using System;
using System.Collections.Generic;

namespace BadTree.BadDataStructures.BadTree
{
    public class BadTreeNode<T> where T : IComparable
    {
        /// <summary>
        /// The data stored in this node
        /// </summary>
        public T Data { get; private set; }
        
        /// <summary>
        /// The previous node attached to this node, the parent of this node
        /// </summary>
        public BadTreeNode<T> Parent { get; private set; }

        /// <summary>
        /// The linked list containing all the children of this node
        /// </summary>
        public readonly List<BadTreeNode<T>> children = new();

        
        public int Level { get; private set; }

        // Constructors
        
        public BadTreeNode(T data, BadTreeNode<T> parent)
        {
            Data = data;
            Parent = parent;

            if (parent == null)
            {
                Level = 1;
                return;
            }

            Level = parent.Level + 1;
        }
        
        
        // Internal Child Functions

        private void Add(BadTreeNode<T> newChild)
        {
            if (newChild == null) return;
            children.Add(newChild);
        }

        private void Remove(BadTreeNode<T> child)
        {
            children.Remove(child);
        }
        private void RemoveAt(int index)
        {
            children.RemoveAt(index);
        }

        private void KillAllChildren()
        {
            children.Clear();
        }

        
        // ----------- public child functions -----------
        // -     Find
        public BadTreeNode<T> FindChildWithData(T data)
        {
            return children.Find(node => node.Data.CompareTo(data) == 0);
        }

        // -     Get
        public BadTreeNode<T> GetChildAt(int index)
        {
            return children[index];
        }
        
        // -     Set
        public void SetData(T newData) => Data = newData;

        // -     Add
        public void AddChild(T data)
        {
            Add(new BadTreeNode<T>(data, this));
        }
        public void AddChildren(T[] values)
        {
            foreach (var value in values)
            {
                Add(new BadTreeNode<T>(value, this));
            }
        }
        
        // -     Graft
        public void GraftTree(BadTreeNode<T> root) => Add(root);
        public void GraftTrees(BadTreeNode<T>[] roots)
        {
            foreach (var root in roots)
            {
                GraftTree(root);
            }
        }

        // -     Remove
        public void RemoveChildAt(int index)
        {
            if (!IsIndexInRange(index)) throw new IndexOutOfRangeException();
            RemoveAt(index);
        }
        public void RemoveChildWithData(T data) => Remove(FindChildWithData(data));


        private bool IsIndexInRange(int index) => index > 0 && index < children.Count;
        public int CompareTo(object obj)
        {
            if (obj is not BadTreeNode<T> node) return -1;

            if (Data.CompareTo(node.Data) == 0
                && Parent.CompareTo(node.Parent) == 0)
            {
                return 0;
            }

            return -1;
        }
    }
}