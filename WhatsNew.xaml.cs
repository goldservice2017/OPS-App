using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OPS {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WhatsNew : Page {

        #region Declarations

        const int AnimTime = 1000;
        const int FadeTime = 500;
        const int FeaturesCount = 8;
        const int AnimTimeRepeat = 3000;
        const double HandAlpha = 0.3;
        const bool RepeatAnim = true;
        const bool StickListToAnim = false;
        const int Interval = 100;

        App App = App.app;
        bool animating = true, dragging;
        double distance;
        double spPosition, spPos;
        double startY;

        Uri[] videoSource = new Uri[FeaturesCount];

        DispatcherTimer timer = new DispatcherTimer();
        int animTime = 0;
        
        #endregion

        WhatsNew rootPage;
        public WhatsNew() {
            this.InitializeComponent();
            this.Width = Double.NaN;
            this.Height = Double.NaN;
            distance = App.Height / 4;

            rootPage = this;

            timer.Interval = TimeSpan.FromMilliseconds(Interval);
            timer.Tick += TimerTick;
            timer.Start();

            String defaultPath = (String)Application.Current.Resources["default_path"];

            for (int i = 1; i <= FeaturesCount; i++)
            {
                String filePath = defaultPath + (String)Application.Current.Resources["FeatureVideo" + i];

                videoSource[i - 1] = new Uri(filePath);
            }               
            animateHand();

            videoGrid.Opacity = 0;
            videoGrid.Visibility = Visibility.Visible;
            Storyboards.DissapearRight(videoGrid, 1, collapseVideo);

            string currentMode = App.currentLanguageMode();

            if (currentMode == "cs")
            {
                featureInkToMath.Visibility = Visibility.Collapsed;
                featureWordResearcher.Visibility = Visibility.Collapsed;
                featureSwayQuickStarter.Visibility = Visibility.Collapsed;
                featureExcellTellMe.Visibility = Visibility.Collapsed;

            } else
            {
                featureInkToMath.Visibility = Visibility.Collapsed;
                featureExcellTellMe.Visibility = Visibility.Collapsed;
            }

            App.whatsNewPage = this;
        }

        #region Animations

        private void animateHand() {
            animating = true;
            Storyboards.MoveXYAndFade(finger, AnimTime, 0, distance, 0, 0, AnimTime / 2, HandAlpha, 1.0, true, doneAnimating);
            if (StickListToAnim == true)
            {
                Storyboards.MoveY(stackPanel, 0, -distance, AnimTime, true, null);
            }                
        }

        private void collapseVideo(object sender, object e) {
            videoGrid.Opacity = 1.0;
            videoGrid.Visibility = Visibility.Collapsed;
            if (video.Source != null) video.Pause();
        }

        private void doneAnimating(object sender, object e) {
            animating = false;
        }

        private void TimerTick(object sender, object e) {
            if (!animating) {
                animTime += Interval;
                if (animTime > AnimTimeRepeat) {
                    animTime = 0;
                    animateHand();
                }
            }
        }

        #endregion

        #region Dragginng

        private void pressed(object sender, PointerRoutedEventArgs e) {
            if (animating && StickListToAnim) {
                e.Handled = true;
                return;
            }
            stackPanel.CapturePointer(e.Pointer);
            e.Handled = true;
            PointerPoint pp = e.GetCurrentPoint(this);
            startY = pp.Position.Y;
            dragging = true;
        }

        private void moved(object sender, PointerRoutedEventArgs e) {
            if (!dragging || (animating && StickListToAnim)) {
                e.Handled = true;
                return;
            }
            PointerPoint pp = e.GetCurrentPoint(this);
            spPos = pp.Position.Y - startY + spPosition;
            updatePosition();
        }

        public void updatePosition() {
            if (spPos > 0) spPos = 0;
            if (spPos < App.Height - stackPanel.ActualHeight) spPos = App.Height - stackPanel.ActualHeight;
            stackPanel.Margin = new Thickness(0, spPos, 0, 0);
        }

        private void released(object sender, PointerRoutedEventArgs e) {
            if (animating && StickListToAnim) {
                e.Handled = true;
                return;
            }
            spPosition = spPos;
            stackPanel.ReleasePointerCapture(e.Pointer);
            e.Handled = true;
            dragging = false;
        }

        #endregion

        #region Navigation

        private void ToProduct(object sender, TappedRoutedEventArgs e) {
            Storyboards.DissapearLeft(rootGrid, toProduct);
        }

        private void toProduct(object sender, object e) {
            timer.Tick -= TimerTick;
            ProductPage.BackPage = typeof(WhatsNew);
            this.Frame.Navigate(typeof(ProductPage));
        }

        private void GoHome(object sender, TappedRoutedEventArgs e) {
            Storyboards.DissapearRight(rootGrid, toHomePage);
        }

        private void toHomePage(object sender, object e) {
            timer.Tick -= TimerTick;
            this.Frame.Navigate(typeof(HomePage));
        }

        #endregion

        #region Video

        private void closeVideo(object sender, TappedRoutedEventArgs e) {
            Storyboards.DissapearRight(videoGrid, collapseVideo);
        }

        private void showVideo(object sender, TappedRoutedEventArgs e) {
            video.Source = videoSource[Int32.Parse((sender as Image).Tag.ToString())];
            video.Position = TimeSpan.FromMilliseconds(0);
            Storyboards.AppearRight(videoGrid, startVideo);            
        }

        

        private void startVideo(object sender, object e) {
            video.Play();
            video.MediaEnded += mediaEnded;
        }

        private void mediaEnded(object sender, RoutedEventArgs e) {
            closeVideo(null, null);
        }

        #endregion
    }
}
