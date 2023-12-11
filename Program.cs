using System;
using BadTree.BadDataStructures.BadTree;

namespace BadTree
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            BadTree<int> test = new BadTree<int>(1);

            test.AddRangeToRoot(new []{2, 3});
            
            test.AddRangeAt(new []{0}, new []{4, 5});

            Console.WriteLine(test.GetFancyRepresentation(true));
            Console.WriteLine();
        }
    }
}