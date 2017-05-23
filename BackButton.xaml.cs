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
    public sealed partial class BackButton : UserControl {
        App App = App.app;

        public string Text {
            get { return title.Text; }
            set { title.Text = value; }
        }

        public double ImageRotation {
            get { return (backImage.RenderTransform as CompositeTransform).Rotation; }
            set { CompositeTransform ct = new CompositeTransform();
                  ct.Rotation = value;
                  backImage.RenderTransform = ct; }
        }

        public BackButton() {
            this.InitializeComponent();
        }
    }
}
