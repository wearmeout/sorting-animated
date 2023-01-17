using FractionalSorting.Visualization.Models;
using System.Windows;
using System.Windows.Media;
using Xunit;

namespace FractionalSorting.Visualization.Test.Models
{
    public class BarTest
    {
        const double _proportion = 1.0 / 3.0;
        Bar _bar;

        public BarTest()
        {
            _bar = new Bar(1, _proportion);
        }

        [Fact]
        public void BarHeightRepresentProportionalGridHeight()
        {
            assertHeight(_proportion, _bar.Height);
        }

        [Fact]
        public void ComplementalHeightAddBarHeightEqualsOneStar()
        {
            assertHeight(1 - _proportion, _bar.ComplementalHeight);
        }

        [Fact]
        public void FillSilverColorByDefault()
        {
            Assert.Equal(Colors.Silver, _bar.Fill);
        }

        [Fact]
        public void Highlighted_FillIsLightGreen()
        {
            _bar.HighlightFill();
            Assert.Equal(Colors.LightGreen, _bar.Fill);
        }

        [Fact]
        public void CanResetToDefaultFill()
        {
            _bar.ResetFill();
            Assert.Equal(Colors.Silver, _bar.Fill);
        }

        [Fact]
        public void ShouldNotifyFillChange()
        {
            Assert.PropertyChanged(_bar, nameof(_bar.Fill),
                                   () => _bar.HighlightFill());
        }

        [Fact]
        public void HasDefaultBackgroundColor()
        {
            Assert.Equal(Colors.Transparent, _bar.Background);
        }

        [Fact]
        public void CanHighlightBackground()
        {
            _bar.HighlightBackground();
            Assert.Equal(Bar.HighlightBackgroundColor, _bar.Background);
        }

        [Fact]
        public void CanResetBackground()
        {
            _bar.HighlightBackground();
            _bar.ResetBackground();
            Assert.Equal(Bar.DefaultBackgroundColor, _bar.Background);
        }

        [Fact]
        public void ShouldNotifyBackgroundChange()
        {
            Assert.PropertyChanged(_bar, nameof(_bar.Background),
                                   () => _bar.HighlightBackground());
        }

        [Fact]
        public void HasDefaultBorderColor()
        {
            Assert.Equal(Colors.DimGray, _bar.BorderColor);
        }

        [Fact]
        public void CanHighlightBorder()
        {
            _bar.HighlightBorder();
            Assert.Equal(Colors.Green, _bar.BorderColor);
        }

        [Fact]
        public void CanResetBorderColor()
        {
            _bar.HighlightBorder();
            _bar.ResetBorderColor();
            Assert.Equal(Bar.DefaultBorderColor, _bar.BorderColor);
        }

        [Fact]
        public void ShouldNotifyBorderColorChange()
        {
            Assert.PropertyChanged(_bar, nameof(_bar.BorderColor),
                                   () => _bar.HighlightBorder());
        }

        private void assertHeight(double proportion, GridLength height)
        {
            GridLength expected = new GridLength(proportion, GridUnitType.Star);
            Assert.Equal(expected, height);
        }
    }
}
