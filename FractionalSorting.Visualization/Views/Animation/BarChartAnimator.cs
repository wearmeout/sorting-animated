using System;
using System.Windows;
using System.Windows.Controls;

namespace FractionalSorting.Visualization.Views.Animation
{
    abstract class BarChartAnimator
    {
        readonly ItemContainerGenerator _itemGenerator;

        protected BarChartAnimator(ItemContainerGenerator itemGenerator)
        {
            _itemGenerator = itemGenerator ?? throw new ArgumentNullException(nameof(itemGenerator));
        }

        protected double GetItemWidth()
        {
            if (_itemGenerator.Items.Count == 0)
                return 0;

            var itemContainer = GetItemContainer(0);
            var itemMargin = itemContainer.Margin;
            return itemContainer.RenderSize.Width + itemMargin.Left + itemMargin.Right;
        }

        protected bool IsItemsRendered()
        {
            return GetItemContainer(0)?.RenderSize.Width > 1;
        }

        protected FrameworkElement GetItemContainer(int index)
        {
            return (FrameworkElement)_itemGenerator.ContainerFromIndex(index);
        }
    }
}
