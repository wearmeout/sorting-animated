using System.Collections.Generic;
using Algorithm;
using FractionalSorting.Visualization.Data;

namespace FractionalSorting.VisualizationTest.Data
{
    class TestAlgorithmsProvider : IAlgorithmsProvider
    {
        public IEnumerable<ExchangeSort> Create(IEnumerable<int> sourceList)
        {
            yield return new BubbleSort(sourceList);
            yield return new BubbleSort(sourceList);
            yield return new QuickSort(sourceList);
            yield return new DummySort(sourceList);
        }

        public int QuickSortIndex => 2;

        class DummySort : ExchangeSort
        {
            public DummySort(IEnumerable<int> list) : base(list) { }
            public override string Name => "Dummy sort";
            protected override IReadOnlyList<int> Sort(IEnumerable<int> list) => null;
        }
    }

    class SortComparer : IEqualityComparer<ExchangeSort>
    {
        public bool Equals(ExchangeSort x, ExchangeSort y)
            => x.GetType().Equals(y.GetType());

        public int GetHashCode(ExchangeSort obj)
            => obj.GetHashCode();
    }
}
