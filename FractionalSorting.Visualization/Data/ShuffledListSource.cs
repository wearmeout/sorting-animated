using Algorithm;
using System;
using System.Linq;

namespace FractionalSorting.Visualization.Data
{
    public class ShuffledListSource : ISortingListSource
    {
        int[] _list = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                        11, 12, 13, 14, 15, 16, 17, 18 };

        public int[] Get()
        {
            shuffle(_list);
            return _list.ToArray();
        }

        private void shuffle(int[] list)
        {
            Random rnd = new Random();
            int count = list.Length;

            for (var i = 0; i < count; i++)
            {
                int j = rnd.Next(i, count);
                ExchangeSort.Swap(list, i, j);
            }
        }
    }
}
