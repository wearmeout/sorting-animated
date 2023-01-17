using FractionalSorting.Visualization.Data;
using System.Linq;
using Xunit;

namespace FractionalSorting.VisualizationTest.Data
{
    public class ShuffledListSourceTest
    {
        ISortingListSource _source;

        public ShuffledListSourceTest()
        {
            _source = new ShuffledListSource();
        }

        [Fact]
        public void GetList_ContainsAtLeast10Item()
        {
            Assert.True(_source.Get().Length >= 10);
        }
        
        [Fact]
        public void GetList_ReturnShuffledList()
        {
            int[] list1 = _source.Get();
            int[] list2 = _source.Get();

            Assert.Equal(list1.Length, list2.Length);
            Assert.All(list1, i => list2.Contains(i));
            Assert.All(list2, i => list1.Contains(i));
            Assert.NotSame(list1, list2);
            Assert.NotEqual<int>(list1, list2);
        }
    }
}
