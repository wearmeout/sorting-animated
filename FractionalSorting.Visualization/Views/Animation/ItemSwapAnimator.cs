using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FractionalSorting.Visualization.Views.Animation
{
    class ItemSwapAnimator : BarChartAnimator
    {
        readonly TimeSpan _stepDuration;
        readonly IEasingFunction _easingFunction;

        public ItemSwapAnimator(ItemContainerGenerator itemGenerator, TimeSpan stepDuration)
            : base(itemGenerator)
        {
            _stepDuration = stepDuration;
            _easingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut };
        }

        public void Animate(int leftIndex, int rightIndex)
        {
            double stepDistance = GetItemWidth();
            double moveDistance = stepDistance * Math.Abs(rightIndex - leftIndex);
            animateItem(leftIndex, moveDistance);
            animateItem(rightIndex, -moveDistance);
        }

        private void animateItem(int itemIndex, double distance)
        {
            var item = GetItemContainer(itemIndex);
            var translateTransform = getItemTransform<TranslateTransform>(item);
            animateTranslation(distance, translateTransform);

            double furthestScale = distance < 0 ? 1.01 : 0.99;
            var scaleTransform = getItemTransform<ScaleTransform>(item);
            animateScale(furthestScale, scaleTransform);
        }

        private void animateScale(double furthestScale, ScaleTransform transform)
        {
            var animation = new DoubleAnimationUsingKeyFrames { Duration = _stepDuration };
            animation.KeyFrames.Add(createPacedFrame(1));
            animation.KeyFrames.Add(createFrame(furthestScale, KeyTime.FromPercent(0.1)));
            animation.KeyFrames.Add(createPacedFrame(1));
            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }

        private void animateTranslation(double distance, TranslateTransform transform)
        {
            var animation = new DoubleAnimationUsingKeyFrames { Duration = _stepDuration };
            animation.KeyFrames.Add(createPacedFrame(distance));
            animation.KeyFrames.Add(createPacedFrame(0));
            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private DoubleKeyFrame createFrame(double value, KeyTime keyTime)
        {
            return new EasingDoubleKeyFrame()
            {
                Value = value,
                KeyTime = keyTime,
                EasingFunction = _easingFunction
            };
        }

        private DoubleKeyFrame createPacedFrame(double value)
        {
            return createFrame(value, KeyTime.Paced);
        }

        private T getItemTransform<T>(FrameworkElement item) where T : Transform
        {
            if (!(item.RenderTransform is TransformGroup group))
            {
                group = createTransformGroup();
                item.RenderTransform = group;
            }
            return findTransform<T>(group.Children);
        }

        private T findTransform<T>(TransformCollection transforms) where T : Transform
        {
            foreach (var child in transforms)
                if (child is T transform)
                    return transform;

            return default(T);
        }

        private TransformGroup createTransformGroup()
        {
            TransformGroup group = new TransformGroup();
            group.Children.Add(new TranslateTransform());
            group.Children.Add(new ScaleTransform());
            return group;
        }
    }
}
