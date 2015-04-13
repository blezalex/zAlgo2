using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw2
{
    public class UnionFind
    {
        private int[] items;

        private int[] ranks;

        public UnionFind(int size)
        {
            items = new int[size];
            for (int i = 0; i < size; i++)
            {
                items[i] = i;
            }

            ranks = new int[size];
        }

        public int Find(int target)
        {
            int lookupElement = target;

            while (items[lookupElement] != lookupElement) // while not at the root
            {
                lookupElement = items[lookupElement] = items[items[lookupElement]]; // path compression
            }

            return lookupElement;
        }

        public void Union(int item1, int item2)
        {
            int item1Root = Find(item1);
            int item2Root = Find(item2);

            int smalerSetRoot = item1Root;
            int largerSetRoot = item2Root;

            int item1RootRank = ranks[item1Root];
            int item2RootRank = ranks[item2Root];

            if (item1RootRank > item2RootRank)
            {
                smalerSetRoot = item2Root;
                largerSetRoot = item1Root;
            }

            if (item1RootRank == item2RootRank)
            {
                ranks[largerSetRoot]++;
            }

            items[smalerSetRoot] = largerSetRoot;
        }
    }
}
