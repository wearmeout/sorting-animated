using Algorithm;
using System.Collections.Generic;

namespace FractionalSorting.Visualization.Data
{
    public interface IAlgorithmsProvider
    {
        IEnumerable<ExchangeSort> Create(IEnumerable<int> sourceList);
    }
}
