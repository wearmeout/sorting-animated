using System.Collections.Generic;
using System.Linq;

namespace Algorithm
{
    public class QuickSort : ExchangeSort
    {
        List<PartitionIndices> _partitionIndicesList = new List<PartitionIndices>();

        public QuickSort(IEnumerable<int> list) : base(list)
        {
        }

        public override string Name => "Quick sort";

        public IReadOnlyList<PartitionIndices> PartitionIndicesList
            => _partitionIndicesList;

        protected override IReadOnlyList<int> Sort(IEnumerable<int> list)
        {
            int[] array = list.ToArray();
            sortRecursively(array, 0, array.Length - 1);
            return array;
        }

        private void sortRecursively(int[] array, int start, int end)
        {
            if (start >= end)
                return;

            int stepStart = Steps.Count;
            int pivot = partition(array, start, end);
            int stepEnd = Steps.Count - 1;

            _partitionIndicesList.Add(new PartitionIndices
            {
                Pivot = end,
                PartitionStart = start,
                PartitionEnd = end,
                StepStart = stepStart,
                StepEnd = stepEnd
            });

            sortRecursively(array, start, pivot - 1);
            sortRecursively(array, pivot + 1, end);
        }

        private int partition(int[] array, int lower, int upper)
        {
            int pivot = upper;
            int index = lower;

            for (int i = lower; i < pivot; i++)
            {
                if (lessThanPivot(array, i, pivot))
                {
                    doSwap(array, index, i);
                    index++;
                }
            }
            doSwap(array, index, pivot);
            return index;
        }

        private bool lessThanPivot(int[] array, int right, int pivot)
        {
            AddStep(right, pivot, StepOperation.Compare);
            return array[pivot] > array[right];
        }

        private void doSwap(int[] array, int index1, int index2)
        {
            if (index1 != index2)
            {
                AddStep(index1, index2, StepOperation.Swap);
                Swap(array, index1, index2);
            }
        }

        public PartitionIndices FindPartitionIndices(int stepIndex)
        {
            return _partitionIndicesList.First(x => x.StepStart <= stepIndex
                                                 && x.StepEnd >= stepIndex);
        }

        public class PartitionIndices
        {
            public int PartitionStart { get; internal set; }
            public int PartitionEnd { get; internal set; }
            public int StepStart { get; internal set; }
            public int StepEnd { get; internal set; }
            public int Pivot { get; internal set; }
        }
    }
}
