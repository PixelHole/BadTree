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
        /// The linked list containing all the Children of this node
        /// </summary>
        public List<BadTreeNode<T>> Children { get; private set; } = new();

        /// <summary>
        /// The level of this node
        /// </summary>
        public int Level { get; private set; }


        public bool IsDeadEnd => Children.Count == 0;
        public bool IsRoot => Level == 1;
        
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
            Children.Add(newChild);
        }

        private void Remove(BadTreeNode<T> child)
        {
            Children.Remove(child);
        }
        private void RemoveAt(int index)
        {
            Children.RemoveAt(index);
        }

        private void KillAllChildren()
        {
            Children.Clear();
        }

        
        // ----------- public child functions -----------
        // -     Find
        public BadTreeNode<T> FindChildWithData(T data)
        {
            return Children.Find(node => node.Data.CompareTo(data) == 0);
        }
        public int FindChildIndexWithData(T data)
        {
            return Children.IndexOf(Children.Find(node => node.Data.CompareTo(data) == 0));
        }

        // -     Get
        public BadTreeNode<T> GetChildAt(int index)
        {
            return Children[index];
        }
        public int GetIndexOfChild(BadTreeNode<T> child)
        {
            return Children.IndexOf(child);
        }
        
        // -     Set
        public void SetData(T newData) => Data = newData;

        // -     Add
        public void AddChild(T data)
        {
            Add(new BadTreeNode<T>(data, this));
        }
        public void AddChildren(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                Add(new BadTreeNode<T>(value, this));
            }
        }
        
        // -     Graft
        public void GraftTree(BadTreeNode<T> root) => Add(root);
        public void GraftTrees(IEnumerable<BadTreeNode<T>> roots)
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
        public void ClearChildren() => KillAllChildren();


        private bool IsIndexInRange(int index) => index > 0 && index < Children.Count;
        public int CompareTo(object obj)
        {
            if (obj is not BadTreeNode<T> node) return -1;

            if ((Parent == null && node.Parent != null) || (Parent != null && node.Parent == null)) return -1;

            if (Parent == null && node.Parent == null && Data.CompareTo(node.Data) == 0) return 0;
            
            if (Data.CompareTo(node.Data) == 0
                && Parent.CompareTo(node.Parent) == 0)
            {
                return 0;
            }

            return -1;
        }
    }
}