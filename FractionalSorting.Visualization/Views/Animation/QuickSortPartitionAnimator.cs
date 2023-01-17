using Algorithm;
using FractionalSorting.Visualization.Common;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Algorithm.QuickSort;
using static System.Windows.Visibility;

namespace FractionalSorting.Visualization.Views.Animation
{
    class QuickSortPartitionAnimator : BarChartAnimator
    {
        ItemsControl _barsPresenter;
        FrameworkElement _partitionRect;
        PartitionIndices _currentIndices;

        public QuickSortPartitionAnimator(ItemsControl barsPresenter,
                                          FrameworkElement partitionRect)
            : base(barsPresenter?.ItemContainerGenerator)
        {
            _partitionRect = partitionRect ?? throw new ArgumentNullException(nameof(partitionRect));
            _barsPresenter = barsPresenter ?? throw new ArgumentNullException(nameof(barsPresenter));
            _partitionRect.RenderTransform = new TranslateTransform();
            _barsPresenter.SizeChanged += barsPresenter_SizeChanged;
        }

        public TimeSpan TransitionDuration { get; } = TimeSpan.FromMilliseconds(100);

        public void Animate(QuickSort sort, SortStep lastStep)
        {
            int stepIndex = sort.Steps.FindIndex(lastStep);

            if (_currentIndices == null)
                _currentIndices = sort.PartitionIndicesList[0];

            if (!isStepInCurrentPartition(stepIndex))
            {
                _currentIndices = sort.FindPartitionIndices(stepIndex);
                aniamtePartitionRect(_currentIndices.PartitionStart,
                                     _currentIndices.PartitionEnd);
            }
        }

        public async Task ResetAsync()
        {
            await Task.Delay(TransitionDuration);
            await waitItemsRenderingAsync();
            int lastItemIndex = getLastItemIndex();

            if (lastItemIndex >= 0)
                aniamtePartitionRect(0, lastItemIndex);

            _currentIndices = null;
        }

        private async Task waitItemsRenderingAsync()
        {
            for (int i = 0; i < 3; i++)
                if (!IsItemsRendered())
                    await Task.Delay(20);
        }

        private void barsPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_partitionRect.Visibility == Visible
                && _partitionRect.HorizontalAlignment == HorizontalAlignment.Left)
            {
                int start = _currentIndices?.PartitionStart ?? 0;
                int end = _currentIndices?.PartitionEnd ?? getLastItemIndex();
                aniamtePartitionRect(start, end);
            }
        }

        private int getLastItemIndex()
        {
            return _barsPresenter.ItemContainerGenerator.Items.Count - 1;
        }

        private bool isStepInCurrentPartition(int stepIndex)
        {
            return stepIndex >= _currentIndices.StepStart && stepIndex <= _currentIndices.StepEnd;
        }

        private void aniamtePartitionRect(int partitionStart, int partitionEnd)
        {
            double itemWidth = GetItemWidth();
            animateWidth(itemWidth, partitionStart, partitionEnd);
            animatePosition(itemWidth, partitionStart);
        }

        private void animatePosition(double itemWidth, int partitionStart)
        {
            _partitionRect.HorizontalAlignment = HorizontalAlignment.Left;
            double leftDistance = partitionStart * itemWidth;
            var animation = new DoubleAnimation(leftDistance, TransitionDuration);
            _partitionRect.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void animateWidth(double itemWidth, int partitionStart, int partitionEnd)
        {
            _partitionRect.Width = _barsPresenter.RenderSize.Width;
            int itemsInPartitionCount = partitionEnd - partitionStart + 1;
            double newWidth = itemsInPartitionCount * itemWidth + 4;
            var animation = new DoubleAnimation(newWidth, TransitionDuration);
            _partitionRect.BeginAnimation(Border.WidthProperty, animation);
        }
    }
}
