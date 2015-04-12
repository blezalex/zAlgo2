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
                lookupElement = items[lookupElement];
            }

            return lookupElement;
        }

        public void Union(int item1, int item2)
        {
            int smalerSetRoot = Find(item1);
            int largerSetRoot = Find(item2);

            if (ranks[smalerSetRoot] > ranks[largerSetRoot])
            {
                smalerSetRoot = item2;
                largerSetRoot = item1;
            }

            if (ranks[item1] == ranks[item2])
            {
                ranks[largerSetRoot]++;
            }

            items[smalerSetRoot] = largerSetRoot;
        }
    }
}
