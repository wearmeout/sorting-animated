namespace Algorithm
{
    public enum StepOperation
    {
        Compare,
        Swap
    }

    public class SortStep
    {
        public SortStep(int leftIndex, int rightIndex, StepOperation operation)
        {
            LeftIndex = leftIndex;
            RightIndex = rightIndex;
            Operation = operation;
        }

        public int LeftIndex { get; }
        public int RightIndex { get; }
        public StepOperation Operation { get; }
    }
}
