using Algorithm;
using FractionalSorting.Visualization.Common;
using FractionalSorting.Visualization.Data;
using FractionalSorting.Visualization.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Algorithm.QuickSort;

namespace FractionalSorting.Visualization.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields
        IFractionalSortingHost _sortingHost;
        ISortingListSource _listSource;
        IAlgorithmsProvider _algorithmsProvider;
        int _selectedAlgorithmIndex = 0;
        bool _isRunning = false;
        #endregion

        #region Constructor
        public MainWindowViewModel(IFractionalSortingHost sortingHost,
                                   ISortingListSource listSource,
                                   IAlgorithmsProvider algorithmsProvider)
        {
            _sortingHost = sortingHost ?? throw new ArgumentNullException(nameof(sortingHost));
            _listSource = listSource ?? throw new ArgumentNullException(nameof(listSource));
            _algorithmsProvider = algorithmsProvider ??
                throw new ArgumentNullException(nameof(algorithmsProvider));

            NextCommand = new DelegateCommand(Next);
            PreviousCommand = new DelegateCommand(Previous);
            ReloadCommand = new DelegateCommand(ReloadSourceList);
            RunCommand = new DelegateCommand(async () => await RunToCompletionAsync());

            ReloadSourceList();
            _sortingHost.Permutated += sortingHost_Permutated;
            StepDuration = TimeSpan.FromMilliseconds(300);
        }
        #endregion

        #region Properties
        public ObservableCollection<ExchangeSort> SortAlgorithms { get; }
            = new ObservableCollection<ExchangeSort>();

        public ObservableCollection<Bar> SortingBars { get; }
            = new ObservableCollection<Bar>();

        public ExchangeSort SelectedAlgorithm
            => SortAlgorithms.ElementAtOrDefault(SelectedAlgorithmIndex);

        public int SelectedAlgorithmIndex
        {
            get => _selectedAlgorithmIndex;
            set
            {
                if (value < 0 || value >= SortAlgorithms.Count)
                    return;

                Set(ref _selectedAlgorithmIndex, value);
                RaisePropertyChanged(nameof(SelectedAlgorithm));
                backToStart();
                checkIfHostingQuickSort();
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                RaisePropertyChanged(nameof(IsInteractable));
            }
        }

        public bool IsInteractable => !_isRunning;
        public TimeSpan StepDuration { get; set; }

        public DelegateCommand NextCommand { get; }
        public DelegateCommand PreviousCommand { get; }
        public DelegateCommand ReloadCommand { get; }
        public DelegateCommand RunCommand { get; }
        #endregion

        #region Public functions
        public async Task RunToCompletionAsync()
        {
            if (IsRunning)
            {
                IsRunning = false;
                return;
            }

            IsRunning = true;
            await runAsync(StepDuration);
            IsRunning = false;
        }

        public void ReloadSourceList()
        {
            int[] newList = _listSource.Get();
            resetSortingBars(newList);
            initializeAlgorithms(newList);
            _sortingHost.Algorithm = SelectedAlgorithm;
            RaisePropertyChanged(nameof(SelectedAlgorithmIndex));
            RaisePropertyChanged(nameof(SelectedAlgorithm));
        }

        public void Next()
        {
            if (hasNextStep())
                doNextStep();
            else
                resetBarsToDefaultColor(_sortingHost.PreviousStep);
        }

        public void Previous()
        {
            if (hasPreviousStep())
                undoPreviousStep();
            else
                resetBarsToDefaultColor(_sortingHost.NextStep);
        }
        #endregion

        #region Stepping
        private void doNextStep()
        {
            if (hasPreviousStep())
                resetBarColorByStep(_sortingHost.PreviousStep);

            _sortingHost.DoNextStep();
        }

        private void undoPreviousStep()
        {
            _sortingHost.UndoPreviousStep();
            resetBarColorByStep(_sortingHost.NextStep);

            if (hasPreviousStep())
                highlightBarColorByStep(_sortingHost.PreviousStep);
        }

        private void resetBarsToDefaultColor(SortStep step)
        {
            resetBarsHighlight();
            resetBarColorByStep(step);
        }

        private async Task runAsync(TimeSpan stepDuration)
        {
            while (IsRunning && hasNextStep())
            {
                Next();
                await Task.Delay(stepDuration);
            }
            Next();
        }

        private bool hasPreviousStep()
        {
            return _sortingHost.PreviousStep != null;
        }

        private bool hasNextStep()
        {
            return _sortingHost.NextStep != null;
        }
        #endregion

        #region Bars source
        private void resetSortingBars(IEnumerable<int> list)
        {
            SortingBars.Clear();
            var bars = calculateBars(list);

            foreach (Bar bar in bars)
                SortingBars.Add(bar);
        }

        private IEnumerable<Bar> calculateBars(IEnumerable<int> ints)
        {
            int min = ints.Min();
            int max = ints.Max();
            double denominator = max - min;

            foreach (int value in ints)
            {
                double numerator = value - min;
                double proportion = numerator / denominator;
                yield return new Bar(value, proportion);
            }
        }

        private void initializeAlgorithms(IEnumerable<int> source)
        {
            SortAlgorithms.Clear();
            var algorithms = _algorithmsProvider.Create(source);

            foreach (var item in algorithms)
                SortAlgorithms.Add(item);
        }

        private void backToStart()
        {
            resetSortingBars(SelectedAlgorithm.Source);
            _sortingHost.Algorithm = SelectedAlgorithm;
        }
        #endregion

        #region Bars coloring
        private void highlightBarColorByStep(SortStep step)
        {
            SortingBars[step.LeftIndex].HighlightFill();
            SortingBars[step.RightIndex].HighlightFill();
        }

        private void resetBarColorByStep(SortStep step)
        {
            SortingBars[step.LeftIndex].ResetFill();
            SortingBars[step.RightIndex].ResetFill();
        }
        #endregion

        #region Events
        private void sortingHost_Permutated(object sender, PermutatedEventArgs e)
        {
            highlightBarColorByStep(e.Step);

            if (e.Step.Operation == StepOperation.Swap)
                ExchangeSort.Swap(SortingBars, e.Step.LeftIndex, e.Step.RightIndex);
        }
        #endregion

        #region Quick sort specialties
        private Visibility _quickSortPartitionRectVisibility = Visibility.Collapsed;
        public Visibility QuickSortPartitionRectVisibility
        {
            get => _quickSortPartitionRectVisibility;
            set => Set(ref _quickSortPartitionRectVisibility, value);
        }

        private void checkIfHostingQuickSort()
        {
            if (isHostingQuickSort())
            {
                _sortingHost.Permutated += quickSort_Permutated;
                QuickSortPartitionRectVisibility = Visibility.Visible;
            }
            else
            {
                _sortingHost.Permutated -= quickSort_Permutated;
                QuickSortPartitionRectVisibility = Visibility.Collapsed;
            }
        }

        private bool isHostingQuickSort()
        {
            return _sortingHost.Algorithm as QuickSort != null;
        }

        private void quickSort_Permutated(object sender, PermutatedEventArgs e)
        {
            updatePivot(e.Step);
        }

        private void updatePivot(SortStep step)
        {
            resetBarsHighlight();
            var currentPartition = findPartitionIndicesByStep(step);
            SortingBars[currentPartition.Pivot].HighlightBackground();
            SortingBars[currentPartition.Pivot].HighlightBorder();
        }

        private void resetBarsHighlight()
        {
            foreach (Bar bar in SortingBars)
            {
                bar.ResetBackground();
                bar.ResetBorderColor();
            }
        }

        private PartitionIndices findPartitionIndicesByStep(SortStep step)
        {
            QuickSort sort = (QuickSort)_sortingHost.Algorithm;
            int stepIndex = sort.Steps.FindIndex(step);
            return sort.PartitionIndicesList.First(x => x.StepStart <= stepIndex
                                                     && x.StepEnd >= stepIndex);
        }
        #endregion
    }
}
