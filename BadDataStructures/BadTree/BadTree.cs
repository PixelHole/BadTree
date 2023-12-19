#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
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

        private const int EndOfAddressNumber = -1;


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
            foreach (var node in GetEnumerator(false))
            {
                if (node.Level > Depth) Depth = node.Level;
            }
        }


        // -------- External Functions --------
        //      Add
        
        /// <summary>
        /// Add a range of nodes at a specific address within the tree
        /// </summary>
        /// <param name="address">the address to add the nodes at, set to null to get root</param>
        /// <param name="values">the collection of values to add at the address</param>
        public void AddRangeAt(int[] address, T[] values)
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
        public void AddAt(int[] address, T value) => AddRangeAt(address, new[] { value });

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
        public void GraftRangeAt(int[] address, BadTree<T>[] trees)
        {
            int maxTreeDepth = 0;
            
            // if the address is null then graft at root
            if (address == null)
            {
                foreach (var tree in trees)
                {
                    if (tree.Depth > maxTreeDepth) maxTreeDepth = tree.Depth;
                    Root.GraftTree(tree.Root);
                    
                    tree.Root.SetParent(Root);
                }

                if (maxTreeDepth + 1 > Depth) Depth = maxTreeDepth + 1;

                return;
            }

            BadTreeNode<T> node;

            try
            {
                node = GetNodeAtAddress(address);
            }
            catch (IndexOutOfRangeException)
            {
                throw;
                return;
            }
            
            foreach (var tree in trees)
            {
                if (tree.Depth > maxTreeDepth) maxTreeDepth = tree.Depth;
                
                node.GraftTree(tree.Root);
                tree.Root.SetParent(node);
            }

            int newDepth = address.Length + 1 + maxTreeDepth;
            if (Depth < newDepth) Depth = newDepth;
        }
        public void GraftAt(int[] address, BadTree<T> tree) => GraftRangeAt(address, new[] { tree });
        public void GraftAtRoot(BadTree<T> tree) => GraftRangeAt(null, new[] { tree });
        public void GraftRangeAtRoot(BadTree<T>[] trees) => GraftRangeAt(null, trees);
        
        
        //      Remove
        public void RemoveNodeAt(int[] address) => RemoveSubTreeAt(address, true);
        public void RemoveSubTreeAt(int[] address, bool keepChildren)
        {
            if (address == null)
            {
                Root.ClearChildren();
                return;
            }

            var node = GetNodeAtAddress(address);
            
            if (keepChildren) node.Parent.GraftTrees(node.Children);
            
            node.Parent.RemoveChildAt(address[address.Length - 1]);
            
            CalculateDepth();
        }
        
        
        //      Set
        public void SetRoot(BadTreeNode<T> newRoot) => Root = newRoot; 
        
        
        //      Get
        public BadTree<T> GetSubTreeAt(int[] address)
        {
            if (address == null) return this;

            BadTreeNode<T> root = GetNodeAtAddress(address);

            return new BadTree<T>(root);
        }
        public BadTreeNode<T> GetNodeAtAddress(int[] address)
        {
            //if (address.Length > Depth) throw new IndexOutOfRangeException("the given address does not exist");

            if (address == null) return Root;
            
            var node = Root;

            for (int i = 0; i < address.Length; i++)
            {
                if (address[i] == EndOfAddressNumber) break;
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
        public IEnumerable<BadTreeNode<T>> GetEnumerator(bool Forwards = true)
        {
            var enumerable = new List<BadTreeNode<T>>();
            
            BadTreeNode<T> node = Root;

            Queue<BadTreeNode<T>> children = new Queue<BadTreeNode<T>>();

            children.Enqueue(node);

            while (children.Count > 0)
            {
                node = children.Dequeue();

                enumerable.Add(node);

                foreach (var child in node.Children)
                {
                    children.Enqueue(child);
                }
            }

            if (!Forwards) enumerable.Reverse();
            
            return enumerable;
        }
        
        
        //      Find
        public int[] FindNodeAddressWithData(T data)
        {
            if (Root.Data.CompareTo(data) == 0) return null;
            
            int[] address = new int[Depth];
            var enumerator = GetEnumerator().GetEnumerator();

            BadTreeNode<T> match = null;

            enumerator.MoveNext();
            
            while (enumerator.Current != null)
            {
                if (enumerator.Current.Data.CompareTo(data) == 0)
                {
                    match = enumerator.Current;
                    break;
                }

                enumerator.MoveNext();
            }

            if (match == null) return new[] { -1 };

            address[match.Level - 1] = -1;
            
            for (int i = match.Level - 2; i >= 0; i--)
            {
                if (match.IsRoot) break;
                address[i] = match.Parent.GetIndexOfChild(match);
                match = match.Parent;
            }

            return address;
        }
        public BadTreeNode<T> FindClosestParent(BadTreeNode<T> a, BadTreeNode<T> b)
        {
            return FindClosestAncestorWithDistance(a, b).ClosestParent;
        }
        public int FindDistanceBetweenNodes(BadTreeNode<T> a, BadTreeNode<T> b)
        {
            return FindClosestAncestorWithDistance(a, b).distance;
        }
        public (BadTreeNode<T> ClosestParent, int distance) FindClosestAncestorWithDistance(BadTreeNode<T> a, BadTreeNode<T> b)
        {
            if (a.Level == 1) return (a, b.Level - 1);
            if (b.Level == 1) return (b, a.Level - 1);

            int distance = 0;
            
            while (b.Level > a.Level)
            {
                distance++;
                b = b.Parent;
            }

            while (a.Level > b.Level)
            {
                distance++;
                a = a.Parent;
            }

            while (a.CompareTo(b) != 0)
            {
                distance += 2;
                a = a.Parent;
                b = b.Parent;
            }

            return (a, distance);
        }
        
        //      Print
        public string GetFancyRepresentation(bool forwards)
        {
            StringBuilder result = new StringBuilder();
            
            BadTreeNode<T> lastNode = null; 
            
            foreach (var node in GetEnumerator(forwards))
            {
                if (lastNode != null)
                {
                    if (lastNode.Level < node.Level && forwards)
                    {
                        result.Append("\n");
                    }
                    else if (lastNode.Level > node.Level)
                    {
                        result.Append("\n");
                    }
                }
                
                result.Append($"[{node.Data}] ");

                lastNode = node;
            }

            return result.ToString();
        }
    }
}