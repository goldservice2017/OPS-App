using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace OPS {
    class Storyboards {

        const int PageDissappearTime = 1000;

        public const string
            Opacity = "Opacity",
            ScaleX = "(UIElement.RenderTransform).(CompositeTransform.ScaleX)",
            ScaleY = "(UIElement.RenderTransform).(CompositeTransform.ScaleY)",
            TranslateX = "(UIElement.RenderTransform).(CompositeTransform.TranslateX)",
            TranslateY = "(UIElement.RenderTransform).(CompositeTransform.TranslateY)";

        public static void PulseAndOpacityAnimate(UIElement obj, int totalTime, int pulseTime, double maxScale) {
            obj.RenderTransform = new CompositeTransform();
            Storyboard story = new Storyboard();
            story.RepeatBehavior = RepeatBehavior.Forever;
            story.Duration = TimeSpan.FromMilliseconds(totalTime);
            AddDoubleAnimation(obj, story, pulseTime, 1, 0, Opacity);
            AddDoubleAnimation(obj, story, pulseTime, 1, maxScale, ScaleX);
            AddDoubleAnimation(obj, story, pulseTime, 1, maxScale, ScaleY);
            story.Begin();
        }

        public static void MoveXY(UIElement obj, int time, double fromX, double fromY, double toX, double toY, EventHandler<object> onCompleted) {
            obj.RenderTransform = new CompositeTransform();
            Storyboard story = new Storyboard();
            story.Duration = TimeSpan.FromMilliseconds(time);
            story.Completed += onCompleted;
            AddDoubleAnimation(obj, story, time, fromX, toX, TranslateX);
            AddDoubleAnimation(obj, story, time, fromY, toY, TranslateY);
            story.Begin();
        }

        public static void MoveXYAndScale(UIElement obj, int time, double fromX, double fromY, double toX, double toY, double scale, int scaleTime, EventHandler<object> onCompleted) {
            obj.RenderTransform = new CompositeTransform();
            Storyboard story = new Storyboard();
            story.Duration = TimeSpan.FromMilliseconds(time);
            story.Completed += onCompleted;
            AddDoubleAnimation(obj, story, scaleTime, 1, scale, true, ScaleX);
            AddDoubleAnimation(obj, story, scaleTime, 1, scale, true, ScaleY);
            AddDoubleAnimation(obj, story, time, fromX, toX, TranslateX);
            AddDoubleAnimation(obj, story, time, fromY, toY, TranslateY);
            story.Begin();
        }

        public static void MoveXYAndFadeIn(UIElement obj, int time, double fromX, double fromY, double toX, double toY, EventHandler<object> onCompleted) {
            MoveXYAndFade(obj, time, fromX, fromY, toX, toY, time, 0, 1, false, onCompleted);
        }

        public static void MoveXYAndFadeOut(UIElement obj, int time, double fromX, double fromY, double toX, double toY, EventHandler<object> onCompleted) {
            MoveXYAndFade(obj, time, fromX, fromY, toX, toY, time, 1, 0, false, onCompleted);
        }

        public static void MoveFromXAndFadeIn(UIElement obj, int time, double fromX, EventHandler<object> onCompleted) {
            MoveXYAndFade(obj, time, fromX, 0, 0, 0, time, 0, 1, false, onCompleted);
        }

        public static void MoveToXAndFadeOut(UIElement obj, int time, double toX, EventHandler<object> onCompleted) {
            MoveXYAndFade(obj, time, 0, 0, toX, 0, time, 1, 0, false, onCompleted);
        }

        /*public static void MoveXYAndFadeIn(UIElement obj, int time, double fromX, double fromY, double toX, double toY, EventHandler<object> onCompleted) {
            obj.RenderTransform = new CompositeTransform();
            Storyboard story = new Storyboard();
            story.Duration = TimeSpan.FromMilliseconds(time);
            story.Completed += onCompleted;
            AddDoubleAnimation(obj, story, time, 0, 1, Opacity);
            AddDoubleAnimation(obj, story, time, fromX, toX, TranslateX);
            AddDoubleAnimation(obj, story, time, fromY, toY, TranslateY);
            story.Begin();
        }

        public static void MoveXYAndFadeOut(UIElement obj, int time, double fromX, double fromY, double toX, double toY, EventHandler<object> onCompleted) {
            obj.RenderTransform = new CompositeTransform();
            Storyboard story = new Storyboard();
            story.Duration = TimeSpan.FromMilliseconds(time);
            story.Completed += onCompleted;
            AddDoubleAnimation(obj, story, time, 1, 0, Opacity);
            AddDoubleAnimation(obj, story, time, fromX, toX, TranslateX);
            AddDoubleAnimation(obj, story, time, fromY, toY, TranslateY);
            story.Begin();
        }*/

        public static void MoveXYAndFade(UIElement obj, int time, double fromX, double fromY, double toX, double toY, int fadeTime, double fromO, double toO, bool reverse, EventHandler<object> onCompleted) {
            obj.RenderTransform = new CompositeTransform();
            Storyboard story = new Storyboard();
            story.Duration = TimeSpan.FromMilliseconds(time);
            story.AutoReverse = reverse;
            story.Completed += onCompleted;
            AddDoubleAnimation(obj, story, fadeTime, fromO, toO, Opacity);
            AddDoubleAnimation(obj, story, time, fromX, toX, TranslateX);
            AddDoubleAnimation(obj, story, time, fromY, toY, TranslateY);
            story.Begin();
        }

        public static void Scale(UIElement obj, int time, double from, double to, bool reverse, EventHandler<object> onCompleted) {
            Storyboard story = new Storyboard();
            story.Duration = TimeSpan.FromMilliseconds(time);
            story.Completed += onCompleted;
            story.AutoReverse = reverse;
            AddDoubleAnimation(obj, story, time, from, to, reverse, ScaleX);
            AddDoubleAnimation(obj, story, time, from, to, reverse, ScaleY);
            story.Begin();
        }

        public static void ColorTransition(UIElement obj, int time, Color from, Color to, EventHandler<object> onCompleted, string property) {
            Storyboard story = new Storyboard();
            story.Duration = TimeSpan.FromMilliseconds(time);
            story.Completed += onCompleted;
            AddColorAnimation(obj, story, time, from, to, property);
            story.Begin();
        }

        public static void Fade(UIElement obj, int time, double from, double to, EventHandler<object> onCompleted) {
            animateProperty(obj, time, from, to, onCompleted, Opacity);
        }

        public static void FadeIn(UIElement obj, int time, EventHandler<object> onCompleted) {
            obj.Visibility = Visibility.Visible;
            obj.Opacity = 0;
            animateProperty(obj, time, 0, 1, onCompleted, Opacity);
        }

        public static void FadeOut(UIElement obj, int time, EventHandler<object> onCompleted) {
            animateProperty(obj, time, 1, 0, onCompleted, Opacity);
        }

        public static void FadeOut(UIElement obj, int time, double target, EventHandler<object> onCompleted) {
            animateProperty(obj, time, 1, target, onCompleted, Opacity);
        }

        private static void animateProperty(UIElement obj, int time, double start, double end, EventHandler<object> onCompleted, string property) {
            Storyboard story = new Storyboard();
            story.Duration = TimeSpan.FromMilliseconds(time);
            story.Completed += onCompleted;
            AddDoubleAnimation(obj, story, time, start, end, property);
            story.Begin();
        }

        public static void DissapearLeft(Grid grid, EventHandler<object> handler) {
            MoveX(grid, 0, -grid.ActualWidth, PageDissappearTime, handler);
        }

        public static void DissapearRight(Grid grid, int time, EventHandler<object> handler) {
            MoveX(grid, 0, grid.ActualWidth, time, handler);
        }

        public static void DissapearRight(Grid grid, EventHandler<object> handler) {
            MoveX(grid, 0, grid.ActualWidth, PageDissappearTime, handler);
        }

        public static void DissapearBottom(Grid grid, int time, EventHandler<object> handler) {
            MoveY(grid, 0, grid.ActualHeight, time, handler);
        }

        public static void AppearBottom(Grid grid, int time, EventHandler<object> handler) {
            grid.Visibility = Visibility.Visible;
            MoveY(grid, grid.ActualHeight, 0, time, handler);
        }

        public static void AppearLeft(Grid grid, EventHandler<object> handler) {
            grid.Visibility = Visibility.Visible;
            MoveX(grid, -grid.ActualWidth, 0, PageDissappearTime, handler);
        }

        public static void AppearRight(Grid grid, EventHandler<object> handler) {
            grid.Visibility = Visibility.Visible;
            MoveX(grid, grid.ActualWidth, 0, PageDissappearTime, handler);
        }

        public static void MoveX(UIElement element, double start, double end, int millis, EventHandler<object> handler) {
            moveTransform(element, start, end, millis, "X", handler);
        }

        public static void MoveY(UIElement element, double start, double end, int millis, EventHandler<object> handler) {
            moveTransform(element, start, end, millis, "Y", handler);
        }

        public static void MoveY(UIElement element, double start, double end, int millis, bool reverse, EventHandler<object> handler) {
            moveTransform(element, start, end, millis, "Y", reverse, handler);
        }

        public static void MoveAndReverseY(UIElement element, double start, double end, int millis, EventHandler<object> handler) {
            moveTransform(element, start, end, millis, "Y", true, handler);
        }

        public static void moveTransform(UIElement element, double start, double end, int millis, string property, EventHandler<object> handler) {
            moveTransform(element, start, end, millis, property, false, handler);
        }

        public static void moveTransform(UIElement element, double start, double end, int millis, string property, bool reverse, EventHandler<object> handler) {
            Storyboard story = new Storyboard();
            story.Completed += handler;
            story.AutoReverse = reverse;

            element.RenderTransform = (Transform)new TranslateTransform();
            DoubleAnimation anim = new DoubleAnimation();
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(millis));
            anim.From = start;
            anim.To = end;
            anim.EasingFunction = ease();

            Storyboard.SetTarget(anim, (DependencyObject)element.RenderTransform);
            Storyboard.SetTargetProperty(anim, property);
            story.Children.Add((Timeline)anim);

            story.Begin();
        }

        public static void AddColorAnimation(UIElement obj, Storyboard story, int duration, Color from, Color to, String property) {
            var anim = new ColorAnimation();
            anim.Duration = TimeSpan.FromMilliseconds(duration);
            anim.From = from;
            anim.To = to;
            anim.EasingFunction = ease();

            story.Children.Add(anim);
            Storyboard.SetTarget(anim, obj);
            Storyboard.SetTargetProperty(anim, property);
        }

        public static void AddDoubleAnimation(UIElement obj, Storyboard story, int duration, double from, double to, String property) {
            AddDoubleAnimation(obj, story, duration, from, to, false, property);
        }

        public static void AddDoubleAnimation(UIElement obj, Storyboard story, int duration, double from, double to, bool reverse, String property) {
            var anim = new DoubleAnimation();
            anim.Duration = TimeSpan.FromMilliseconds(duration);
            anim.From = from;
            anim.To = to;
            anim.EasingFunction = ease();
            anim.AutoReverse = reverse;

            story.Children.Add(anim);
            Storyboard.SetTarget(anim, obj);
            Storyboard.SetTargetProperty(anim, property);
        }

        private static CubicEase ease() {
            CubicEase easingFunction = new CubicEase();
            easingFunction.EasingMode = EasingMode.EaseInOut;
            return easingFunction;
        }
    }
}
