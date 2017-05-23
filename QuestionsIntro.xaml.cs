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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OPS {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuestionsIntro : Page {

        public App App = App.app;

        public QuestionsIntro() {
            this.InitializeComponent();
            this.Width = Double.NaN;
            this.Height = Double.NaN;

            Storyboards.PulseAndOpacityAnimate(imageAnim, 2200, 1500, 1.3);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(HomePage));
        }

        private void NextPage(object sender, TappedRoutedEventArgs e) {
            Storyboards.DissapearLeft(grid, Storyboard_Completed);
        }

        private void Storyboard_Completed(object sender, object e) {
            this.Frame.Navigate(typeof(Questions));
        }
    }
}
