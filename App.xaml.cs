using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.ComponentModel;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;
using Windows.System.UserProfile;
using Windows.Graphics.Display;

namespace OPS
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application, INotifyPropertyChanged {

        //ApplicationLanguages.PrimaryLanguageOverride = GlobalizationPreferences.Language[0];
        //ApplicationLanguages.PrimaryLanguageOverride = GlobalizationPreferences.Languages[0];

        Type StartPage = typeof(HomePage);

        public const double InnerCircleScale = 0.88;
        public const double ProductCircleScale = 0.655;

        public static App app;
        public static Questions questionsPage;
        public static ProductPage productsPage;
        public static WhatsNew whatsNewPage;
        public static double Width, Height, Scale, Aspect, MinSide, MaxSide;        
        public static int SelectedProduct = -1; // Product not selected

        public string currentLangMode;

        public Frame rootFrame;
        double baseWidth, baseHeight, baseLargeFontSize, baseQuestionsFontSize, baseInfoBoxFontSize, baseInfoBoxTitleFontSize, baseOrangeButtonFontSize,
            baseWhatsNewSubtitleFontSize, baseComparisonFontSize, baseWhatsNewFontSize;
        double fontScale = 1.0;

        public event PropertyChangedEventHandler PropertyChanged;
        public static ResourceLoader ResourceLoader = new ResourceLoader();

        private void NotifyPropertyChanged(String propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Data bindings
        // Fonts
        double comparisonFontSize = 21;
        public double ComparisonFontSize { get { return comparisonFontSize; } set { comparisonFontSize = value; NotifyPropertyChanged("ComparisonFontSize"); } }
        double infoBoxFontSize = 21;
        public double InfoBoxFontSize { get { return infoBoxFontSize; } set { infoBoxFontSize = value; NotifyPropertyChanged("InfoBoxFontSize"); } }
        double infoBoxTitleFontSize = 21;
        public double InfoBoxTitleFontSize { get { return infoBoxTitleFontSize; } set { infoBoxTitleFontSize = value; NotifyPropertyChanged("InfoBoxTitleFontSize"); } }
        double answerFontSize = 38;
        public double AnswerFontSize { get { return answerFontSize; } set { answerFontSize = value; NotifyPropertyChanged("AnswerFontSize"); } }
        double questionsFontSize = 38;
        public double QuestionsFontSize { get { return questionsFontSize; } set { questionsFontSize = value; NotifyPropertyChanged("QuestionsFontSize"); } }
        double largeFontSize = 42;
        public double LargeFontSize { get { return largeFontSize; } set { largeFontSize = value; NotifyPropertyChanged("LargeFontSize"); } }
        double backFontSize = 22;
        public double BackFontSize { get { return backFontSize; } set { backFontSize = value; NotifyPropertyChanged("BackFontSize"); } }

        // Orange button
        double orangeButtonFontSize = 20;
        public double OrangeButtonFontSize { get { return orangeButtonFontSize; } set { orangeButtonFontSize = value; NotifyPropertyChanged("OrangeButtonFontSize"); } }
        Thickness orangeButtonBorder = new Thickness(5);
        public Thickness OrangeButtonBorder { get { return orangeButtonBorder; } set { orangeButtonBorder = value; NotifyPropertyChanged("OrangeButtonBorder"); } }
        Thickness orangeButtonMargin = new Thickness(10);
        public Thickness OrangeButtonMargin { get { return orangeButtonMargin; } set { orangeButtonMargin = value; NotifyPropertyChanged("OrangeButtonMargin"); } }
        Thickness orangeButtonPadding = new Thickness(10);
        public Thickness OrangeButtonPadding { get { return orangeButtonPadding; } set { orangeButtonPadding = value; NotifyPropertyChanged("OrangeButtonPadding"); } }
        CornerRadius orangeButtonCorners = new CornerRadius(15);
        public CornerRadius OrangeButtonCorners { get { return orangeButtonCorners; } set { orangeButtonCorners = value; NotifyPropertyChanged("OrangeButtonCorners"); } }

        // Features
        double featureUserDeviceFontSize = 20;
        public double FeatureUserDeviceFontSize { get { return featureUserDeviceFontSize; } set { featureUserDeviceFontSize = value; NotifyPropertyChanged("FeatureUserDeviceFontSize"); } }
        Thickness assetsSectionMargin = new Thickness(20);
        public Thickness AssetsSectionMargin { get { return assetsSectionMargin; } set { assetsSectionMargin = value; NotifyPropertyChanged("AssetsSectionMargin"); } }
        double assetDetailsFontSize = 20;
        public double AssetDetailsFontSize { get { return assetDetailsFontSize; } set { assetDetailsFontSize = value; NotifyPropertyChanged("AssetDetailsFontSize"); } }
        double assetDummySize = 20;
        public double AssetDummySize { get { return assetDummySize; } set { assetDummySize = value; NotifyPropertyChanged("AssetDummySize"); } }
        double assetDummySizeUser = 20;
        public double AssetDummySizeUser { get { return assetDummySizeUser; } set { assetDummySizeUser = value; NotifyPropertyChanged("AssetDummySizeUser"); } }
        double assetDummySizeDevice = 20;
        public double AssetDummySizeDevice { get { return assetDummySizeDevice; } set { assetDummySizeDevice = value; NotifyPropertyChanged("AssetDummySizeDevice"); } }
        Thickness featuresGridMargin = new Thickness(20);
        public Thickness FeaturesGridMargin { get { return featuresGridMargin; } set { featuresGridMargin = value; NotifyPropertyChanged("FeaturesGridMargin"); } }
        Thickness featuresBackButtonMargin = new Thickness(20);
        public Thickness FeaturesBackButtonMargin { get { return featuresBackButtonMargin; } set { featuresBackButtonMargin = value; NotifyPropertyChanged("FeaturesBackButtonMargin"); } }
        Thickness assetDummyMargin = new Thickness(20);
        public Thickness AssetDummyMargin { get { return assetDummyMargin; } set { assetDummyMargin = value; NotifyPropertyChanged("AssetDummyMargin"); } }
        Thickness assetDummyMargin2 = new Thickness(20);
        public Thickness AssetDummyMargin2 { get { return assetDummyMargin2; } set { assetDummyMargin2 = value; NotifyPropertyChanged("AssetDummyMargin2"); } }


        // Product
        double productCompareTitleFontSize = 20;
        public double ProductCompareTitleFontSize { get { return productCompareTitleFontSize; } set { productCompareTitleFontSize = value; NotifyPropertyChanged("ProductCompareTitleFontSize"); } }

        double productCompareDetailFontSize = 20;
        public double ProductCompareDetailFontSize { get { return productCompareDetailFontSize; } set { productCompareDetailFontSize = value; NotifyPropertyChanged("ProductCompareDetailFontSize"); } }

        double productFontSize = 20;
        public double ProductFontSize { get { return productFontSize; } set { productFontSize = value; NotifyPropertyChanged("ProductFontSize"); } }
        double productNameFontSize = 20;
        public double ProductNameFontSize { get { return productNameFontSize; } set { productNameFontSize = value; NotifyPropertyChanged("ProductNameFontSize"); } }
        double productOverNameFontSize = 20;
        public double ProductOverNameFontSize { get { return productOverNameFontSize; } set { productOverNameFontSize = value; NotifyPropertyChanged("ProductOverNameFontSize"); } }
        double productCircleSize = 300;
        public double ProductCircleSize { get { return productCircleSize; } set { productCircleSize = value; NotifyPropertyChanged("ProductCircleSize"); } }
        double productStackPanelWidth = 100;
        public double ProductStackPanelWidth { get { return productStackPanelWidth; } set { productStackPanelWidth = value; NotifyPropertyChanged("ProductStackPanelWidth"); } }
        double productImageSize = 100;
        public double ProductImageSize { get { return productImageSize; } set { productImageSize = value; NotifyPropertyChanged("ProductImageSize"); } }
        double productLargeImageSize = 200;
        public double ProductLargeImageSize { get { return productLargeImageSize; } set { productLargeImageSize = value; NotifyPropertyChanged("ProductLargeImageSize"); } }
        Thickness productStackPanelMargin = new Thickness(20);
        public Thickness ProductStackPanelMargin { get { return productStackPanelMargin; } set { productStackPanelMargin = value; NotifyPropertyChanged("ProductStackPanelMargin"); } }
        Thickness productInfoIconMargin = new Thickness(20);
        public Thickness ProductInfoIconMargin { get { return productInfoIconMargin; } set { productInfoIconMargin = value; NotifyPropertyChanged("ProductInfoIconMargin"); } }
        Thickness productWhatsIncludedMargin = new Thickness(20);
        public Thickness ProductWhatsIncludedMargin { get { return productWhatsIncludedMargin; } set { productWhatsIncludedMargin = value; NotifyPropertyChanged("ProductWhatsIncludedMargin"); } }
        double productInfoBoxWidth = 20;
        public double ProductInfoBoxWidth { get { return productInfoBoxWidth; } set { productInfoBoxWidth = value; NotifyPropertyChanged("ProductInfoBoxWidth"); } }
        double productInfoBoxHeight = 20;
        public double ProductInfoBoxHeight { get { return productInfoBoxHeight; } set { productInfoBoxHeight = value; NotifyPropertyChanged("ProductInfoBoxHeight"); } }
        Thickness productStackPanelButtonsMargin = new Thickness(20);
        public Thickness ProductStackPanelButtonsMargin { get { return productStackPanelButtonsMargin; } set { productStackPanelButtonsMargin = value; NotifyPropertyChanged("ProductStackPanelButtonsMargin"); } }
        bool isProductInfoBoxSizeChange = false;
        
        // Questions
        double questionsInfoBoxWidth = 300;
        public double QuestionsInfoBoxWidth { get { return questionsInfoBoxWidth; } set { questionsInfoBoxWidth = value; NotifyPropertyChanged("QuestionsInfoBoxWidth"); } }
        double questionsCircleDiameter = 300;
        public double QuestionsCircleDiameter { get { return questionsCircleDiameter; } set { questionsCircleDiameter = value; NotifyPropertyChanged("QuestionsCircleDiameter"); } }
        double questionsOuterCircleDiameter = 300;
        public double QuestionsOuterCircleDiameter { get { return questionsOuterCircleDiameter; } set { questionsOuterCircleDiameter = value; NotifyPropertyChanged("QuestionsOuterCircleDiameter"); } }
        double questionsDotDiameter = 60;
        public double QuestionsDotDiameter { get { return questionsDotDiameter; } set { questionsDotDiameter = value; NotifyPropertyChanged("QuestionsDotDiameter"); } }
        Thickness questionsPadding = new Thickness(50, 50, 50, 50);
        public Thickness QuestionsPadding { get { return questionsPadding; } set { questionsPadding = value; NotifyPropertyChanged("QuestionsPadding"); } }

        // Whats new
        Thickness whatsNewPadding = new Thickness(50);
        public Thickness WhatsNewPadding { get { return whatsNewPadding; } set { whatsNewPadding = value; NotifyPropertyChanged("WhatsNewPadding"); } }
        double whatsNewFontSize = 42;
        public double WhatsNewFontSize { get { return whatsNewFontSize; } set { whatsNewFontSize = value; NotifyPropertyChanged("WhatsNewFontSize"); } }
        double whatsNewSubtitleFontSize = 42;
        public double WhatsNewSubtitleFontSize { get { return whatsNewSubtitleFontSize; } set { whatsNewSubtitleFontSize = value; NotifyPropertyChanged("WhatsNewSubtitleFontSize"); } }
        double whatsNewFingerSize = 50;
        public double WhatsNewFingerSize { get { return whatsNewFingerSize; } set { whatsNewFingerSize = value; NotifyPropertyChanged("WhatsNewFingerSize"); } }
        Thickness whatsNewFingerMargin = new Thickness(0);
        public Thickness WhatsNewFingerMargin { get { return whatsNewFingerMargin; } set { whatsNewFingerMargin = value; NotifyPropertyChanged("WhatsNewFingerMargin"); } }

        // Home
        double homeOfficeSize = 100;
        public double HomeOfficeSize { get { return homeOfficeSize; } set { homeOfficeSize = value; NotifyPropertyChanged("HomeOfficeSize"); } }
        double homeArrowSize = 100;
        public double HomeArrowSize { get { return homeArrowSize; } set { homeArrowSize = value; NotifyPropertyChanged("HomeArrowSize"); } }
        double homeArrowSize2x = 200;
        public double HomeArrowSize2x { get { return homeArrowSize2x; } set { homeArrowSize2x = value; NotifyPropertyChanged("HomeArrowSize2x"); } }
        Thickness homeOfficeMargin = new Thickness(-20, 0, 0, -20);
        public Thickness HomeOfficeMargin { get { return homeOfficeMargin; } set { homeOfficeMargin = value; NotifyPropertyChanged("HomeOfficeMargin"); } }
        double backIconSize = 44;
        public double BackIconSize { get { return backIconSize; } set { backIconSize = value; NotifyPropertyChanged("BackIconSize"); } }
        Thickness homePadding = new Thickness(50, 100, 50, 50);
        public Thickness HomePadding { get { return homePadding; } set { homePadding = value; NotifyPropertyChanged("HomePadding"); } }
        Thickness homeArrowSizeMargin = new Thickness(0, 100, 0, 0);
        public Thickness HomeArrowSizeMargin { get { return homeArrowSizeMargin; } set { homeArrowSizeMargin = value; NotifyPropertyChanged("HomeArrowSizeMargin"); } }

        // Info box
        CornerRadius infoBoxCornerRadius = new CornerRadius(15);
        public CornerRadius InfoBoxCornerRadius { get { return infoBoxCornerRadius; } set { infoBoxCornerRadius = value; NotifyPropertyChanged("InfoBoxCornerRadius"); } }
        double infoIconSize = 77;
        public double InfoIconSize { get { return infoIconSize; } set { infoIconSize = value; NotifyPropertyChanged("InfoIconSize"); } }
        double infoBoxCloseIconSize = 20;
        public double InfoBoxCloseIconSize { get { return infoBoxCloseIconSize; } set { infoBoxCloseIconSize = value; NotifyPropertyChanged("InfoBoxCloseIconSize"); } }
        Thickness infoIconMargin = new Thickness(30);
        public Thickness InfoIconMargin { get { return infoIconMargin; } set { infoIconMargin = value; NotifyPropertyChanged("InfoIconMargin"); } }
        Thickness infoBoxPadding = new Thickness(30);
        public Thickness InfoBoxPadding { get { return infoBoxPadding; } set { infoBoxPadding = value; NotifyPropertyChanged("InfoBoxPadding"); } }
        Thickness infoBoxCloseIconMargin = new Thickness(30);
        public Thickness InfoBoxCloseIconMargin { get { return infoBoxCloseIconMargin; } set { infoBoxCloseIconMargin = value; NotifyPropertyChanged("InfoBoxCloseIconMargin"); } }
        
        // Questions intor
        Thickness introPadding = new Thickness(0, 0, 100, 100);
        public Thickness IntroPadding { get { return introPadding; } set { introPadding = value; NotifyPropertyChanged("IntroPadding"); } }

        // Back button
        Thickness backButtonMargin = new Thickness(0);
        public Thickness BackButtonMargin { get { return backButtonMargin; } set { backButtonMargin = value; NotifyPropertyChanged("BackButtonMargin"); } }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {   
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            app = this;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            // Default window size
            baseWidth = (double)Current.Resources["BaseWindowWidth"];
            baseHeight = (double)Current.Resources["BaseWindowHeight"];
            baseLargeFontSize = (double)Current.Resources["BaseLargeFontSize"];
            baseQuestionsFontSize = (double)Current.Resources["BaseQuestionsFontSize"];
            baseInfoBoxFontSize = (double)Current.Resources["BaseInfoBoxFontSize"];
            baseInfoBoxTitleFontSize = (double)Current.Resources["BaseInfoBoxTitleFontSize"];
            baseOrangeButtonFontSize = (double)Current.Resources["BaseOrangeButtonFontSize"];
            baseWhatsNewSubtitleFontSize = (double)Current.Resources["BaseWhatsNewSubtitleFontSize"];
            baseComparisonFontSize = (double)Current.Resources["BaseComparisonFontSize"];
            baseWhatsNewFontSize = (double)Current.Resources["BaseWhatsNewFontSize"];
            currentLangMode = currentLanguageMode();

            double launchWidth = (double)Current.Resources["WindowWidth"];
            double launchHeight = (double)Current.Resources["WindowHeight"];
            
            ApplicationView.PreferredLaunchViewSize = new Size(launchWidth, launchHeight);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            ApplicationLanguages.PrimaryLanguageOverride = GlobalizationPreferences.Languages[0];

            if ((bool)Current.Resources["FullScreen"]) ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            // Resize handler
            rootFrame = Window.Current.Content as Frame;

            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            fontScale = size.Width * 9 / (size.Height * 16);

            Debug.WriteLine("Window Width : " + Window.Current.CoreWindow.Bounds.Width + ",  Window Height :" + Window.Current.CoreWindow.Bounds.Height);

            //windowResize(Window.Current.CoreWindow.Bounds.Width, Window.Current.CoreWindow.Bounds.Height);

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.ContentTransitions = new TransitionCollection();
                rootFrame.ContentTransitions.Add(new NavigationThemeTransition());
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                Window.Current.CoreWindow.SizeChanged += (sender, args) => { windowResize(); };
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(StartPage, e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }
        public string currentLanguageMode()
        {
            String currentCulture = Windows.System.UserProfile.GlobalizationPreferences.Languages[0].ToString();
            

            if (currentCulture == "cs" || currentCulture == "fr-CA" || currentCulture == "fr-FR")
            {
                return currentCulture;
            }
            else
            {
                return "en-US";
            }
        }
        public void changeProductInfoBoxHeight(bool flag)
        {

            if (currentLangMode == "cs")
            {
                if (fontScale < 1.0)
                {
                    if (flag == true)
                    {
                        ProductInfoBoxHeight = Height * 0.28;
                    }
                    else
                    {
                        ProductInfoBoxHeight = Height * 0.34;
                    }
                } else
                {
                    if (flag == true)
                    {
                        ProductInfoBoxHeight = Height * 0.34;
                    }
                    else
                    {
                        ProductInfoBoxHeight = Height * 0.42;
                    }
                }
               
            } else if (currentLangMode == "fr-CA")
            {
                if (fontScale < 1.0)
                {
                    if (flag == true)
                    {
                        ProductInfoBoxHeight = Height * 0.28;
                    }
                    else
                    {
                        ProductInfoBoxHeight = Height * 0.34;
                    }
                }
                else
                {
                    if (flag == true)
                    {
                        ProductInfoBoxHeight = Height * 0.32;
                    }
                    else
                    {
                        ProductInfoBoxHeight = Height * 0.40;
                    }
                }
                
            } else if (currentLangMode == "fr-FR")
            {
                if (fontScale < 1.0)
                {
                    if (flag == true)
                    {
                        ProductInfoBoxHeight = Height * 0.28;
                    }
                    else
                    {
                        ProductInfoBoxHeight = Height * 0.34;
                    }
                }
                else
                {
                    if (flag == true)
                    {
                        ProductInfoBoxHeight = Height * 0.33;
                    }
                    else
                    {
                        ProductInfoBoxHeight = Height * 0.44;
                    }
                }
                
            } else
            {
                if (fontScale < 1.0)
                {
                    if (flag == true)
                    {
                        ProductInfoBoxHeight = Height * 0.34;
                    }
                    else
                    {
                        ProductInfoBoxHeight = Height * 0.38;
                    }
                }
                else
                {
                    if (flag == true)
                    {
                        ProductInfoBoxHeight = Height * 0.36;
                    }
                    else
                    {
                        ProductInfoBoxHeight = Height * 0.44;
                    }
                }
                
            }

            /*bool isEnUS = currentLanguageMode();
            if (flag == true)
            {
                if (isEnUS)
                {
                    ProductInfoBoxHeight = Height * 0.36;
                } else
                {
                    ProductInfoBoxHeight = Height * 0.42;
                }
                
            }
            else
            {
                if (isEnUS)
                {
                    ProductInfoBoxHeight = Height * 0.44;
                }
                else
                {
                    ProductInfoBoxHeight = Height * 0.46;
                }
                
            }*/
        }

        public void windowResize() {
            //windowResize(((Frame)Window.Current.Content).ActualWidth, ((Frame)Window.Current.Content).ActualHeight);
            windowResize(Window.Current.CoreWindow.Bounds.Width, Window.Current.CoreWindow.Bounds.Height);
        }


        double p5, p10, p15, p20, p25, p30, p35, p50, p100;
        public double P5 { get { return p5; } set { p5 = value; NotifyPropertyChanged("P5"); } }
        public double P10 { get { return p10; } set { p10 = value; NotifyPropertyChanged("P10"); } }
        public double P15 { get { return p15; } set { p15 = value; NotifyPropertyChanged("P15"); } }
        public double P20 { get { return p20; } set { p20 = value; NotifyPropertyChanged("P20"); } }
        public double P25 { get { return p25; } set { p25 = value; NotifyPropertyChanged("P25"); } }
        public double P30 { get { return p30; } set { p30 = value; NotifyPropertyChanged("P30"); } }
        public double P35 { get { return p35; } set { p35 = value; NotifyPropertyChanged("P35"); } }
        public double P50 { get { return p50; } set { p50 = value; NotifyPropertyChanged("P50"); } }
        public double P100 { get { return p100; } set { p100 = value; NotifyPropertyChanged("P100"); } }

        private void windowResize(double width, double height) {
            if (width == 0 || height == 0) return;
            Width = width;
            Height = height;
            MinSide = Math.Min(width, height);
            MaxSide = Math.Max(width, height);
            Scale = MinSide / baseHeight;
            Aspect = Width / Height;            

            LargeFontSize = baseLargeFontSize * Scale;
            ComparisonFontSize = baseComparisonFontSize * Scale;

            P5 = 5 * Scale;
            P10 = 10 * Scale;
            P15 = 15 * Scale;
            P20 = 20 * Scale;
            P25 = 25 * Scale;
            P30 = 30 * Scale;
            P35 = 35 * Scale;
            P50 = 50 * Scale;
            P100 = 100 * Scale;

            HomeArrowSize = P100 * 0.8;
            HomeArrowSize2x = HomeArrowSize * 2;
            HomeArrowSizeMargin = new Thickness(0, P100, 0, 0);
            HomePadding = new Thickness(P35, 80 * Scale, P35, P50);
            HomeOfficeMargin = new Thickness(0, 0, 0, 0);
            HomeOfficeSize = P50;

            IntroPadding = new Thickness(P100 * 0.7, P100 * 0.7, P100 * 0.8, P50);

            BackFontSize = MinSide / 40;
            BackIconSize = 16 * Scale;

            BackButtonMargin = new Thickness(P50);

            QuestionsPadding = new Thickness(width * 0.0525, height * 0.075, width * 0.0525, height * 0.075);
            QuestionsFontSize = baseQuestionsFontSize * Scale;
            AnswerFontSize = baseQuestionsFontSize * Scale;
            QuestionsDotDiameter = 70 * Scale;
            QuestionsInfoBoxWidth = width * 0.41;
            QuestionsOuterCircleDiameter = MinSide * ProductCircleScale;
            QuestionsCircleDiameter = QuestionsOuterCircleDiameter * InnerCircleScale;

            InfoIconSize = 45 * Scale;
            InfoIconMargin = new Thickness(1.2 * P15, 0, 0, 0.84 * P15);
            InfoBoxFontSize = baseInfoBoxFontSize * Scale;
            InfoBoxTitleFontSize = baseInfoBoxTitleFontSize * Scale;
            InfoBoxPadding = new Thickness(P30, P25, P30, P35);
            InfoBoxCornerRadius = new CornerRadius(P10);
            InfoBoxCloseIconMargin = new Thickness(0, -P10, -P15, 0);
            InfoBoxCloseIconSize = P15;

            OrangeButtonFontSize = baseOrangeButtonFontSize * Scale;
            OrangeButtonBorder = new Thickness(2.6 * Scale);
            OrangeButtonPadding = new Thickness(P10, P10 * 0.8, P10, P10 * 0.75);
            OrangeButtonCorners = new CornerRadius(P5);
            OrangeButtonMargin = new Thickness(0, P5, 0, P5);

            WhatsNewPadding = new Thickness(P50, P50, P15, P50);
            WhatsNewSubtitleFontSize = baseWhatsNewSubtitleFontSize * Scale;
            WhatsNewFontSize = baseWhatsNewFontSize * Scale;
            WhatsNewFingerSize = P50;
            WhatsNewFingerMargin = new Thickness(0, 0, P15, 0);

            ProductCircleSize = QuestionsCircleDiameter;
            ProductStackPanelWidth = height * 0.32;
            ProductImageSize = ProductStackPanelWidth / 2;
            ProductLargeImageSize = ProductCircleSize / 2.3;
            ProductFontSize = ProductStackPanelWidth / 10;            
            
            ProductStackPanelMargin = new Thickness(0, P20, P15, 0);
            ProductInfoIconMargin = new Thickness(50 * Scale, 0, 0, 150 * Scale);
            ProductWhatsIncludedMargin = new Thickness(P5, 0, 0, 77 * Scale);
            ProductInfoBoxWidth = width * 0.7;
            
                        
            ProductStackPanelButtonsMargin = new Thickness(ProductStackPanelWidth * 0.02, P10, ProductStackPanelWidth * 0.02, -P15 * 0.9);

            AssetDummySize = MinSide * 0.12;
            AssetDummySizeUser = AssetDummySize * 0.7;
            AssetDummySizeDevice = AssetDummySize * 1.7 * fontScale;
            AssetDummyMargin = new Thickness(P25, P5, 0, P5);
            AssetDummyMargin2 = new Thickness(P25, P10, 0, P10);
            AssetDetailsFontSize = ComparisonFontSize * 0.9 * fontScale;
            AssetsSectionMargin = new Thickness(0, P10, 0, 0);

            FeaturesGridMargin = new Thickness(P20, P20 * 2, P20, P20);
            FeaturesBackButtonMargin = new Thickness(P10);
            FeatureUserDeviceFontSize = ComparisonFontSize * 0.5;

            if (currentLangMode == "cs")
            {
                //QuestionsFontSize = QuestionsFontSize * 0.9;
                if (fontScale < 1.0)
                {
                    WhatsNewFontSize = WhatsNewFontSize * 0.8 * fontScale;
                    AnswerFontSize = AnswerFontSize * 0.76 * fontScale;
                    InfoBoxFontSize = InfoBoxFontSize * 0.78 * fontScale;
                    InfoBoxTitleFontSize = InfoBoxTitleFontSize * 0.70 * fontScale;
                    ProductCompareDetailFontSize = ComparisonFontSize * 0.70 * fontScale;
                    ProductCompareTitleFontSize = ProductFontSize * 0.70 * fontScale;
                    ProductNameFontSize = ProductCircleSize / 20 * fontScale;
                    ProductOverNameFontSize = ProductNameFontSize * 0.7 * fontScale;
                    ProductInfoBoxHeight = height * 0.34;
                } else
                {
                    WhatsNewFontSize = WhatsNewFontSize * 0.8 * fontScale;
                    AnswerFontSize = AnswerFontSize * 0.76 * fontScale;
                    InfoBoxFontSize = InfoBoxFontSize * 0.78 * fontScale;
                    InfoBoxTitleFontSize = InfoBoxTitleFontSize * 0.86 * fontScale;
                    ProductCompareDetailFontSize = ComparisonFontSize * 0.78 * fontScale;
                    ProductCompareTitleFontSize = ProductFontSize * 0.74 * fontScale;
                    ProductNameFontSize = ProductCircleSize / 20 * fontScale;
                    ProductOverNameFontSize = ProductNameFontSize * 0.7 * fontScale;
                    ProductInfoBoxHeight = height * 0.42;
                    AssetDetailsFontSize = AssetDetailsFontSize * 0.8;
                }
                
                //ProductInfoBoxWidth = ProductInfoBoxWidth * 1.08;
            }
            else if (currentLangMode == "fr-CA")
            {
                if (fontScale < 1.0)
                {
                    WhatsNewFontSize = WhatsNewFontSize * 0.8 * fontScale;
                    InfoBoxFontSize = InfoBoxFontSize * 0.78 * fontScale;
                    InfoBoxTitleFontSize = InfoBoxTitleFontSize * 0.70 * fontScale;
                    ProductCompareDetailFontSize = ComparisonFontSize * 0.70 * fontScale;
                    ProductCompareTitleFontSize = ProductFontSize * 0.70 * fontScale;
                    ProductNameFontSize = ProductCircleSize / 20 * fontScale;
                    ProductOverNameFontSize = ProductNameFontSize * 0.7 * fontScale;
                    ProductInfoBoxHeight = height * 0.34;
                }
                else
                {
                    WhatsNewFontSize = WhatsNewFontSize * 0.8 * fontScale;
                    InfoBoxFontSize = InfoBoxFontSize * 0.78 * fontScale;
                    InfoBoxTitleFontSize = InfoBoxTitleFontSize * 0.86 * fontScale;
                    ProductCompareDetailFontSize = ComparisonFontSize * 0.78 * fontScale;
                    ProductCompareTitleFontSize = ProductFontSize * 0.74 * fontScale;
                    ProductNameFontSize = ProductCircleSize / 20 * fontScale;
                    ProductOverNameFontSize = ProductNameFontSize * 0.7 * fontScale;
                    ProductInfoBoxHeight = height * 0.40;
                    AssetDetailsFontSize = AssetDetailsFontSize * 0.9;
                }
                
            }
            else if (currentLangMode == "fr-FR")
            {
                if (fontScale < 1.0)
                {
                    WhatsNewFontSize = WhatsNewFontSize * 0.8 * fontScale;
                    InfoBoxFontSize = InfoBoxFontSize * 0.78 * fontScale;
                    InfoBoxTitleFontSize = InfoBoxTitleFontSize * 0.70 * fontScale;
                    ProductCompareDetailFontSize = ComparisonFontSize * 0.70 * fontScale;
                    ProductCompareTitleFontSize = ProductFontSize * 0.70 * fontScale;
                    ProductNameFontSize = ProductCircleSize / 20 * fontScale;
                    ProductOverNameFontSize = ProductNameFontSize * 0.7 * fontScale;
                    ProductInfoBoxHeight = height * 0.34;
                    
                }
                else
                {
                    WhatsNewFontSize = WhatsNewFontSize * 0.8 * fontScale;
                    InfoBoxFontSize = InfoBoxFontSize * 0.78 * fontScale;
                    InfoBoxTitleFontSize = InfoBoxTitleFontSize * 0.86 * fontScale;
                    ProductCompareDetailFontSize = ComparisonFontSize * 0.78 * fontScale;
                    ProductCompareTitleFontSize = ProductFontSize * 0.74 * fontScale;
                    ProductNameFontSize = ProductCircleSize / 20 * fontScale;
                    ProductOverNameFontSize = ProductNameFontSize * 0.7 * fontScale;
                    ProductInfoBoxHeight = height * 0.44;
                    AssetDetailsFontSize = AssetDetailsFontSize * 0.9;
                }                
            }
            else
            {
                if (fontScale < 1.0)
                {
                    WhatsNewFontSize = WhatsNewFontSize * fontScale * 0.88;
                    ProductCompareDetailFontSize = ComparisonFontSize * fontScale * 0.85;
                    ProductCompareTitleFontSize = ProductFontSize * fontScale * 0.85;
                    ProductInfoBoxHeight = height * 0.38;
                    ProductNameFontSize = ProductCircleSize / 15.5 * fontScale;
                    ProductOverNameFontSize = ProductNameFontSize * 0.85 * fontScale;
                    InfoBoxFontSize = InfoBoxFontSize * fontScale * 0.80;
                    InfoBoxTitleFontSize = InfoBoxTitleFontSize * fontScale * 0.82;

                    Debug.WriteLine("Setup");

                }
                else
                {
                    WhatsNewFontSize = WhatsNewFontSize * fontScale;
                    ProductCompareDetailFontSize = ComparisonFontSize * fontScale;
                    ProductCompareTitleFontSize = ProductFontSize * fontScale;
                    ProductInfoBoxHeight = height * 0.44;
                    ProductNameFontSize = ProductCircleSize / 15.5 * fontScale;
                    ProductOverNameFontSize = ProductNameFontSize * 0.7 * fontScale;
                }
                
            }

            /*
            if (isEnUS)
            {
                ProductCompareDetailFontSize = ComparisonFontSize;
                ProductCompareTitleFontSize = ProductFontSize;
                ProductInfoBoxHeight = height * 0.44;
                ProductNameFontSize = ProductCircleSize / 15.5;
                ProductOverNameFontSize = ProductNameFontSize * 0.7;
            } else
            {
                ProductCompareDetailFontSize = ComparisonFontSize * 0.78;
                ProductCompareTitleFontSize = ProductFontSize * 0.78;
                ProductInfoBoxHeight = height * 0.46;
                ProductNameFontSize = ProductCircleSize / 20;
                ProductOverNameFontSize = ProductNameFontSize * 0.7;
            } */

            if (questionsPage != null && rootFrame.SourcePageType == typeof(Questions)) questionsPage.updateAnswersGrid();
            if (productsPage != null && rootFrame.SourcePageType == typeof(ProductPage)) productsPage.updatePositions();
            if (whatsNewPage != null && rootFrame.SourcePageType == typeof(WhatsNew)) whatsNewPage.updatePosition();
        }

        protected override void OnActivated(IActivatedEventArgs args) {
            base.OnActivated(args);
            windowResize();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
