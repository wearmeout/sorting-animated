using Algorithm;
using System;
using System.Collections.Generic;
using Xunit;
using static Algorithm.StepOperation;
using static AlgorithmTest.ExchangeSortTest;

namespace AlgorithmTest
{
    public class BubbleSortTest
    {
        [Fact]
        public void GivenNullArgument_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new BubbleSort(null));
        }

        [Fact]
        public void GivenAList_SourceHasTheSameSequence()
        {
            int[] list = new int[] { 1, 0, 3, 5, 2 };
            BubbleSort sort = new BubbleSort(list);
            Assert.Equal<int>(list, sort.Source);
        }

        [Fact]
        public void ProduceSortedResult()
        {
            assertSorted(intList(1), intList(1));
            assertSorted(intList(2, 1), intList(1, 2));
            assertSorted(intList(1, 3, 2), intList(1, 2, 3));
            assertSorted(intList(2, 3, 1), intList(1, 2, 3));
        }

        [Fact]
        public void SortOneItemList()
        {
            assertSteps(intList(1), new SortStep[0]);
        }

        [Fact]
        public void Sort21()
        {
            assertSteps(intList(2, 1), stepList(
                new SortStep(0, 1, Compare),
                new SortStep(0, 1, Swap)));
        }

        [Fact]
        public void Sort132()
        {
            assertSteps(intList(1, 3, 2), stepList(
                new SortStep(0, 1, Compare),
                new SortStep(1, 2, Compare),
                new SortStep(1, 2, Swap),
                new SortStep(0, 1, Compare)));
        }

        [Fact]
        public void Sort321()
        {
            assertSteps(intList(3, 2, 1), stepList(
                new SortStep(0, 1, Compare),
                new SortStep(0, 1, Swap),
                new SortStep(1, 2, Compare),
                new SortStep(1, 2, Swap),
                new SortStep(0, 1, Compare),
                new SortStep(0, 1, Swap)));
        }

        private void assertSorted(IEnumerable<int> unsorted, IEnumerable<int> sorted)
        {
            BubbleSort sort = new BubbleSort(unsorted);
            Assert.Equal(sorted, sort.Result);
        }

        private void assertSteps(IEnumerable<int> list, IEnumerable<SortStep> steps)
        {
            BubbleSort sort = new BubbleSort(list);
            Assert.Equal(steps, sort.Steps, new TestStepComparer());
        }

        private IEnumerable<int> intList(params int[] ints) => ints;
        private IEnumerable<SortStep> stepList(params SortStep[] steps) => steps;
    }
}
