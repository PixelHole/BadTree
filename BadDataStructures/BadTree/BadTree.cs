﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace BadTree.BadDataStructures.BadTree
{
    public class BadTree<T> where T : IComparable
    {
        /// <summary>
        /// The root node of this tree
        /// </summary>
        public BadTreeNode<T> Root { get; private set; } = new BadTreeNode<T>(default(T), null);

        public int Depth { get; private set; } = 1;


        // -------- Internal Functions --------
        // -    Constructors
        
        public BadTree() {}
        public BadTree(T rootData)
        {
            Root.SetData(rootData);
        }
        public BadTree(BadTreeNode<T> root)
        {
            Root = root;
        }
        
        // -    Utility

        private void CalculateDepth()
        {
            
        }
        
        
        // -------- External Functions --------
        //      Add
        
        /// <summary>
        /// Add a range of nodes at a specific address within the tree
        /// </summary>
        /// <param name="address">the address to add the nodes at, set to null to get root</param>
        /// <param name="values">the collection of values to add at the address</param>
        public void AddRangeAt(int[]? address, T[] values)
        {
            if (values.Length == 0) return;
            
            if (address == null)
            {
                if (Depth == 1) Depth = 2;
                
                foreach (T value in values)
                {
                    Root.AddChild(value);
                }
                return;
            }

            BadTreeNode<T> node;

            try
            {
                node = GetNodeAtAddress(address);
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
            
            foreach (T value in values)
            {
                node.AddChild(value);
            }

            if (node.GetChildAt(0).Level > Depth) Depth = node.GetChildAt(0).Level;
        }
        
        /// <summary>
        /// Add a node at a specific address within the tree
        /// </summary>
        /// <param name="address">the address of the parent node</param>
        /// <param name="value">the value stored in the new node</param>
        public void AddAt(int[]? address, T value) => AddRangeAt(address, new[] { value });

        /// <summary>
        /// Add a node to the root of the tree
        /// </summary>
        /// <param name="data">The value stored in the new node</param>
        public void AddAtRoot(T data) => AddAt(null, data);
        
        /// <summary>
        /// Add a range of nodes to the root of the tree
        /// </summary>
        /// <param name="values">The values stored in the new nodes</param>
        public void AddRangeToRoot(T[] values) => AddRangeAt(null, values);
        
        
        //      Graft
        public void GraftRangeAt(int[]? address, BadTree<T>[] trees)
        {
            int maxTreeDepth = 0;
            
            if (address == null)
            {
                foreach (var tree in trees)
                {
                    if (Depth == 1)
                    {
                        if (tree.Depth > maxTreeDepth) maxTreeDepth = tree.Depth;
                    }
                    Root.GraftTree(tree.Root);
                }

                if (Depth == 1) Depth = maxTreeDepth + 1;
                
                return;
            }

            BadTreeNode<T> node;

            try
            {
                node = GetNodeAtAddress(address);
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
            
            foreach (var tree in trees)
            {
                if (tree.Depth > maxTreeDepth) maxTreeDepth = tree.Depth;
                
                node.GraftTree(tree.Root);
            }

            int newDepth = address.Length + 1 + maxTreeDepth;
            if (Depth < newDepth) Depth = newDepth;
        }
        public void GraftAt(int[]? address, BadTree<T> tree) => GraftRangeAt(address, new[] { tree });
        public void GraftAtRoot(BadTree<T> tree) => GraftRangeAt(null, new[] { tree });
        public void GraftRangeAtRoot(BadTree<T>[] trees) => GraftRangeAt(null, trees);
        
        
        //      Remove
        public void RemoveAt(int[]? address)
        {
            
        }
        
        
        //      Get
        public BadTree<T> GetSubTreeAt(int[]? address)
        {
            if (address == null) return this;

            BadTreeNode<T> root = GetNodeAtAddress(address);

            return new BadTree<T>(root);
        }
        public BadTreeNode<T> GetNodeAtAddress(int[] address)
        {
            if (address.Length > Depth) throw new IndexOutOfRangeException("the given address does not exist");

            var node = Root;

            for (int i = 0; i < address.Length; i++)
            {
                try
                {
                    node = node.GetChildAt(address[i]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException("the given address does not exist");
                }
            }

            return node;
        }


        //      Print
        public string GetFancyRepresentation()
        {
            StringBuilder result = new StringBuilder();
            
            BadTreeNode<T> node = Root;

            Queue<BadTreeNode<T>> children = new Queue<BadTreeNode<T>>();

            children.Enqueue(node);

            while (children.Count > 0)
            {
                node = children.Dequeue();
                
                result.Append($"[{node.Data}]");

                foreach (var child in node.children)
                {
                    children.Enqueue(child);
                }
            }

            return result.ToString();
        }
    }
}