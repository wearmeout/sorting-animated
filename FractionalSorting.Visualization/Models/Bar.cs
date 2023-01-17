using FractionalSorting.Visualization.Common;
using System.Windows;
using System.Windows.Media;

namespace FractionalSorting.Visualization.Models
{
    public class Bar : BindableBase
    {
        public static Color DefaultFillColor { get; } = Colors.Silver;
        public static Color HighlightFillColor { get; } = Colors.LightGreen;
        public static Color HighlightBackgroundColor { get; } = Color.FromRgb(209, 255, 209);
        public static Color DefaultBackgroundColor { get; } = Colors.Transparent;
        public static Color DefaultBorderColor { get; } = Colors.DimGray;
        public static Color HighlightBorderColor { get; } = Colors.Green;

        Color _fill;
        Color _background;
        private Color _borderColor;

        public Bar(int rawValue, double proportion)
        {
            RawValue = rawValue;
            Proportion = proportion;
            Height = new GridLength(proportion, GridUnitType.Star);
            ComplementalHeight = new GridLength(1 - proportion, GridUnitType.Star);
            Fill = DefaultFillColor;
            Background = DefaultBackgroundColor;
            BorderColor = DefaultBorderColor;
        }

        public int RawValue { get; }
        public double Proportion { get; }
        public GridLength Height { get; }
        public GridLength ComplementalHeight { get; }
        public Color Fill
        {
            get => _fill;
            set => Set(ref _fill, value);
        }

        public Color Background
        {
            get => _background;
            set => Set(ref _background, value);
        }

        public Color BorderColor
        {
            get => _borderColor;
            set => Set(ref _borderColor, value);
        }

        public void ResetFill()
        {
            Fill = DefaultFillColor;
        }

        public void HighlightFill()
        {
            Fill = HighlightFillColor;
        }

        public void ResetBackground()
        {
            Background = DefaultBackgroundColor;
        }

        public void HighlightBackground()
        {
            Background = HighlightBackgroundColor;
        }

        public void HighlightBorder()
        {
            BorderColor = HighlightBorderColor;
        }

        public void ResetBorderColor()
        {
            BorderColor = DefaultBorderColor;
        }
    }
}
