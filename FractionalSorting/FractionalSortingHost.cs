using Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FractionalSorting
{
    public class FractionalSortingHost : IFractionalSortingHost
    {
        ExchangeSort _algorithm;
        List<int> _currentPermutation;
        Stack<SortStep> _doneSteps = new Stack<SortStep>();
        Stack<SortStep> _remainSteps = new Stack<SortStep>();

        public FractionalSortingHost()
        {
            _currentPermutation = new List<int>();
        }

        public FractionalSortingHost(ExchangeSort algorithm)
        {
            Algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        public event EventHandler<PermutatedEventArgs> Permutated;
        public IReadOnlyList<int> CurrentPermutation => _currentPermutation;
        public SortStep PreviousStep => _doneSteps.Any() ? _doneSteps.Peek() : null;
        public SortStep NextStep => _remainSteps.Any() ? _remainSteps.Peek() : null;

        public ExchangeSort Algorithm
        {
            get => _algorithm;
            set
            {
                _algorithm = value ?? throw new NullReferenceException();
                resetOnAlgorithmChange();
            }
        }

        private void resetOnAlgorithmChange()
        {
            _currentPermutation = _algorithm.Source.ToList();
            _doneSteps.Clear();
            _remainSteps.Clear();

            foreach (var step in _algorithm.Steps.Reverse())
                _remainSteps.Push(step);
        }

        public void DoNextStep()
        {
            if (_remainSteps.Any())
            {
                SortStep stepTodo = _remainSteps.Pop();
                applyStep(stepTodo);
                _doneSteps.Push(stepTodo);
            }
        }

        public void UndoPreviousStep()
        {
            if (_doneSteps.Any())
            {
                SortStep stepToUndo = _doneSteps.Pop();
                applyStep(stepToUndo);
                _remainSteps.Push(stepToUndo);
            }
        }

        private void applyStep(SortStep step)
        {
            RaisePermutated(step);

            if (step.Operation == StepOperation.Swap)
                ExchangeSort.Swap(_currentPermutation, step.LeftIndex, step.RightIndex);
        }

        protected virtual void RaisePermutated(SortStep doneStep)
        {
            Permutated?.Invoke(this, new PermutatedEventArgs(doneStep));
        }
    }
}
