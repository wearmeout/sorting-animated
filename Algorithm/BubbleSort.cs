using System.Collections.Generic;
using System.Linq;

namespace Algorithm
{
    public class BubbleSort : ExchangeSort
    {
        public BubbleSort(IEnumerable<int> list) : base(list)
        {
        }

        public override string Name => "Bubble sort";

        protected override IReadOnlyList<int> Sort(IEnumerable<int> list)
        {
            List<int> sortList = list.ToList();

            for (int count = sortList.Count; count > 0; count--)
                for (int i = 0; count > i + 1; i++)
                    swapIfOutOfOrder(sortList, i);

            return sortList;
        }

        private void swapIfOutOfOrder(List<int> list, int index)
        {
            AddStep(index, index + 1, StepOperation.Compare);

            if (outOfOrder(list, index))
            {
                AddStep(index, index + 1, StepOperation.Swap);
                Swap(list, index, index + 1);
            }
        }

        private bool outOfOrder(List<int> list, int index)
        {
            return list[index] > list[index + 1];
        }
    }
}
