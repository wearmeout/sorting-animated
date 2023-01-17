using Algorithm;
using System;

namespace FractionalSorting
{
    public class PermutatedEventArgs : EventArgs
    {
        public PermutatedEventArgs(SortStep step)
        {
            Step = step;
        }

        public SortStep Step { get; }
    }
}
