using Algorithm;
using System.Collections.Generic;

namespace FractionalSorting.Visualization.Data
{
    public class ExchangeSortsProvider : IAlgorithmsProvider
    {
        public IEnumerable<ExchangeSort> Create(IEnumerable<int> sourceList)
        {
            yield return new BubbleSort(sourceList);
            yield return new CombSort(sourceList);
            yield return new QuickSort(sourceList);
        }
    }
}
