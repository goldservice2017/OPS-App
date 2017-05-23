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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OPS {
    public sealed partial class OrangeButton : UserControl {

        const int ColorAnimTime = 200;

        public App App = App.app;

        public string Text {
            get { return title.Text; }
            set { title.Text = value; }
        }

        public OrangeButton() {
            this.InitializeComponent();
            this.Tapped += OrangeButton_Tapped;
        }

        private void OrangeButton_Tapped(object sender, TappedRoutedEventArgs e) {
            animateColor();
        }

        public void animateColor() {
            Storyboards.ColorTransition(border, ColorAnimTime,
                ((SolidColorBrush)App.Current.Resources["Orange"]).Color,
                ((SolidColorBrush)App.Current.Resources["White"]).Color,
                null,
                "(Grid.Background).(SolidColorBrush.Color)");
        }
    }
}
