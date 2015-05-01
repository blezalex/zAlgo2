using System;
using System.Collections.Generic;

namespace hw4
{
    public class KeyValue<TK, TV>
    {
        public TK Key;

        public TV Value;

        public KeyValue()
        {
        }

        public KeyValue(TK key, TV value)
        {
            Key = key;
            Value = value;
        }
    }

    public class DijkstraMinHeap
    {
        private KeyValue<int, int>[] Items;
        private int[] Positions; // 1 based idx 

        private int count = 0;

        public DijkstraMinHeap(int size)
        {
            Items = new KeyValue<int, int>[size];
            Positions = new int[size];
        }

        public KeyValue<int, int> ExtractMin()
        {
            if (count == 0)
                return null;

            var minEl = Items[0];

            var lastElement1Index = count;
            Swap(lastElement1Index, 1); // move last element as root

            count--;

            BubbleDown(1);
            return minEl;
        }

        // assumes that items are distinct,positive sequential
        public void DecreasePriority(int item, int newPriority)
        {
            var itemIdx = Positions[item];
            var targetItem = Items[itemIdx - 1];
            if (targetItem.Key < newPriority)
                throw new InvalidOperationException("new priority must be smaller");

            targetItem.Key = newPriority;
            BubbleUp(itemIdx);
        }

        public bool Contains(int item)
        {
            var position = Positions[item];
            return position <= count && position > 0;
        }

        public KeyValue<int, int> GetItemByValue(int value)
        {
            var itemPosition = Positions[value];
            if (itemPosition <= count && itemPosition > 0)
                return Items[itemPosition - 1];

            return null;
        }

        public void Add(KeyValue<int, int> item)
        {
            Items[count++] = item;
            var addedItemIdx = count; // 1 based idx
            Positions[item.Value] = count;

            BubbleUp(addedItemIdx);
        }

        private void BubbleDown(int elementIdx) // 1 based
        {
            while (true)
            {
                var child1Idx = GetChildIdx(elementIdx, true);
                var child2Idx = GetChildIdx(elementIdx, false);

                bool leftExists = child1Idx <= count;
                bool rightExists = child2Idx <= count;

                if (!leftExists) // in balanced tree if no left child => no children
                    return;

                var smallerIdx = !rightExists || Items[child1Idx - 1].Key < Items[child2Idx - 1].Key ? child1Idx : child2Idx;

                bool rootIsBiggerThanLeft = leftExists && Items[child1Idx - 1].Key < Items[elementIdx - 1].Key;
                bool rootIsBiggerThanRight = rightExists && Items[child2Idx - 1].Key < Items[elementIdx - 1].Key;

                if (!rootIsBiggerThanLeft && !rootIsBiggerThanRight)
                {
                    return; // done balancing
                }

                Swap(smallerIdx, elementIdx);
                elementIdx = smallerIdx;
            }
        }

        private void BubbleUp(int itemIdx) // 1 based idx
        {
            int parentIdx;
            while ((parentIdx = GetParentIdx(itemIdx)) > 0 && Items[parentIdx - 1].Key > Items[itemIdx - 1].Key) // while parent exists and smaller than currentElement
            {
                Swap(itemIdx, parentIdx);
                itemIdx = parentIdx;
            }
        }

        private void Swap(int item1Idx, int item2Idx)
        {
            var item1 = Items[item1Idx - 1];
            var item2 = Items[item2Idx - 1];

            var tmp = Positions[item1.Value];
            Positions[item1.Value] = Positions[item2.Value];
            Positions[item2.Value] = tmp;

            Swap(Items, item1Idx, item2Idx);
        }

        private static void Swap<TC>(IList<TC> collection, int item1Idx, int item2Idx)
        {
            var tmp = collection[item1Idx - 1];
            collection[item1Idx - 1] = collection[item2Idx - 1];
            collection[item2Idx - 1] = tmp;
        }

        private static int GetParentIdx(int idx)
        {
            return idx / 2;
        }

        private static int GetChildIdx(int idx, bool left)
        {
            var childIdx = idx * 2;
            if (!left)
            {
                childIdx += 1;
            }

            return childIdx;
        }
    }
}
