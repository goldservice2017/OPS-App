using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Core;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Input;
using Windows.UI.Xaml.Navigation;

namespace OPS {
    public sealed partial class ProductPage : Page {

        #region Declarations
        public static Type BackPage = typeof(Questions);
        public static Type HomePage = typeof(HomePage);
        public static bool FadeoutCircle = false;

        const double TWOPI = 6.2831853071795865;
        const double RAD2DEG = 57.2957795130823209;

        const int CenterGridSplitTime = 900;
        const int CenterGridScaleUpTime = 150;
        const double CenterGridScaleUp = 1.025;
        const int AppearTime = 500;
        const int FadeTime = 200;
        const int StartVideoCloseAt = 2100;
        const double AnimDivider = 3.3;
        const double yShift = 0.07, xShift = 0.23, xShiftCompareFactor = 0.52;
        const int WaveShift = 60;
        const double WaveScale = 1.4;
        const int WaveTime = 220;
        const double WaveYFactor = 0.5;
        const double GrayedOpacity = 0.33;

        App App = App.app;
        DispatcherTimer timer = new DispatcherTimer();
        Stopwatch stopwatch = new Stopwatch();
        int Interval = 16;
        MediaPlayer mediaplayer;
        bool videoClosed = false;
        int currentProduct = 0, lastProduct = 0;
        bool animating;
        int NrOfAssets, NrOfProducts;
        bool useScaleTargets = false;
        bool featuresDetailsMode = false;
        bool centerGridAnimating = false;
        bool centerGridShowCompare = true;
        bool useScalingOnCenterGridAnimation = false;
        double cgTargetRotation = 0;

        double aImgSize, aImgSpan, aImgYOffset, aImgXOffset;

        string[] productName = new string[6];
        string[] productInfoBoxDescription = new string[6];
        string[] productInfoBoxName = new string[6];
        string[] productDeviceInfo = new string[6];
        string[] productDeviceTitle = new string[6];

        public static bool[][] ProductFeatures = new bool[][]{
            //          Word  Excel PPT     1Note Outlook  Pub    Acc    1TB    Skype  UTD    Support   5 Users Mac    PC
            //==================================================================================================================================
            new bool[]{ true, true, true,   true, true,    true,  true,  true,  true,  true,  true,     true,   true,  true},  // 365  Home
            new bool[]{ true, true, true,   true, true,    true,  true,  true,  true,  true,  true,     false,  true,  true},  // 365  Personal
            new bool[]{ true, true, true,   true, true,    false, false, false, false, false, false,    false,  false, true},  // 2016 Home & Bussiness  PC
            new bool[]{ true, true, true,   true, false,   false, false, false, false, false, false,    false,  false, true},  // 2016 Home & Student    PC
            new bool[]{ true, true, true,   true, true,    false, false, false, false, false, false,    false,  true,  false}, // 2016 Home & Bussiness  Mac
            new bool[]{ true, true, true,   true, false,   false, false, false, false, false, false,    false,  true,  false}  // 2016 Home & Student    Mac
        };

        ImageSource[] colorAssetSource, grayAssetSource;

        Image[] productImages, assetImages, assetDummy;
        //Image[] comparisonImages;
        Point[] xyTargets, xyScaleTargets;
        Grid[] comparisonGrids;

        BitmapImage personalDevices, homeDevices, singleDevice;

        enum CompareMode { Full, Compare1, Compare2 };
        CompareMode compareMode = CompareMode.Full;

        #endregion

        #region Debug
        const bool debug = true;
        void dbg(string str) { if (debug) Debug.WriteLine(str); }
        void dbg(string str, params object[] args ) { if (debug) Debug.WriteLine(str, args); }
        #endregion

        #region Initializations
        public ProductPage() {
            Debug.WriteLine("Production Page.");

            this.InitializeComponent();
            this.Width = Double.NaN;
            this.Height = Double.NaN;

            for (int i = 0; i < 6; i++) {
                string str = (i + 1).ToString();
                productName[i] = App.ResourceLoader.GetString("Product" + str);
                productInfoBoxDescription[i] = App.ResourceLoader.GetString("ProductInfoDescription" + str);
                productInfoBoxName[i] = App.ResourceLoader.GetString("ProductTitle" + str);
                productDeviceInfo[i] = App.ResourceLoader.GetString("ProductDeviceInfo" + str);
                productDeviceTitle[i] = App.ResourceLoader.GetString("ProductDeviceTitle" + str);
            }

            assetDummy = new Image[] { dummyUsers, dummyDevice, aDummy1, aDummy2, aDummy3, aDummy4, aDummy5, aDummy6, aDummy7, aDummy8, aDummy9, aDummy10 };
            comparisonGrids = new Grid[] { comparisonGrid0, comparisonGrid1, comparisonGrid3, comparisonGrid2, comparisonGrid5, comparisonGrid4 };
            //comparisonImages = new Image[] { comparisonImage0, comparisonImage1, comparisonImage2, comparisonImage3, comparisonImage4, comparisonImage5 };
            productImages = new Image[] { pImg1, pImg2, pImg3, pImg4, pImg5, pImg6 };
            assetImages = new Image[] { aUsers, aDevice, aImg1, aImg2, aImg3, aImg4, aImg5, aImg6, aImg7, aImg8, aImg9, aImg10 };
            NrOfAssets = assetImages.Length;
            NrOfProducts = productName.Length;
            xyTargets = new Point[NrOfAssets];
            xyScaleTargets = new Point[NrOfAssets];
            colorAssetSource = new ImageSource[NrOfAssets];
            grayAssetSource = new ImageSource[NrOfAssets];

            for (int i = 0; i < NrOfAssets; i++) {
                colorAssetSource[i] = assetImages[i].Source;
                if (i >= 2) grayAssetSource[i] = (ImageSource)Application.Current.Resources["GrayAsset" + (i - 1)];
                assetDummy[i].Opacity = 0.0;
                assetImages[i].RenderTransform = new CompositeTransform();
                assetImages[i].Tag = i;
                xyTargets[i] = new Point();
                xyScaleTargets[i] = new Point(1, 1);
            }

            homeButton.Opacity = 0;
            infoIcon.Opacity = 0;
            whatsIncludedBtn.Opacity = 0;
            video.Opacity = 0;
            centerGrid.Opacity = 0;
            productsPanel.Visibility = Visibility.Collapsed;

            centerVideo();
            video.Source = MediaSource.CreateFromUri(new Uri((String)Application.Current.Resources["CircleFlipVideo"]));
            mediaplayer = video.MediaPlayer;
            mediaplayer.Play();

            timer.Interval = TimeSpan.FromMilliseconds(Interval);
            timer.Tick += Timer_Tick;
            timer.Start();

            if (App.SelectedProduct >= 0) {
                currentProduct = App.SelectedProduct;
                lastProduct = currentProduct;
            }
            productImage.Source = productImages[currentProduct].Source;
            productTitle.Text = productName[currentProduct];
            productImages[currentProduct].Opacity = 0.5;
            setProductHeader();
            setUsersImage();

            infoBox.Visibility = Visibility.Collapsed;
            setInfoBoxText(currentProduct);

            subscriptionVsOneTimeGrid.Opacity = 0;

            App.productsPage = this;
            updatePositions();
            initAssets();

            // Slide in grids animation preparation / workaround
            assetDetails.Opacity = 0;
            assetDetails.Visibility = Visibility.Visible;
            Storyboards.DissapearRight(assetDetails, restoreOpacity);
            comparisonGrid.Opacity = 0;
            comparisonGrid.Visibility = Visibility.Visible;
            Storyboards.DissapearRight(comparisonGrid, null);
            featuresDetails.Opacity = 0;
            featuresDetails.Visibility = Visibility.Visible;
            Storyboards.DissapearBottom(featuresDetails, 100, null);

            // Fade in video
            Storyboards.FadeIn(video, AppearTime * 2, null);
            if (FadeoutCircle)
                Storyboards.FadeOut(questionCircle, AppearTime, null);
            else
                questionCircle.Opacity = 0;
            FadeoutCircle = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            //base.OnNavigatedTo(e);
            if (Transitions != null) Transitions.Clear();
            App.windowResize();
            stopwatch.Start();
        }

        private void loadSources() {
            personalDevices = new BitmapImage(new Uri((String)Application.Current.Resources["DevicesPersonal"]));
            homeDevices = new BitmapImage(new Uri((String)Application.Current.Resources["DevicesHome"]));
            singleDevice = new BitmapImage(new Uri((String)Application.Current.Resources["DevicesSingle"]));
        }

        private void restoreOpacity(object sender, object e) {
            assetDetails.Opacity = 1.0;
            assetDetails.Visibility = Visibility.Collapsed;
            comparisonGrid.Opacity = 1.0;
            comparisonGrid.Visibility = Visibility.Collapsed;
            featuresDetails.Opacity = 1.0;
            featuresDetails.Visibility = Visibility.Collapsed;
        }

        private void centerVideo() {
            if (videoClosed) return;
            double aspect = 1920.0 / 1080.0;
            double scale = App.InnerCircleScale;
            double offsetX = 0.16;
            double offsetY = 0.06;
            video.Height = App.Height * scale;
            video.Width = video.Height * aspect;
            video.Margin = new Thickness(App.Height * offsetX, App.Height * offsetY, 0, 0); // Center fix
        }
        #endregion

        #region Product dragging
        double cgStartX, cgStartY, startY, startX, cg;
        CompositeTransform cgTransform;
        double cgTargetX, cgTargetY, cgStartAngle;
        Point cgStartPos;
        bool dragging = false;
        bool cgMoving = false;
        bool cgIsRotating = false;

        private void cgPressed(object sender, PointerRoutedEventArgs e) {
            if (centerGridAnimating || cgMoving) {
                e.Handled = true;
                dragging = false;
                dbg("Pressed Point - not dragging");
                return;
            }

            dbg("Pressed Point - dragging");
            centerGrid.CapturePointer(e.Pointer);
            e.Handled = true;
            PointerPoint pp = e.GetCurrentPoint(this);
            startX = pp.Position.X;
            startY = pp.Position.Y;
            dragging = true;
            cgTransform = centerGrid.RenderTransform as CompositeTransform;
            cgStartX = cgTransform.TranslateX;
            cgStartY = cgTransform.TranslateY;
            cgTargetX = cgStartX;
            cgTargetY = cgStartY;

            Point cgPos = centerGrid.TransformToVisual(rootGrid).TransformPoint(new Point(0, 0));
            cgStartPos = new Point(cgPos.X, cgPos.Y);
            double radius = centerGrid.ActualHeight / 2;
            double cgX = startX - cgPos.X - radius;
            double cgY = startY - cgPos.Y - radius;
            double distance = Math.Sqrt(cgX * cgX + cgY * cgY);
            cgIsRotating = distance > radius;
            //cgStartAngle = bearing(0, radius, cgX, cgY);
            cgStartAngle = Math.Atan2(cgY, cgX) * RAD2DEG;
            //Debug.WriteLine("x = " + cgPos.X + " y = " + cgPos.Y + " cgx = " + cgX + "  cgy = " + cgY + " distance = " + distance);
        }

        private void cgMoved(object sender, PointerRoutedEventArgs e) {
            if (!dragging || centerGridAnimating) {
                e.Handled = true;
                return;
            }
            PointerPoint pp = e.GetCurrentPoint(this);
            cgMoving = true;

            if (cgIsRotating) {
                Point cgPos = centerGrid.TransformToVisual(rootGrid).TransformPoint(new Point(0, 0));
                double radius = centerGrid.ActualHeight / 2;
                double cgX = pp.Position.X - cgStartPos.X - radius;
                double cgY = pp.Position.Y - cgStartPos.Y - radius;
                //cgTargetRotation = cgStartAngle - bearing(0, radius, cgX, cgY);
                double currentRotation = Math.Atan2(cgY, cgX) * RAD2DEG;
                cgTargetRotation = currentRotation - cgStartAngle;
                //Debug.WriteLine("cgStartPos = " + (int)cgStartPos.X + "," + (int)cgStartPos.Y + "cgX = " + cgX + " - cgY = " + cgY + 
                //  " / start = " + (int)cgStartAngle + " - current = " + (int)currentRotation + " - target = " + (int)cgTargetRotation);

                cgTargetX = cgStartX + (pp.Position.X - startX) / 1.5;
                cgTargetY = cgStartY + (pp.Position.Y - startY) / 2.5;

            } else {
                cgTargetX = cgStartX + pp.Position.X - startX;
                cgTargetY = cgStartY + pp.Position.Y - startY;
            }
        }

        private void cgReleased(object sender, PointerRoutedEventArgs e) {
            dbg("Release - enter");

            if (dragging) {
                centerGrid.ReleasePointerCapture(e.Pointer);
                e.Handled = true;
                dragging = false;
                dbg("Release - dragging handled");
            }
            else {
                dbg("Release - not dragging - return");
                return;
            }

            if (centerGridAnimating) {
                dbg("Release - return: move = " + cgMoving + " / animate = " + centerGridAnimating);
                return;
            }
            PointerPoint pp = e.GetCurrentPoint(this);
            cgTargetRotation = 0;
            dbg("Release - is move || animate");

            if (startX == pp.Position.X && startY == pp.Position.Y) {
                dbg("Release - toggle and return");
                toggleCenterGrid();
                return;
            }

            CompositeTransform ct = centerGrid.RenderTransform as CompositeTransform;
            double toX = -App.Width * xShift;
            double toY = -App.Height * yShift;
            centerGridAnimating = true;
            cgMoving = true;
            dbg("Release - compare = " + centerGridShowCompare);
            if (centerGridShowCompare) {
                Storyboards.MoveXY(subscriptionVsOneTimeGrid, CenterGridSplitTime, -toX * xShiftCompareFactor, toY, 0, toY, null);
                Storyboards.FadeOut(subscriptionVsOneTimeGrid, CenterGridSplitTime, finishCenterGridAnimation);
                dbg("Release - cgTargetX = " + cgTargetX);
                /*if (cgTransform != null && Math.Abs(cgTargetX) < 5) {
                    dbg("Release Point - set TranslateX" + toX);
                    cgTransform.TranslateX = toX;
                    cgTransform.TranslateY = toY;
                }*/
                cgTargetX = 0;
                cgTargetY = toY;
                centerGridShowCompare = false;
                dbg("Release - hide compare");
            }
            else {
                Storyboards.MoveXY(subscriptionVsOneTimeGrid, CenterGridSplitTime, 0, toY, -toX * xShiftCompareFactor, toY, null);
                Storyboards.FadeIn(subscriptionVsOneTimeGrid, CenterGridSplitTime, finishCenterGridAnimation);
                dbg("Release - cgTargetX = " + cgTargetX);
                /*if (cgTransform != null && Math.Abs(cgTargetX) < 5) {
                    dbg("Release Point - set TranslateX = 0");
                    cgTransform.TranslateX = 0;
                    cgTransform.TranslateY = toY;
                }*/
                cgTargetX = toX;
                cgTargetY = toY;
                centerGridShowCompare = true;
                dbg("Release - show compare");
            }
        }

        double bearing(double a1, double a2, double b1, double b2) {
            if (a1 == b1 && a2 == b2) return 0;
            double theta = Math.Atan2(b1 - a1, a2 - b2);
            if (theta < 0.0)
                theta += TWOPI;
            return RAD2DEG * theta;
        }
        #endregion

        private void setInfoBoxText(int id) {
            infoBox.Title = String.Format(App.ResourceLoader.GetString("ProductInfoBoxTitleFormat"), productInfoBoxName[id]);
            infoBox.Description = productInfoBoxDescription[id];
            featuresProductTitle.Text = productDeviceTitle[id];
            featuresProductDescription.Text = productDeviceInfo[id];
        }

        private void setProductHeader() {
            productHeader.Text = App.SelectedProduct == currentProduct ?
                App.ResourceLoader.GetString("ProductHeaderIfSelected") : App.ResourceLoader.GetString("ProductHeader");
        }

        private void Timer_Tick(object sender, object e) {
            if (!videoClosed && stopwatch.ElapsedMilliseconds > StartVideoCloseAt && !centerGridAnimating) {
                stopwatch.Stop();
                centerGridAnimating = true;
                Storyboards.FadeIn(centerGrid, AppearTime, moveCenterGridUp);
            }

            if (videoClosed) {
                bool snapScale = false;
                for (int i = 0; i < NrOfAssets; i++) {
                    CompositeTransform ct = assetImages[i].RenderTransform as CompositeTransform;
                    ct.TranslateX += (xyTargets[i].X - ct.TranslateX) / AnimDivider;
                    ct.TranslateY += (xyTargets[i].Y - ct.TranslateY) / AnimDivider;
                    if (useScaleTargets) {
                        ct.ScaleX += (xyScaleTargets[i].X - ct.ScaleX) / AnimDivider;
                        ct.ScaleY += (xyScaleTargets[i].Y - ct.ScaleY) / AnimDivider;
                        if (Math.Abs(xyScaleTargets[i].X - ct.ScaleX) < 0.02) snapScale = true;
                    }
                }

                if (snapScale) {
                    useScaleTargets = false;
                    for (int i = 0; i < NrOfAssets; i++) {
                        CompositeTransform ct = assetImages[i].RenderTransform as CompositeTransform;
                        ct.ScaleX = xyScaleTargets[i].X;
                        ct.ScaleY = xyScaleTargets[i].Y;
                        }
                    }

                updateUserDeviceTexts();

                if (cgMoving) {
                    //dbg("cgMoving {0},{1} to {2},{3}", (int)cgTransform.TranslateX, (int)cgTransform.TranslateY, (int)cgTargetX, (int)cgTargetY);
                    if (Math.Abs(cgTargetX - cgTransform.TranslateX) < 1.0 && Math.Abs(cgTargetY - cgTransform.TranslateY) < 1.0) {
                        dbg("cgMoving = false - snapped");
                        cgMoving = false;
                        cgTransform.TranslateX = cgTargetX;
                        cgTransform.TranslateY = cgTargetY;

                    } else {
                        dbg("cgTransformXY = {0},{1}", (int)cgTransform.TranslateX, (int)cgTransform.TranslateY);
                        cgTransform.TranslateX += (cgTargetX - cgTransform.TranslateX) / AnimDivider;
                        cgTransform.TranslateY += (cgTargetY - cgTransform.TranslateY) / AnimDivider;
                        centerGrid.UpdateLayout();
                    }
                }

                if (cgTransform != null)
                    cgTransform.Rotation += (cgTargetRotation - cgTransform.Rotation) / AnimDivider;
            }
        }

        private void moveCenterGridUp(object sender, object e) {
            videoClosed = true;
            double toX = 0;
            double toY = -App.Height * yShift;
            Storyboards.MoveXY(video, AppearTime, 0, 0, toX, toY, hideVideo);
            Storyboards.MoveXY(centerGrid, AppearTime, 0, 0, toX, toY, null);
            Storyboards.MoveXY(subscriptionVsOneTimeGrid, AppearTime, 0, 0, toX, toY, null);
            productsPanel.Visibility = Visibility.Visible;
            Storyboards.MoveX(productsPanel, App.ProductStackPanelWidth, 0, AppearTime, null);
            Storyboards.FadeIn(whatsIncludedBtn, AppearTime, null);
            showAssets();
        }

        private void hideVideo(object sender, object e) {
            video.Visibility = Visibility.Collapsed;
            updatePositions();
            Storyboards.FadeIn(homeButton, AppearTime, splitCenterGrids);
            Storyboards.FadeIn(infoIcon, AppearTime, null);
        }

        private void splitCenterGrids(object sender, object e) {
            centerGridAnimating = true;
            centerGridShowCompare = true;
            double toX = -App.Width * xShift;
            double toY = -App.Height * yShift;
            Storyboards.MoveXY(subscriptionVsOneTimeGrid, CenterGridSplitTime, 0, toY, -toX * xShiftCompareFactor, toY, finishCenterGridAnimation);
            if (useScalingOnCenterGridAnimation)
                Storyboards.MoveXYAndScale(centerGrid, CenterGridSplitTime, 0, toY, toX, toY, CenterGridScaleUp, CenterGridScaleUpTime, null);
            else
                Storyboards.MoveXY(centerGrid, CenterGridSplitTime, 0, toY, toX, toY, null);
            Storyboards.FadeIn(subscriptionVsOneTimeGrid, CenterGridSplitTime, null);
        }

        private void combineCenterGrids(object sender, object e) {
            centerGridAnimating = true;
            centerGridShowCompare = false;
            double toX = -App.Width * xShift;
            double toY = -App.Height * yShift;
            Storyboards.MoveXY(subscriptionVsOneTimeGrid, CenterGridSplitTime, -toX * xShiftCompareFactor, toY, 0, toY, finishCenterGridAnimation);
            if (useScalingOnCenterGridAnimation)
                Storyboards.MoveXYAndScale(centerGrid, CenterGridSplitTime, toX, toY, 0, toY, CenterGridScaleUp, CenterGridScaleUpTime, null);
            else
                Storyboards.MoveXY(centerGrid, CenterGridSplitTime, toX, toY, 0, toY, null);
            Storyboards.FadeOut(subscriptionVsOneTimeGrid, CenterGridSplitTime, null);
        }

        private void finishCenterGridAnimation(object sender, object e) {
            centerGridAnimating = false;
            useScalingOnCenterGridAnimation = true;
        }

        private void toggleCenterGrid() {
            if (centerGridShowCompare)
                combineCenterGrids(null, null);
            else
                splitCenterGrids(null, null);
        }

        private void waveAssets() {
            Storyboards.MoveAndReverseY(infoIcon, 0, -aImgSize * WaveYFactor, WaveTime * 3, null);
            Storyboards.MoveAndReverseY(whatsIncludedBtn, 0, -aImgSize * WaveYFactor, WaveTime * 3, null);
            Storyboards.MoveAndReverseY(assetsGrid, 0, -aImgSize * WaveYFactor, WaveTime * 3, null);
            for (int i = 0; i < NrOfAssets; i++) {
                if (i < 2 || ProductFeatures[currentProduct][i - 2])
                    runScaleDelayed(i * WaveShift, i);
                else
                    break;
            }
        }

        async void runScaleDelayed(int time, int id) {
            await Task.Delay(time);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                Storyboards.Scale(assetImages[id], WaveTime, 1.0, WaveScale, true, null);
            });
        }

        private void doneAnimating(object sender, object e) {
            animating = false;
        }

        private void GoHome(object sender, TappedRoutedEventArgs e) {
            if (timer != null)
            {
                timer.Tick -= Timer_Tick;
            }
            this.Frame.Navigate(HomePage);
        }

        private void fadeInNewProduct(object sender, object e) {
            productImage.Source = productImages[currentProduct].Source;
            productTitle.Text = productName[currentProduct];
            setProductHeader();
            Storyboards.FadeIn(centerGrid, FadeTime / 2, null);
        }

        private void selectProduct(int id) {
            if (animating || id == currentProduct) return;
            animating = true;
            currentProduct = id;
            Storyboards.Fade(productImages[lastProduct], FadeTime, 0.5, 1, null);
            Storyboards.FadeOut(centerGrid, FadeTime / 2, fadeInNewProduct);
            setUsersImage();
            if (currentProduct == 0) Storyboards.FadeIn(assetImages[0], AppearTime, null);

            for (int i = 2; i < NrOfAssets; i++) {
                bool hasFeature = ProductFeatures[currentProduct][i - 2];
                assetImages[i].Source = hasFeature ? colorAssetSource[i] : grayAssetSource[i];
                assetImages[i].Opacity = hasFeature ? 1.0 : GrayedOpacity;
            }

            setInfoBoxText(id);
            showAssets();
            waveAssets();

            lastProduct = currentProduct;
            Storyboards.Fade(productImages[currentProduct], FadeTime, 1, 0.5, doneAnimating);
        }

        private void setUsersImage() {
            assetImages[0].Source = currentProduct != 0 ?
                (ImageSource)Application.Current.Resources["AssetUser"] : (ImageSource)Application.Current.Resources["Asset5Users"];
            dummyUsers.HorizontalAlignment = currentProduct == 0 ? HorizontalAlignment.Left : HorizontalAlignment.Center;
        }

        private void showInfoBox(object sender, TappedRoutedEventArgs e) {
            Storyboards.FadeOut(infoIcon, AppearTime, collapseInfoIcon);
            Storyboards.FadeIn(infoBox, AppearTime, null);
            infoBox.Visibility = Visibility.Visible;
        }

        private void showProductiInfoGrid(object sender, object e) {
            Storyboards.FadeIn(learnMore365Button, AppearTime, null);
            Storyboards.FadeIn(productInfoGrid, AppearTime, null);
        }

        private void showAssetDetails(object sender, TappedRoutedEventArgs e) {
            if (featuresDetailsMode) return;
            Storyboards.FadeIn(featuresDetails, AppearTime / 2, showProductiInfoGrid);
            Storyboards.AppearBottom(featuresDetails, AppearTime / 2, null);
            learnMore365Button.Opacity = 0;
            productInfoGrid.Opacity = 0;

            featuresDetailsMode = true;
            useScaleTargets = true;
            Storyboards.FadeOut(aUsersText, FadeTime, null);
            Storyboards.FadeOut(aDeviceText, FadeTime, null);

            // TODO Rework this section
            for (int i = 0; i < NrOfAssets; i++) {
                var transform = assetDummy[i].TransformToVisual(Window.Current.Content);
                Point absolutePosition = transform.TransformPoint(new Point(0, 0));
                double ff = 1.0, xoff = 0;
                double size = App.AssetDummySize;
                double originSize = aImgSize;
                if (i == 0) { // Ignore?
                    size = App.AssetDummySizeUser;
                    xoff = size * 0.2;
                    ff = 0.85;
                } else if (i == 1) {
                    size = App.AssetDummySizeDevice * 1.2;
                    xoff = currentProduct > 1 ? 0: size * 0.1;
                    ff = 0.75;
                }
                double scale = size / originSize;
                xyScaleTargets[i].X = scale;
                xyScaleTargets[i].Y = scale;
                xyTargets[i].X = absolutePosition.X + (scale - 1.0) * size / 4 + xoff;
                xyTargets[i].Y = -App.Height + absolutePosition.Y + size * ff;
            }

            double x = xyTargets[1].X;
            if (currentProduct == 1) x *= 0.75;
            else if (currentProduct == 0) x *= 0.9;
            xyTargets[0].X = x;
            xyTargets[0].Y = xyTargets[1].Y - App.AssetDummySizeDevice / 1.7;

            loadSources();
            if (currentProduct == 0) {
                aDevice.Source = homeDevices;
                homeDevices.Stop();
                homeDevices.Play();
            } else if (currentProduct == 1) {
                aDevice.Source = personalDevices;
                personalDevices.Stop();
                personalDevices.Play();
            } else {
                aDevice.Source = singleDevice;
                singleDevice.Stop();
                singleDevice.Play();
            }
        }

        private void hideFeaturesDetails(object sender, TappedRoutedEventArgs e) {
            Storyboards.DissapearBottom(featuresDetails, AppearTime, collapseFeaturesDetails);
            useScaleTargets = true;
            featuresDetailsMode = false;
            Storyboards.FadeIn(aUsersText, FadeTime, null);
            Storyboards.FadeIn(aDeviceText, FadeTime, null);
            aDevice.Source = singleDevice;
            showAssets();
        }

        private void showComparisonGrid(object sender, TappedRoutedEventArgs e) {
            /*if (UseComparisonAnimation) {
                for (int i = 0; i < NrOfProducts; i++)
                    comparisonGrids[i].Opacity = 0.0;
                Storyboards.AppearRight(comparisonGrid, animateComparisonChart);
            } else {*/
                animateComparisonChart(null, null);
                Storyboards.AppearRight(comparisonGrid, null);
            //}
        }

        private void animateComparisonChart(object sender, object e) {
            Grid grid = (VisualTreeHelper.GetParent(comparisonBorderGrid) as Grid);

            grid.Children.Remove(comparisonBorderGrid);
            //(grid.Children[0] as Image).Opacity = 1.0;
            //grid.Background = null;
            //(comparisonGrids[id].Children[0] as Image).Opacity = 0.0;
            //ImageBrush back = new ImageBrush();
            //back.ImageSource = comparisonImages[id].Source;
            //back.Stretch = Stretch.Fill;
            //comparisonGrids[id].Background = back;
            comparisonGrids[currentProduct].Children.Add(comparisonBorderGrid);

            /*if (UseComparisonAnimation)
                for (int i = 0; i < NrOfProducts; i++)
                    runCompareAnimationDelayed(i * WaveShift * 2, i);*/
        }

        async void runCompareAnimationDelayed(int time, int id) {
            await Task.Delay(time);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                //comparisonImages[id].Opacity = 1.0;
                double shift = -comparisonGrids[id].ActualHeight * 2;
                Storyboards.MoveXYAndFadeIn(comparisonGrids[id], AppearTime, shift, shift, 0, 0, null);
            });
        }

        private void hideAssetDetails(object sender, TappedRoutedEventArgs e) {
            Storyboards.DissapearBottom(assetDetails, AppearTime, collapseAssetDetails);
        }

        private void collapseAssetDetails(object sender, object e) {
            assetDetails.Visibility = Visibility.Collapsed;
        }

        private void infoBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void hideComparisonGrid(object sender, TappedRoutedEventArgs e) {
            switch (compareMode) {
                case CompareMode.Full:
                    Storyboards.DissapearRight(comparisonGrid, collapseComparisonGrid);
                    break;
                case CompareMode.Compare1:
                    compareTransition = true;
                    compareMode = CompareMode.Full;
                    Storyboards.MoveToXAndFadeOut(compare1left, AppearTime, -App.Width / 2, fadeInCompareChart);
                    Storyboards.MoveToXAndFadeOut(compare1right, AppearTime, App.Width / 2, null);
                    break;
                case CompareMode.Compare2:
                    compareTransition = true;
                    compareMode = CompareMode.Full;
                    Storyboards.MoveToXAndFadeOut(compare2left1, AppearTime, -App.Width / 2, fadeInCompareChart);
                    Storyboards.MoveToXAndFadeOut(compare2rleft2, AppearTime, -App.Width / 2, null);
                    Storyboards.MoveToXAndFadeOut(compare2right1, AppearTime, App.Width / 2, null);
                    Storyboards.MoveToXAndFadeOut(compare2right2, AppearTime, App.Width / 2, null);
                    Storyboards.MoveToXAndFadeOut(compare2right3, AppearTime, App.Width / 2, null);
                    Storyboards.MoveToXAndFadeOut(compare2right4, AppearTime, App.Width / 2, null);
                    break;
            }
        }

        private void fadeInCompareChart(object sender, object e) {
            comparisonGridHP.Visibility = Visibility.Collapsed;
            comparisonGrid365_2016.Visibility = Visibility.Collapsed;
            Storyboards.FadeIn(comparisonChart, AppearTime, finishCompareTransition);
        }

        bool compareTransition = false;

        private void compareHomeAndPersonal(object sender, TappedRoutedEventArgs e) {
            if (compareTransition) return;
            compareTransition = true;
            compareMode = CompareMode.Compare1;
            Storyboards.FadeOut(comparisonChart, FadeTime, showCompare1);
        }

        private void showCompare1(object sender, object e) {
            comparisonChart.Visibility = Visibility.Collapsed;
            comparisonGridHP.Visibility = Visibility.Visible;
            Storyboards.MoveFromXAndFadeIn(compare1left, AppearTime, -App.Width / 2, finishCompareTransition);
            Storyboards.MoveFromXAndFadeIn(compare1right, AppearTime, App.Width / 2, null);
        }

        private void finishCompareTransition(object sender, object e) {
            compareTransition = false;
        }

        private void compare365And2016(object sender, TappedRoutedEventArgs e) {
            if (compareTransition) return;
            compareTransition = true;
            compareMode = CompareMode.Compare2;
            Storyboards.FadeOut(comparisonChart, FadeTime, showCompare2);
        }

        private void showCompare2(object sender, object e) {
            comparisonChart.Visibility = Visibility.Collapsed;
            comparisonGrid365_2016.Visibility = Visibility.Visible;
            Storyboards.MoveFromXAndFadeIn(compare2left1, AppearTime, -App.Width / 2, finishCompareTransition);
            Storyboards.MoveFromXAndFadeIn(compare2rleft2, AppearTime, -App.Width / 2, null);
            Storyboards.MoveFromXAndFadeIn(compare2right1, AppearTime, App.Width / 2, null);
            Storyboards.MoveFromXAndFadeIn(compare2right2, AppearTime, App.Width / 2, null);
            Storyboards.MoveFromXAndFadeIn(compare2right3, AppearTime, App.Width / 2, null);
            Storyboards.MoveFromXAndFadeIn(compare2right4, AppearTime, App.Width / 2, null);
        }

        private void collapseFeaturesDetails(object sender, object e) {
            featuresDetails.Visibility = Visibility.Collapsed;
        }

        private void collapseInfoIcon(object sender, object e) {
            infoIcon.Visibility = Visibility.Collapsed;
        }

        private void showNewFeatures(object sender, TappedRoutedEventArgs e) {
            if (timer != null)
            {
                timer.Tick -= Timer_Tick;
            }
            this.Frame.Navigate(typeof(WhatsNew));
        }

        private void collapseInfoBox(object sender, object e) {
            infoBox.Visibility = Visibility.Collapsed;
        }

        private void collapseComparisonGrid(object sender, object e) {
            comparisonGrid.Visibility = Visibility.Collapsed;
        }

        private void hideInfoBox(object sender, TappedRoutedEventArgs e) {
            Storyboards.FadeOut(infoBox, AppearTime, collapseInfoBox);
            Storyboards.FadeIn(infoIcon, AppearTime, null);
            infoIcon.Visibility = Visibility.Visible;
        }

        private void pImg_Tapped(object sender, TappedRoutedEventArgs e) {
            int tag = int.Parse((sender as Image).Tag.ToString());
            if (tag == 0 || tag == 1)
            {
                App.changeProductInfoBoxHeight(false);
            } else
            {
                App.changeProductInfoBoxHeight(true);
            }
            selectProduct(tag);
        }


        private void updateUserDeviceTexts() {
            double yFactor = 0.9;

            CompositeTransform act = new CompositeTransform();
            aUsersText.Text = currentProduct == 0 ? "5 Users" : "1 User";
            act.TranslateX = (assetImages[0].RenderTransform as CompositeTransform).TranslateX + assetImages[0].ActualWidth / 2 - aUsersText.ActualWidth / 2;
            act.TranslateY = (assetImages[0].RenderTransform as CompositeTransform).TranslateY + aImgSize * yFactor;
            aUsersText.RenderTransform = act;

            string nr = currentProduct == 0 ? "5" : "1";
            string dev = "PC";
            if (ProductFeatures[currentProduct][12] && ProductFeatures[currentProduct][13])
                dev = "PC/Mac";
            else if (ProductFeatures[currentProduct][12])
                dev = "Mac";
            aDeviceText.Text = nr + " " + dev;
            act = new CompositeTransform();
            act.TranslateX = (assetImages[1].RenderTransform as CompositeTransform).TranslateX + aImgSize / 2 - aDeviceText.ActualWidth / 2;
            act.TranslateY = (assetImages[1].RenderTransform as CompositeTransform).TranslateY + aImgSize * yFactor;
            aDeviceText.RenderTransform = act;
        }

        private void initAssets() {
            for (int i = 0; i < NrOfAssets; i++) {
                assetImages[i].Height = aImgSize;
                double x = aImgXOffset + i * aImgSpan;
                double y = aImgSpan * 2;
                CompositeTransform ct = assetImages[i].RenderTransform as CompositeTransform;
                ct.TranslateX = x;
                ct.TranslateY = y;
                xyTargets[i] = new Point(x, y);
            }
        }

        private void showAssets() {
            for (int i = 0; i < NrOfAssets; i++) {
                double yoff = 0;
                bool multi = i == 0 && currentProduct == 0;
                if (i < 2) {
                    if (multi) {
                        assetImages[i].Height = aImgSize * 0.65;
                        yoff = 0.35 * aImgSize;
                    } else {
                        assetImages[i].Height = aImgSize;
                        yoff = 0.1 * aImgSize;
                    }
                } else
                    assetImages[i].Height = aImgSize;
                double x = aImgXOffset + i * aImgSpan;
                double y = aImgYOffset - yoff;
                if (multi) x *= 0.2;
                xyTargets[i] = new Point(x, y);
                xyScaleTargets[i] = new Point(1, 1);
            }

            updateUserDeviceTexts();
        }

        public void updatePositions() {
            aImgSize = App.Width / 20;
            aImgSpan = aImgSize * 1.11;
            aImgYOffset = -aImgSize * 0.2;
            aImgXOffset = aImgSpan * 2;
            infoIcon.Margin = new Thickness(App.Scale * 50, 0, 0, aImgSpan + App.Scale * 50);
            App.InfoBoxFontSize *= 1.085;
            App.InfoBoxTitleFontSize *= 1.08;
            App.InfoBoxPadding = new Thickness(App.P30, App.P50, App.P30, App.P35);
            App.InfoBoxCloseIconMargin = new Thickness(0, -App.P35, -App.P15, 0);
            showAssets();
            centerVideo();
        }
    }
}
