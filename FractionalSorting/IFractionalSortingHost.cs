using Algorithm;
using System;
using System.Collections.Generic;

namespace FractionalSorting
{
    public interface IFractionalSortingHost
    {
        IReadOnlyList<int> CurrentPermutation { get; }
        ExchangeSort Algorithm { get; set; }
        SortStep PreviousStep { get; }
        SortStep NextStep { get; }
        event EventHandler<PermutatedEventArgs> Permutated;
        void DoNextStep();
        void UndoPreviousStep();
    }
}