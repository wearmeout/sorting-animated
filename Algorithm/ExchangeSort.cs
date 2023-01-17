using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithm
{
    public abstract class ExchangeSort
    {
        List<SortStep> _steps = new List<SortStep>();

        protected ExchangeSort(IEnumerable<int> list)
        {
            Source = list?.ToArray() ?? throw new ArgumentNullException(nameof(list));
            Result = Sort(list);
        }

        public IReadOnlyList<int> Source { get; }
        public virtual IReadOnlyList<int> Result { get; }
        public virtual IReadOnlyList<SortStep> Steps => _steps;
        public abstract string Name { get; }
        protected abstract IReadOnlyList<int> Sort(IEnumerable<int> list);

        protected virtual void AddStep(int leftIndex, int rightIndex, StepOperation operation)
        {
            _steps.Add(new SortStep(leftIndex, rightIndex, operation));
        }

        public static void Swap<T>(IList<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }
    }
}
