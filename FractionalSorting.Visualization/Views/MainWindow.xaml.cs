using Algorithm;
using FractionalSorting.Visualization.Data;
using FractionalSorting.Visualization.ViewModels;
using FractionalSorting.Visualization.Views.Animation;
using System.Windows;

namespace FractionalSorting.Visualization.Views
{
    public partial class MainWindow : Window
    {
        ItemSwapAnimator _swapAnimator;
        QuickSortPartitionAnimator _partitionAnimator;

        public MainWindow()
        {
            InitializeComponent();
            var sortingHost = new FractionalSortingHost();
            MainWindowViewModel viewModel = new MainWindowViewModel(sortingHost,
                                                  new ShuffledListSource(),
                                                  new ExchangeSortsProvider());
            DataContext = viewModel;
            _swapAnimator = new ItemSwapAnimator(BarsPresenter.ItemContainerGenerator,
                                                 viewModel.StepDuration);
            _partitionAnimator = new QuickSortPartitionAnimator(BarsPresenter, PartitionBorder);
            sortingHost.Permutated += SortingHost_Permutated;
            PartitionBorder.IsVisibleChanged += PartitionBorder_IsVisibleChanged;
        }

        private async void PartitionBorder_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PartitionBorder.Visibility == Visibility.Visible)
                await _partitionAnimator.ResetAsync();
        }

        private async void SortingHost_Permutated(object sender, PermutatedEventArgs e)
        {
            if (e.Step.Operation == StepOperation.Swap)
                _swapAnimator.Animate(e.Step.LeftIndex, e.Step.RightIndex);

            var host = (FractionalSortingHost)sender;

            if (host.Algorithm is QuickSort sort)
            {
                _partitionAnimator.Animate(sort, e.Step);

                if (host.NextStep == null)
                    await _partitionAnimator.ResetAsync();
            }
        }
    }
}
