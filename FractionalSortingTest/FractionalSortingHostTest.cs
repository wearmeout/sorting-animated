using Algorithm;
using FractionalSorting;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;
using static AlgorithmTest.ExchangeSortTest;

namespace VisualizationTest
{
    public class FractionalSortingHostTest
    {
        FakeExchangeSort _fakeSort;
        IFractionalSortingHost _sortingHost;

        public FractionalSortingHostTest()
        {
            _fakeSort = new FakeExchangeSort(
                new[] { 2, 1, 3 },
                new[] { 1, 2, 3 },
                new[]
                {
                    new SortStep(0, 1, StepOperation.Compare),
                    new SortStep(0, 1, StepOperation.Swap),
                    new SortStep(1, 2, StepOperation.Compare)
                });
            _sortingHost = new FractionalSortingHost(_fakeSort);
        }

        [Fact]
        public void GivenNullArgument_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new FractionalSortingHost(null));
            Assert.Throws<NullReferenceException>(() => _sortingHost.Algorithm = null);
        }

        [Fact]
        public void NewInstanceWithoutAlgorithm_NotFunctioning()
        {
            _sortingHost = new FractionalSortingHost();
            _sortingHost.Permutated += (s, e) => throw new XunitException();
            _sortingHost.DoNextStep();
            _sortingHost.UndoPreviousStep();
            Assert.Null(_sortingHost.NextStep);
            Assert.Null(_sortingHost.PreviousStep);
        }

        [Fact]
        public void NewInstanceWithoutAlgorithm_CurrentPermutationShouldBeEmpty()
        {
            Assert.Empty(new FractionalSortingHost().CurrentPermutation);
        }

        [Fact]
        public void SetNewAlgorithm_LoadNewSource()
        {
            FakeExchangeSort anotherSort = new FakeExchangeSort(
                new[] { 4, 3, 5 },
                new[] { 3, 4, 5 },
                new[]
                {
                    new SortStep(0, 1, StepOperation.Compare),
                    new SortStep(0, 1, StepOperation.Swap),
                    new SortStep(1, 2, StepOperation.Compare)
                });

            _sortingHost.Algorithm = anotherSort;
            Assert.Equal<int>(anotherSort.Source, _sortingHost.CurrentPermutation);
        }

        #region Event raising
        [Fact]
        public void DoNextStepOnce_EventArgReportFirstStep()
        {
            assertEventArgStep(0, () => _sortingHost.DoNextStep());
        }

        [Fact]
        public void DoNextStepTwice_EventArgReportSecondStep()
        {
            assertEventArgStep(1, () =>
            {
                _sortingHost.DoNextStep();
                _sortingHost.DoNextStep();
            });
        }

        [Fact]
        public void DoNextStepWhenCompleted_NotRaisingEvent()
        {
            runToCompletion();
            _sortingHost.Permutated += (s, e) => throw new XunitException();
            _sortingHost.DoNextStep();
        }

        [Fact]
        public void UndoPreviousStepWhenUnstarted_NotRaisingEvent()
        {
            _sortingHost.Permutated += (s, e) => throw new XunitException();
            _sortingHost.UndoPreviousStep();
        }

        [Fact]
        public void UndoPreviousStepWhenCompleted_EventArgReportLastStep()
        {
            runToCompletion();
            int lastStepIndex = _fakeSort.Steps.Count - 1;
            assertEventArgStep(lastStepIndex, () => _sortingHost.UndoPreviousStep());
        }

        [Fact]
        public void UndoAStepThenRedo_EventArgReportTheStep()
        {
            _sortingHost.DoNextStep();
            assertEventArgStep(0, () => _sortingHost.UndoPreviousStep());
        }

        [Fact]
        public void RedoAnUndoneStep_EventArgReportTheStep()
        {
            _sortingHost.DoNextStep();
            _sortingHost.UndoPreviousStep();
            assertEventArgStep(0, () => _sortingHost.DoNextStep());
        }
        #endregion

        #region CurrentPermutation
        [Fact]
        public void SortUnstarted_CurrentPermutationIsIdenticalToSource()
        {
            Assert.Equal(_fakeSort.Source, _sortingHost.CurrentPermutation);
        }

        [Fact]
        public void DoACompareStep_CurrentPermutationNotChange()
        {
            _sortingHost.DoNextStep();
            Assert.Equal(_fakeSort.Source, _sortingHost.CurrentPermutation);
        }

        [Fact]
        public void DoASwapStep_CurrentPermutationUpdated()
        {
            _sortingHost.DoNextStep();
            _sortingHost.DoNextStep();
            Assert.Equal(_fakeSort.Result, _sortingHost.CurrentPermutation);
        }

        [Fact]
        public void UndoASwapStep_CurrentPermutationRestored()
        {
            _sortingHost.DoNextStep();
            _sortingHost.DoNextStep();
            _sortingHost.UndoPreviousStep();
            Assert.Equal(_fakeSort.Source, _sortingHost.CurrentPermutation);
        }
        #endregion

        #region PreviousStep
        [Fact]
        public void SortUnstarted_PreviousStepShouldBeNull()
        {
            Assert.Null(_sortingHost.PreviousStep);
        }

        [Fact]
        public void DoNextStepOnce_PreviousStepShouldBeTheFirstStep()
        {
            _sortingHost.DoNextStep();
            assertStep(0, _sortingHost.PreviousStep);
        }

        [Fact]
        public void PreviousStepShouldBeTheLastStepWhenCompleted()
        {
            runToCompletion();
            assertStep(_fakeSort.Steps.Count - 1, _sortingHost.PreviousStep);
        }
        #endregion

        #region NextStep
        [Fact]
        public void SortUnstarted_NextStepShouldBeTheFirstStep()
        {
            assertStep(0, _sortingHost.NextStep);
        }

        [Fact]
        public void DoNextStepTwice_NextStepShouldBeTheSecondStep()
        {
            _sortingHost.DoNextStep();
            assertStep(1, _sortingHost.NextStep);
        }

        [Fact]
        public void NextStepShouldBeNullWhenCompleted()
        {
            runToCompletion();
            Assert.Null(_sortingHost.NextStep);
        }
        #endregion

        private void assertStep(int stepIndex, SortStep step)
        {
            Assert.NotNull(step);
            Assert.Equal(_sortingHost.Algorithm.Steps[stepIndex], step,
                         new TestStepComparer());
        }

        private void assertEventArgStep(int stepIndex, Action eventTrigger)
        {
            PermutatedEventArgs args = null;
            void handler(object s, PermutatedEventArgs e) => args = e;
            _sortingHost.Permutated += handler;

            eventTrigger();

            Assert.Equal(_fakeSort.Steps[stepIndex], args.Step, new TestStepComparer());
        }

        private void runToCompletion()
        {
            int count = _fakeSort.Steps.Count;

            for (int i = 0; i < count; i++)
                _sortingHost.DoNextStep();
        }

        public class FakeExchangeSort : ExchangeSort
        {
            SortStep[] _steps;

            public FakeExchangeSort(IEnumerable<int> source, int[] result, SortStep[] steps)
                : base(source)
            {
                Result = result;
                _steps = steps;
            }
            public override string Name => "Fake sort";
            public override IReadOnlyList<SortStep> Steps => _steps;
            public override IReadOnlyList<int> Result { get; }
            protected override IReadOnlyList<int> Sort(IEnumerable<int> list) => null;
            public void SetSteps(SortStep[] steps) => _steps = steps;
        }
    }
}
