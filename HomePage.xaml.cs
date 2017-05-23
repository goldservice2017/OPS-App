using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OPS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public App App = App.app;

        public HomePage() {
            this.InitializeComponent();
            this.Width = Double.NaN;
            this.Height = Double.NaN;

            App.windowResize();

            setArrowAnimation(arrow1, 0);
            setArrowAnimation(arrow2, 0);
        }

        private void setArrowAnimation(Image image, int beginTime) {
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(new TimeSpan(0, 0, 2));
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.AutoReverse = true;
            storyboard.SpeedRatio = 1.5;

            ((UIElement)image).RenderTransform = (Transform)new TranslateTransform();
            DoubleAnimation doubleAnimation1 = new DoubleAnimation();
            doubleAnimation1.Duration = new Duration(new TimeSpan(0, 0, 1));
            doubleAnimation1.From = 50;
            doubleAnimation1.To = 0;

            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseInOut;
            doubleAnimation1.EasingFunction = easingFunction;

            Storyboard.SetTarget((Timeline)doubleAnimation1, (DependencyObject)image.RenderTransform);
            Storyboard.SetTargetProperty((Timeline)doubleAnimation1, "X");
            ((ICollection<Timeline>)storyboard.Children).Add((Timeline)doubleAnimation1);

            storyboard.BeginTime = new TimeSpan(0, 0, 0, 0, beginTime);
            storyboard.Begin();
        }

        private void IDontKnowPage(object sender, TappedRoutedEventArgs e) {
            Storyboards.DissapearLeft(grid, toQuestionsIntro);
        }

        private void IKnowPage(object sender, TappedRoutedEventArgs e) {
            Storyboards.DissapearLeft(grid, toWhatsNew);
        }

        private void toWhatsNew(object sender, object e) {
            this.Frame.Navigate(typeof(WhatsNew));
        }

        private void toQuestionsIntro(object sender, object e) {
            this.Frame.Navigate(typeof(QuestionsIntro));
        }

    }
}
