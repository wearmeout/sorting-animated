using Algorithm;
using Xunit;
using static Algorithm.StepOperation;

namespace AlgorithmTest
{
    public class CombSortTest : ExchangeSortTest
    {
        [Fact]
        public void CanCreateCombSort()
        {
            int[] list = { 1, 2, 3 };
            ExchangeSort sort = new CombSort(list);
            Assert.Equal("Comb sort", sort.Name);
            Assert.Equal(list, sort.Source);
        }

        [Fact]
        public void Sort12()
        {
            int[] list = { 1, 2 };
            var sort = new CombSort(list);
            Assert.Equal(list, sort.Result);
            Assert.Collection(sort.Steps,
                s1 => AssertStep(s1, 0, 1, Compare),
                s2 => AssertStep(s2, 0, 1, Compare));
        }

        [Fact]
        public void Sort21()
        {
            int[] list = { 2, 1 };
            var sort = new CombSort(list);
            Assert.Equal(new[] { 1, 2 }, sort.Result);
            Assert.Collection(sort.Steps,
                s1 => AssertStep(s1, 0, 1, Compare),
                s2 => AssertStep(s2, 0, 1, Swap),
                s3 => AssertStep(s3, 0, 1, Compare));
        }

        [Fact]
        public void Sort123()
        {
            int[] list = { 1, 2, 3 };
            var sort = new CombSort(list);
            Assert.Equal(new[] { 1, 2, 3 }, sort.Result);
            Assert.Collection(sort.Steps,
                s1 => AssertStep(s1, 0, 2, Compare),
                s2 => AssertStep(s2, 0, 1, Compare),
                s3 => AssertStep(s3, 1, 2, Compare),
                s4 => AssertStep(s4, 0, 1, Compare),
                s5 => AssertStep(s5, 1, 2, Compare));
        }

        [Fact]
        public void Sort321()
        {
            int[] list = { 3, 2, 1 };
            var sort = new CombSort(list);
            Assert.Equal(new[] { 1, 2, 3 }, sort.Result);
            Assert.Collection(sort.Steps,
                s1 => AssertStep(s1, 0, 2, Compare),
                s2 => AssertStep(s2, 0, 2, Swap),
                s3 => AssertStep(s3, 0, 1, Compare),
                s4 => AssertStep(s4, 1, 2, Compare),
                s5 => AssertStep(s5, 0, 1, Compare),
                s6 => AssertStep(s6, 1, 2, Compare));
        }
    }
}
