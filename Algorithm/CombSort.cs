using System.Collections.Generic;
using System.Linq;

namespace Algorithm
{
    public class CombSort : ExchangeSort
    {
        const double ShrinkFactor = 1.3;

        public CombSort(IEnumerable<int> list)
            : base(list)
        {
        }

        public override string Name => "Comb sort";

        protected override IReadOnlyList<int> Sort(IEnumerable<int> list)
        {
            int[] array = list.ToArray();
            int gap = array.Length;

            while (gap > 1)
            {
                gap = (int)(gap / ShrinkFactor);
                runPassWithGap(array, gap);
            }
            runPassWithGap(array, 1);
            return array;
        }

        private void runPassWithGap(int[] array, int gap)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int right = i + gap;

                if (right < array.Length)
                    swapIfOutOfOrder(array, i, right);
            }
        }

        private void swapIfOutOfOrder(int[] array, int left, int right)
        {
            AddStep(left, right, StepOperation.Compare);

            if (array[left] > array[right])
            {
                AddStep(left, right, StepOperation.Swap);
                Swap(array, left, right);
            }
        }
    }
}
