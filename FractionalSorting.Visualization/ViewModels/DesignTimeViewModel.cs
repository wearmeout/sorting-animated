using FractionalSorting.Visualization.Data;
using System;

namespace FractionalSorting.Visualization.ViewModels
{
    public class DesignTimeViewModel : MainWindowViewModel
    {
        public DesignTimeViewModel() 
            : base(new FractionalSortingHost(), 
                   new ShuffledListSource(), 
                   new ExchangeSortsProvider())
        {
            SortingBars[getIndex()].HighlightBackground();
            SortingBars[getIndex()].HighlightBorder();
        }

        private int getIndex()
        {
            return new Random().Next(0, SortingBars.Count);
        }
    }
}
