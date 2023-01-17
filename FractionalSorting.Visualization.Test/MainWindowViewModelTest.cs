using Algorithm;
using FractionalSorting.Visualization.Models;
using FractionalSorting.Visualization.Test.Common;
using FractionalSorting.Visualization.ViewModels;
using FractionalSorting.VisualizationTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Xunit;
using Xunit.Sdk;

namespace FractionalSorting.VisualizationTest
{
    public class MainWindowViewModelTest
    {
        #region Fields
        MainWindowViewModel _viewModel;
        IFractionalSortingHost _sortingHost;
        TestListSource _testListSource;
        TestAlgorithmsProvider _testAlgorithms;
        IReadOnlyList<int> _testList = new[] { 2, 1, 3 };
        IReadOnlyList<Bar> _testBars
            = new[] { new Bar(2, 0.5), new Bar(1, 0.0), new Bar(3, 1.0) };
        #endregion

        #region Constructor
        public MainWindowViewModelTest()
        {
            _sortingHost = new FractionalSortingHost();
            _testListSource = new TestListSource(_testList);
            _testAlgorithms = new TestAlgorithmsProvider();

            _viewModel = new MainWindowViewModel(_sortingHost,
                                                 _testListSource,
                                                 _testAlgorithms)
            {
                StepDuration = TimeSpan.FromMilliseconds(50)
            };
        }
        #endregion

        #region Arguments checking
        [Fact]
        public void GivenNullArgument_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new MainWindowViewModel(null,
                                        _testListSource,
                                        _testAlgorithms));

            Assert.Throws<ArgumentNullException>(() =>
                new MainWindowViewModel(_sortingHost,
                                        null,
                                        _testAlgorithms));

            Assert.Throws<ArgumentNullException>(() =>
                new MainWindowViewModel(_sortingHost,
                                        _testListSource,
                                        null));
        }
        #endregion

        #region Default behaviour
        [Fact]
        public void ShouldLoadFirstSortAlgorithm()
        {
            Assert.Equal(0, _viewModel.SelectedAlgorithmIndex);
        }

        [Fact]
        public void ShouldLoadSortingListFromSortingListSource()
        {
            assertAllSortingList(_testList);
        }

        [Fact]
        public void AlgorithmsShouldLoadedFromProvider()
        {
            var sorts = _testAlgorithms.Create(_testList);
            Assert.Equal(sorts, _viewModel.SortAlgorithms, new SortComparer());
        }
        #endregion

        #region Sorting bars
        [Fact]
        public void ConvertIntegerToProportionalValue()
        {
            Assert.Equal<Bar>(_testBars, _viewModel.SortingBars, new BarTestComparer());
        }

        [Fact]
        public void DoSteps_SortingBarsChangedAccordingly()
        {
            assertSortingBarsUpdate(() =>
            {
                _viewModel.Next();
                _viewModel.Next();
            });
        }

        [Fact]
        public void UndoSteps_SortingBarsChangedAccordingly()
        {
            _viewModel.Next();
            _viewModel.Next();
            assertSortingBarsUpdate(() => _viewModel.Previous());
        }

        [Fact]
        public void CanReloadSortingList()
        {
            int[] newList = new[] { 5, 4, 6 };
            _testListSource.Source = newList;
            _viewModel.ReloadSourceList();
            assertAllSortingList(newList);
        }

        private void assertSortingBarsUpdate(Action changePermutation)
        {
            Bar[] before = _viewModel.SortingBars.ToArray();

            changePermutation();

            Assert.NotEqual<Bar>(before, _viewModel.SortingBars);
            IEnumerable<int> actualList = _viewModel.SortingBars.Select(x => x.RawValue);
            Assert.Equal<int>(_sortingHost.CurrentPermutation, actualList);
        }

        #endregion

        #region Bars coloring
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void DoNext_OperatedBarsHighlighted(int doNextTimes)
        {
            SortStep step = null;
            _sortingHost.Permutated += (s, e) => step = e.Step;

            for (int i = 0; i < doNextTimes; i++)
                _viewModel.Next();

            assertBarsColor(step.LeftIndex, step.RightIndex);
        }

        [Fact]
        public void DoNextOnCompletion_AllBarsColorReset()
        {
            stepToCompletion();
            _viewModel.Next();
            assertBarsColor(-1);
        }

        [Fact]
        public void DoPreviousWhenUnstarted_NothingHappen()
        {
            throwOnEvents();
            _viewModel.Previous();
        }

        [Fact]
        public void DoPreviousAtStart_AllBarsColorReset()
        {
            _viewModel.Next();
            _viewModel.Previous();
            assertBarsColor(-1);
        }

        [Fact]
        public void DoPreviousCompareStep_PriorStepHighlighted()
        {
            _testListSource.Source = new[] { 4, 5, 6 };
            _viewModel.ReloadSourceList();
            _viewModel.Next();
            _viewModel.Next();
            _viewModel.Previous();
            assertBarsColor(0, 1);
        }

        [Fact]
        public void DoPreviousCompareOnCompletion_TheLastButOneStepHighlighted()
        {
            stepToCompletion();
            _viewModel.Previous();
            var steps = _sortingHost.Algorithm.Steps;
            int lastButOneStepIndex = steps.Count - 2;
            SortStep lastButOneStep = steps[lastButOneStepIndex];
            assertBarsColor(lastButOneStep.LeftIndex, lastButOneStep.RightIndex);
        }

        [Fact]
        public void DoPreviousSwapStep_SwappedBarsHighlighted()
        {
            _viewModel.Next();
            _viewModel.Next();
            _viewModel.Previous();
            assertBarsColor(0, 1);
        }

        [Fact]
        public void DoPreviousSwapStepOnCompletion_LastStepBarsHighlighted()
        {
            int[] newSource = new[] { 3, 2, 1 };
            _testListSource.Source = newSource;
            _viewModel.ReloadSourceList();
            stepToCompletion();
            _viewModel.Previous();
            assertBarsColor(0, 1);
        }

        [Fact]
        public void RunToCompletion_AllBarsColorReset()
        {
            AsyncPump.Run(() => _viewModel.RunToCompletionAsync());
            assertBarsColor(-1);
        }

        private void assertBarsColor(params int[] activeIndices)
        {
            int count = _viewModel.SortingBars.Count;

            for (int i = 0; i < count; i++)
                if (activeIndices.Contains(i))
                    assertBarColor(i, Bar.HighlightFillColor);
                else
                    assertBarColor(i, Bar.DefaultFillColor);
        }

        private void assertBarColor(int index, Color expected)
        {
            Color actucalColor = _viewModel.SortingBars[index].Fill;
            Assert.Equal(expected, actucalColor);
        }

        private void stepToCompletion()
        {
            int count = _sortingHost.Algorithm.Steps.Count;

            for (int i = 0; i < count; i++)
                _viewModel.Next();
        }
        #endregion

        #region Quick sort specialties
        [Fact]
        public void ShouldChangeBarBackgroundForQuickSortPivot()
        {
            _viewModel.SelectedAlgorithmIndex = _testAlgorithms.QuickSortIndex;
            _viewModel.Next();
            int lastBarIndex = _viewModel.SortingBars.Count - 1;
            assertBarBackground(Bar.HighlightBackgroundColor, lastBarIndex);
            assertBarBorderColor(Bar.HighlightBorderColor, lastBarIndex);
        }

        [Fact]
        public void ShouldSwitchPivotBackgroundOnPartitionChange()
        {
            _testListSource.Source = new[] { 2, 1, 3 };
            _viewModel.ReloadSourceList();
            _viewModel.SelectedAlgorithmIndex = _testAlgorithms.QuickSortIndex;
            _viewModel.Next();
            _viewModel.Next();
            _viewModel.Next();

            assertBarBackground(Bar.DefaultBackgroundColor, 0, 2);
            assertBarBorderColor(Bar.DefaultBorderColor, 0, 2);
            assertBarBackground(Bar.HighlightBackgroundColor, 1);
            assertBarBorderColor(Bar.HighlightBorderColor, 1);
        }

        [Fact]
        public void HidePivotOnCompletion()
        {
            _viewModel.SelectedAlgorithmIndex = _testAlgorithms.QuickSortIndex;
            stepToCompletion();
            _viewModel.Next();
            Assert.All(_viewModel.SortingBars, assertHasDefalutColor);
        }

        [Fact]
        public void HidePivotAtStart()
        {
            _viewModel.SelectedAlgorithmIndex = _testAlgorithms.QuickSortIndex;
            _viewModel.Next();
            _viewModel.Previous();
            _viewModel.Previous();
            Assert.All(_viewModel.SortingBars, assertHasDefalutColor);
        }

        [Fact]
        public void PartitionRectShouldBeCollapsedByDefault()
        {
            Assert.Equal(Visibility.Collapsed, _viewModel.QuickSortPartitionRectVisibility);
        }

        [Fact]
        public void PartitionRectShouldBeVisibleWhenQuickSortSelected()
        {
            _viewModel.SelectedAlgorithmIndex = _testAlgorithms.QuickSortIndex;
            Assert.Equal(Visibility.Visible, _viewModel.QuickSortPartitionRectVisibility);
        }

        [Fact]
        public void PartitionRectShouldBeCollapsedWhenSelectOtherSorts()
        {
            _viewModel.SelectedAlgorithmIndex = _testAlgorithms.QuickSortIndex;
            _viewModel.SelectedAlgorithmIndex = 0;
            Assert.Equal(Visibility.Collapsed, _viewModel.QuickSortPartitionRectVisibility);
        }

        [Fact]
        public void ShouldNotifyPartitionRectVisibilityChange()
        {
            Assert.PropertyChanged(_viewModel,
                nameof(_viewModel.QuickSortPartitionRectVisibility),
                () => _viewModel.SelectedAlgorithmIndex = _testAlgorithms.QuickSortIndex);
        }

        private void assertHasDefalutColor(Bar bar)
        {
            Assert.Equal(Bar.DefaultBackgroundColor, bar.Background);
            Assert.Equal(Bar.DefaultBorderColor, bar.BorderColor);
        }

        private void assertBarBackground(Color color, params int[] indices)
        {
            foreach (int index in indices)
                Assert.Equal(color, _viewModel.SortingBars[index].Background);
        }

        private void assertBarBorderColor(Color color, params int[] indices)
        {
            foreach (int index in indices)
                Assert.Equal(color, _viewModel.SortingBars[index].BorderColor);
        }
        #endregion

        #region Selected algorithm index
        [Fact]
        public void ShouldNotifyAlgorithmIndexChange()
        {
            assertNotified(nameof(_viewModel.SelectedAlgorithmIndex),
                                 () => _viewModel.SelectedAlgorithmIndex = 1);
        }

        [Fact]
        public void ChangeAlgorithm_SortingBarsShouldBeReset()
        {
            _viewModel.Next();
            _viewModel.SelectedAlgorithmIndex = 1;
            Assert.Equal<Bar>(_testBars, _viewModel.SortingBars, new BarTestComparer());
        }

        [Fact]
        public void ChangeAlgorithm_SortingHostShouldBeReset()
        {
            _viewModel.Next();
            _viewModel.SelectedAlgorithmIndex = 1;
            Assert.Equal(_viewModel.SortAlgorithms[1], _sortingHost.Algorithm);
            Assert.Equal<int>(_testList, _sortingHost.CurrentPermutation);
        }

        [Fact]
        public void ChangeAlgorithm_SortingListShouldNotChange()
        {
            _testListSource.Source = new[] { 5, 4, 6 };
            _viewModel.SelectedAlgorithmIndex = 1;
            assertAllSortingList(_testList);
        }

        [Fact]
        public void ShouldNotifySelectedAlgorithmChange()
        {
            assertNotified(nameof(_viewModel.SelectedAlgorithm),
                                 () => _viewModel.SelectedAlgorithmIndex = 1);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        public void GivenInvalidIndex_PreserveOldIndex(int badIndex)
        {
            _viewModel.SelectedAlgorithmIndex = 1;
            _viewModel.SelectedAlgorithmIndex = badIndex;
            Assert.Equal(1, _viewModel.SelectedAlgorithmIndex);
        }

        [Fact]
        public void AfterReload_NotifySelectedIndex()
        {
            assertNotified(nameof(_viewModel.SelectedAlgorithmIndex),
                                 () => _viewModel.ReloadCommand.Execute());
        }

        [Fact]
        public void AfterReload_NotifySelectedItem()
        {
            assertNotified(nameof(_viewModel.SelectedAlgorithm),
                                 () => _viewModel.ReloadCommand.Execute());
        }
        #endregion

        #region Run to completion
        [Fact]
        public void CanRunToCompletion()
        {
            AsyncPump.Run(() => _viewModel.RunToCompletionAsync());
            assertCompletion();
        }

        [Fact]
        public void RunAgainOnCompletion_DoNothing()
        {
            AsyncPump.Run(() => _viewModel.RunToCompletionAsync());
            throwOnEvents();
            AsyncPump.Run(() => _viewModel.RunToCompletionAsync());
            assertCompletion();
        }

        [Fact]
        public void StartRunWhileRunning_PauseRunning()
        {
            void run() => AsyncPump.Run(() => _viewModel.RunToCompletionAsync());

            void pauseRun()
            {
                Task.Delay(10).Wait();
                AsyncPump.Run(() => _viewModel.RunToCompletionAsync());
            }

            void assert()
            {
                Task.Delay(50).Wait();
                Assert.False(_viewModel.IsRunning);
            }
            Parallel.Invoke(run, pauseRun, assert);
        }

        [Fact]
        public void IsInteractableBeforeRun()
        {
            Assert.True(_viewModel.IsInteractable);
        }

        [Fact]
        public void IsNotInteractableWhileRunning()
        {
            void run() => AsyncPump.Run(() => _viewModel.RunToCompletionAsync());

            void assert()
            {
                Task.Delay(30).Wait();
                Assert.False(_viewModel.IsInteractable);
            }
            Parallel.Invoke(run, assert);
        }

        [Fact]
        public void IsInteractableWhenCompleted()
        {
            AsyncPump.Run(() => _viewModel.RunToCompletionAsync());
            Assert.True(_viewModel.IsInteractable);
        }

        [Fact]
        public void NotifyInteractabilityChange()
        {
            assertNotified(nameof(_viewModel.IsInteractable),
                           () => AsyncPump.Run(() => _viewModel.RunToCompletionAsync()));
        }

        private void throwOnEvents()
        {
            _viewModel.SortingBars.CollectionChanged += (s, e) => throw new XunitException();
            _sortingHost.Permutated += (s, e) => throw new XunitException();
        }

        private void assertCompletion()
        {
            var result = _sortingHost.Algorithm.Result;
            Assert.Equal(result, _viewModel.SortingBars.Select(x => x.RawValue));
            Assert.Equal<int>(result, _sortingHost.CurrentPermutation);
        }
        #endregion

        #region Other assertions
        private void assertNotified(string propertyName, Action change)
        {
            Assert.PropertyChanged(_viewModel, propertyName, change);
        }

        private void assertAllSortingList(IEnumerable<int> list)
        {
            Assert.Equal(list, _sortingHost.CurrentPermutation);
            Assert.Equal(list, _viewModel.SortingBars.Select(x => x.RawValue));

            foreach (var item in _viewModel.SortAlgorithms)
                Assert.Equal<int>(list, item.Source);
        }
        #endregion

        class BarTestComparer : IEqualityComparer<Bar>
        {
            public bool Equals(Bar x, Bar y)
            {
                return x.RawValue == y.RawValue &&
                       Math.Abs(x.Proportion - y.Proportion) < 0.001;
            }

            public int GetHashCode(Bar obj)
            {
                return $"{obj.RawValue} {obj.Proportion}".GetHashCode();
            }
        }
    }
}
