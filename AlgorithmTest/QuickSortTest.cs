using Algorithm;
using Xunit;
using static Algorithm.QuickSort;
using static Algorithm.StepOperation;

namespace AlgorithmTest
{
    public class QuickSortTest : ExchangeSortTest
    {
        [Fact]
        public void CanCreateQuickSort()
        {
            int[] list = { 2, 1, 3 };
            ExchangeSort sort = new QuickSort(list);
            Assert.Equal("Quick sort", sort.Name);
            Assert.Equal(list, sort.Source);
        }

        #region Steps
        [Fact]
        public void GivenOneItemList_AlreadySorted()
        {
            int[] list = { 1 };
            var sort = new QuickSort(list);
            Assert.Empty(sort.Steps);
            Assert.Equal(list, sort.Result);
        }

        [Fact]
        public void GivenTwoItemsSortedList_HasOneCompareStep()
        {
            int[] list = { 1, 2 };
            var sort = new QuickSort(list);
            Assert.Collection(sort.Steps, (s) => AssertStep(s, 0, 1, Compare));
        }

        [Fact]
        public void GivenTwoItemsUnsortedList_HasTwoSteps()
        {
            int[] list = { 2, 1 };
            var sort = new QuickSort(list);
            Assert.Collection(sort.Steps,
            (s1) => AssertStep(s1, 0, 1, Compare),
            (s2) => AssertStep(s2, 0, 1, Swap));
        }

        [Fact]
        public void Sort312()
        {
            int[] list = { 3, 1, 2 };
            var sort = new QuickSort(list);
            Assert.Collection(sort.Steps,
            (s1) => AssertStep(s1, 0, 2, Compare),
            (s2) => AssertStep(s2, 1, 2, Compare),
            (s3) => AssertStep(s3, 0, 1, Swap),
            (s4) => AssertStep(s4, 1, 2, Swap));
        }

        [Fact]
        public void Sort132()
        {
            int[] list = { 1, 3, 2 };
            var sort = new QuickSort(list);
            Assert.Collection(sort.Steps,
            (s1) => AssertStep(s1, 0, 2, Compare),
            (s2) => AssertStep(s2, 1, 2, Compare),
            (s3) => AssertStep(s3, 1, 2, Swap));
        }

        [Fact]
        public void Sort231()
        {
            int[] list = { 2, 3, 1 };
            var sort = new QuickSort(list);
            Assert.Collection(sort.Steps,
            (s1) => AssertStep(s1, 0, 2, Compare),
            (s2) => AssertStep(s2, 1, 2, Compare),
            (s3) => AssertStep(s3, 0, 2, Swap),
            (s4) => AssertStep(s4, 1, 2, Compare),
            (s5) => AssertStep(s5, 1, 2, Swap));
        }

        [Fact]
        public void Sort213()
        {
            int[] list = { 2, 1, 3 };
            var sort = new QuickSort(list);
            Assert.Collection(sort.Steps,
            (s1) => AssertStep(s1, 0, 2, Compare),
            (s2) => AssertStep(s2, 1, 2, Compare),
            (s3) => AssertStep(s3, 0, 1, Compare),
            (s4) => AssertStep(s4, 0, 1, Swap));
        }
        #endregion

        #region PartitionIndices
        [Fact]
        public void Sort12_HasOnePartitionIndices()
        {
            int[] list = { 1, 2 };
            var sort = new QuickSort(list);
            Assert.Collection(sort.PartitionIndicesList,
                indices => assertPartitionIndices(indices, 0, 1, 0, 0, 1));
        }

        [Fact]
        public void Sort123_ProduceTwoPartitionIndices()
        {
            int[] list = { 1, 2, 3 };
            var sort = new QuickSort(list);
            Assert.Collection(sort.PartitionIndicesList,
                indices1 => assertPartitionIndices(indices1, 0, 2, 0, 1, 2),
                indices2 => assertPartitionIndices(indices2, 0, 1, 2, 2, 1));
        }

        [Fact]
        public void Sort213_ProduceTwoPartitionIndices()
        {
            int[] list = { 2, 1, 3 };
            var sort = new QuickSort(list);
            Assert.Collection(sort.PartitionIndicesList,
                indices1 => assertPartitionIndices(indices1, 0, 2, 0, 1, 2),
                indices2 => assertPartitionIndices(indices2, 0, 1, 2, 3, 1));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 1)]
        public void CanFindPartitionIndicesByStepIndex(int stepIndex, int indicesIndex)
        {
            int[] list = { 2, 1, 3 };
            var sort = new QuickSort(list);
            PartitionIndices result = sort.FindPartitionIndices(stepIndex);
            Assert.Equal(sort.PartitionIndicesList[indicesIndex], result);
        }

        private void assertPartitionIndices(PartitionIndices indices,
            int partitionStart, int partitionEnd, int stepStart, int stepEnd, int pivot)
        {
            Assert.Equal(partitionStart, indices.PartitionStart);
            Assert.Equal(partitionEnd, indices.PartitionEnd);
            Assert.Equal(stepStart, indices.StepStart);
            Assert.Equal(stepEnd, indices.StepEnd);
            Assert.Equal(pivot, indices.Pivot);
        }
        #endregion
    }
}
