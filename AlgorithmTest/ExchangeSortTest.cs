using Algorithm;
using System.Collections.Generic;
using Xunit;

namespace AlgorithmTest
{
    public class ExchangeSortTest
    {
        public void AssertStep(SortStep step, int left, int right, StepOperation operation)
        {
            Assert.Equal(left, step.LeftIndex);
            Assert.Equal(right, step.RightIndex);
            Assert.Equal(operation, step.Operation);
        }

        public class TestStepComparer : IEqualityComparer<SortStep>
        {
            public bool Equals(SortStep x, SortStep y)
            {
                return x.LeftIndex == y.LeftIndex &&
                       x.RightIndex == y.RightIndex &&
                       x.Operation == y.Operation;
            }

            public int GetHashCode(SortStep obj)
            {
                return $"{obj.LeftIndex} {obj.RightIndex} {obj.Operation}".GetHashCode();
            }
        }
    }
}
