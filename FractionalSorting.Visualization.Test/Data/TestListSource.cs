using FractionalSorting.Visualization.Data;
using System.Collections.Generic;
using System.Linq;

namespace FractionalSorting.VisualizationTest.Data
{
    class TestListSource : ISortingListSource
    {
        public TestListSource(IEnumerable<int> source)
        {
            Source = source;
        }
        public int[] Get() => Source.ToArray();
        public IEnumerable<int> Source { get; set; }
    }
}
