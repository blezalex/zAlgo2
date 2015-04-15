using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw3
{
	class Item
	{
		public int Value;
		public int Weight;

		public Item()
		{
		}

		public Item(int value, int weight)
		{
			Value = value;
			Weight = weight;
		}

        public override string ToString()
        {
            return string.Format("Value:{0} Weight:{1}", Value, Weight);
        }
	}



	class Program
    {
        static Tuple<Item[], int> ReadKnapsackProblem(string filename)
        {
            using (var sr = File.OpenText(filename))
            {
                var infoLine = sr.ReadLine();
                var infoParts = infoLine.Split(' ');
                int itemsCount = int.Parse(infoParts[1]);

                var items = new Item[itemsCount];

                string line;
                int edgeIdx = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    var parts = line.Split(' ');

                    var item = new Item { Value = int.Parse(parts[0]), Weight = int.Parse(parts[1]) };
                    items[edgeIdx++] = item;
                    
                }

                return Tuple.Create(items, int.Parse(infoParts[0]));
            }
        }

		static void Main(string[] args)
		{
            Part2();
		}

        private static void Part1()
        {
            var problem = ReadKnapsackProblem(@"C:\Users\Alex\Desktop\algo2\knapsack1.txt");
            int answer = KnapsackNaive(problem.Item1, problem.Item2);
        }

        private static void Part2()
        {
            var problem = ReadKnapsackProblem(@"C:\Users\Alex\Desktop\algo2\knapsack_big.txt");

            var sw = new Stopwatch();
            sw.Start();
            int answer = KnapsackRecursive(problem.Item1, problem.Item1.Length, problem.Item2);
            sw.Stop();

            Console.WriteLine("{0} {1}", answer, sw.Elapsed);
        }

        static Dictionary<UInt64, int> cache = new Dictionary<UInt64, int>();

        private static int KnapsackRecursive(Item[] items, int subProblemSize, int sackSize)
        {
            if (subProblemSize <= 0)
                return 0;

            if (sackSize <= 0)
                return 0;

            UInt64 cacheKey = ((UInt64)subProblemSize) << 32 | (UInt64)sackSize;

            int result;
            if (cache.TryGetValue(cacheKey, out result))
            {
                return result;
            }

            int valueNotTakingCurrentItem = KnapsackRecursive(items, subProblemSize - 1, sackSize);
            int valueTakingCurrentItem = 0;
            int weightLeftAfterTakingCurrentItem = sackSize - items[subProblemSize - 1].Weight;

            if (weightLeftAfterTakingCurrentItem >= 0)
            {
                valueTakingCurrentItem = items[subProblemSize - 1].Value + KnapsackRecursive(items, subProblemSize - 1, weightLeftAfterTakingCurrentItem);
            }

            result = Math.Max(valueNotTakingCurrentItem, valueTakingCurrentItem);
            cache[cacheKey] = result;

            return result;
        }

		private static int KnapsackNaive(Item[] problem, int sackSize)
		{
			int[,] cache = new int[problem.Length, sackSize + 1];

			for (var i = 0; i < problem.Length; i++) // for each item
			{
				for (var x = 0; x <= sackSize; x++) // for each sackSize
				{
					var valueNotTakingCurrentItem = i > 0 ? cache[i - 1, x] : 0;
 
                    var valueOfSmallerProblemAndCurrentItem = 0;
                    var currentItemFitsIntoCurrentSack = x - problem[i].Weight >= 0;
                    if (currentItemFitsIntoCurrentSack) // if current item fits
                    {
                        valueOfSmallerProblemAndCurrentItem += problem[i].Value;

                        if (i > 0) // sack smaller by current item weight exists
                        {
                            valueOfSmallerProblemAndCurrentItem += cache[i - 1, x - problem[i].Weight];
                        }
                    }

                    cache[i, x] = Math.Max(valueNotTakingCurrentItem, valueOfSmallerProblemAndCurrentItem);
				}
			}

			return cache[problem.Length - 1, sackSize];
		}
	}
}
