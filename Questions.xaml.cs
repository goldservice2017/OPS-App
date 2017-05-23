using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class Questions : Page {

        public App App = App.app;
        const double AnswersBorderThickness = 1.5;
        const int NumberOfQuestions = 4;
        const int Interval = 16;
        const double PopupFactor = 2.5;
        const double TrackingDivider = 200;
        const double TargetTimeDivider = 3.5;
        const int AnswerPopInTime = 500;
        const int FadeOutTime = 600;
        const int CloseAnswerColorFadeTime = 400;
        const int CloseAnswerPopOutTime = 800;
        const int CloseAnswerPopOutPhase = 800;
        const double AnswersXOffset = 0.08;

        DispatcherTimer timer = new DispatcherTimer();
        double x, y, radius, answerHeight;
        double time = 0, targetTime;

        bool animating, closing;
        int questionId = 0;
        int answersToClose, answersToAppear;
        int button_click_numbers = 0;

        int[] answers = new int[NumberOfQuestions];
        string[] questionText = new string[NumberOfQuestions];
        string[] answerText = new string[NumberOfQuestions];
        string[] infoBoxTitle = new string[NumberOfQuestions];
        string[] infoBoxDescription = new string[NumberOfQuestions];
        List<int> possibleProducts;

        public Questions() {
            this.InitializeComponent();
            this.Width = Double.NaN;
            this.Height = Double.NaN;
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, Interval);
            timer.Start();
            App.questionsPage = this;

            for (int i = 0; i < NumberOfQuestions; i++) {
                String n = (i + 1).ToString();
                questionText[i] = App.ResourceLoader.GetString("Question" + n);
                answerText[i] = App.ResourceLoader.GetString("Answers" + n);
                infoBoxTitle[i] = App.ResourceLoader.GetString("QuestionInfoTitle" + n);
                infoBoxDescription[i] = App.ResourceLoader.GetString("QuestionInfoDescription" + n);
            }
            App.windowResize();

            showQuestion(0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            App.windowResize();
        }

        private void showQuestion(int id) {

            //Debug.WriteLine("Show Question Animation.");

            animating = true;
            questionId = id;
            questionTextBlock.Opacity = 0;
            questionTextBlock.Text = questionText[id];
            Storyboards.FadeIn(questionTextBlock, AnswerPopInTime, popInAnswers);
            infoBox.Title = infoBoxTitle[id];
            infoBox.Description = infoBoxDescription[id];
            button_click_numbers = 0;
        }

        private void popInAnswers(object sender, object e) {

            //Debug.WriteLine("Pop In Answers Animation.");

            string[] answers = answerText[questionId].Split('|');
            answersToClose = answers.Length;
            answersToAppear = answersToClose;
            updateAnswersGrid();
            answersGrid.Children.Clear();
            answersGrid.ColumnDefinitions.Clear();
            answersGrid.RowDefinitions.Clear();
            for (int i = 0; i < answers.Length; i++) {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1.0, GridUnitType.Star);
                answersGrid.RowDefinitions.Add(row);
            }
            int phase = 0;
            for (int i = 0; i < answersToClose; i++) {
                Grid grid = new Grid();
                grid.Tag = "" + questionId + "|" + i;
                grid.Tapped += setAnswer;
                grid.Background = (SolidColorBrush)App.Current.Resources["White"];
                grid.BorderBrush = (SolidColorBrush)App.Current.Resources["Orange"];
                grid.BorderThickness = new Thickness(0, i == 0 ? AnswersBorderThickness : 0, AnswersBorderThickness, AnswersBorderThickness);

                TextBlock tblock = new TextBlock();
                Binding myBinding = new Binding();
                myBinding.Source = App;
                myBinding.Path = new PropertyPath("AnswerFontSize");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(tblock, TextBlock.FontSizeProperty, myBinding);
                tblock.Style = (Style)App.Current.Resources["OrangeText"];
                tblock.HorizontalAlignment = HorizontalAlignment.Center;
                tblock.VerticalAlignment = VerticalAlignment.Center;
                tblock.Text = answers[i];

                grid.Children.Add(tblock);
                answersGrid.Children.Add(grid);
                Grid.SetRow(grid, i);
                Storyboards.MoveX(grid, -radius, AnswersXOffset * radius, CloseAnswerPopOutTime + phase, answerAppeared);
                phase += CloseAnswerPopOutPhase;
            }
        }

        public void updateAnswersGrid() {

            //Debug.WriteLine("Update Answers Animation.");

            radius = App.QuestionsOuterCircleDiameter / 2;
            answerHeight = radius * 0.3;
            answersGrid.Width = radius * 1.1;
            answersGrid.Height = answerHeight * answersToClose;
            answersGrid.Margin = new Thickness(radius * PopupFactor, 0, 0, 0);
        }

        private void answerAppeared(object sender, object e) {

            //Debug.WriteLine("Answers Appreared Animation.");

            answersToAppear--;
            if (answersToAppear == 0) animating = false;
        }

        private void setAnswer(object sender, TappedRoutedEventArgs e) {

            //Debug.WriteLine("Set Answer.");            
            Grid grid = sender as Grid;
            int id = Int32.Parse(grid.Tag.ToString().Split('|')[0]);
            int value = Int32.Parse(grid.Tag.ToString().Split('|')[1]); 
            
            if (button_click_numbers > 0)
            {
                return;
            }
            

            closing = true;
           
            const double ff = 0.3;
            const double Pi2 = Math.PI * 2;
            time += Pi2;
            time = time % Pi2;
            targetTime = value * ff - (ff * answersToClose / 2) + ff / 2;
            if (value == 0) targetTime += Pi2;
            answers[id] = value;

            possibleProducts = new List<int>();
            for (int i = 0; i < 6; i++) possibleProducts.Add(i);

            for (int i = 0; i <= id; i++)
                switch (i) {
                    case 0:
                        if (answers[i] == 0 || answers[i] == 2) filterProducts(13, false); // PC
                        if (answers[i] == 1 || answers[i] == 2) filterProducts(12, false); // Mac
                        break;
                    case 1:
                        filterProducts(11, answers[i] != 1); // 5 users
                        break;
                    case 2:
                        filterProducts(4, answers[i] != 0); // Outlook
                        break;
                    case 3:
                        filterProducts(10, answers[i] != 0); // Support
                        break;
                }

            Storyboards.ColorTransition(grid, CloseAnswerColorFadeTime,
                ((SolidColorBrush)App.Current.Resources["Orange"]).Color,
                ((SolidColorBrush)App.Current.Resources["White"]).Color,
                closeAnswers,
                "(Grid.Background).(SolidColorBrush.Color)");
            button_click_numbers++;
        }

        private void filterProducts(int feature, bool invert) {
            //Debug.WriteLine("Filter Products.");
            var filteredList = possibleProducts.Where(p => invert ? !ProductPage.ProductFeatures[p][feature] : ProductPage.ProductFeatures[p][feature]);
            possibleProducts = filteredList.ToList();
        }

        private void closeAnswers(object sender, object e) {
            //Debug.WriteLine("Close Answers Animation.");
            int phase = 0;
            Storyboards.FadeOut(questionTextBlock, AnswerPopInTime, null);
            foreach (Grid child in answersGrid.Children) {
                double radius = App.QuestionsOuterCircleDiameter / 2;
                Storyboards.MoveX(child, AnswersXOffset * radius, -radius, CloseAnswerPopOutTime + phase, answerClosing);
                phase += CloseAnswerPopOutPhase;
            }
        }

        private void answerClosing(object sender, object e) {
            answersToClose--;
            if (answersToClose == 0) {
                closing = false;
                if (possibleProducts.Count == 1) {
                    //Storyboards.DissapearLeft(rootGrid, toLoadingPage);
                    timer.Stop();
                    answersGrid.Visibility = Visibility.Collapsed;
                    Storyboards.FadeOut(dotImage, FadeOutTime, toLoadingPage);
                    Storyboards.FadeOut(backButton, FadeOutTime, null);
                    Storyboards.FadeOut(infoImage, FadeOutTime, null);
                    Storyboards.FadeOut(questionOuterCircle, FadeOutTime, null);
                    if (infoBox.Opacity > 0) Storyboards.Fade(infoBox, FadeOutTime, infoBox.Opacity, 0, null);
                    App.SelectedProduct = possibleProducts[0];
                }
                else
                    showQuestion(questionId + 1);
            }
        }

        private void Timer_Tick(object sender, object e) {
            //Debug.WriteLine("Time Tick Animation.");
            if (closing)
                time += (targetTime - time) / TargetTimeDivider;
            else
                time += (double)Math.PI / TrackingDivider;
            x = Math.Cos(time) * (App.QuestionsOuterCircleDiameter * 0.95);
            y = Math.Sin(time) * (App.QuestionsOuterCircleDiameter * 0.95);
            dotImage.Margin = new Thickness(x, y, 0, 0);
        }

        private void infoImage_Tapped(object sender, TappedRoutedEventArgs e) {
            infoBox.Visibility = Visibility.Visible;
            infoBox.Opacity = 0;
            Storyboards.FadeIn(infoBox, AnswerPopInTime, null);
        }

        private void back_Tapped(object sender, TappedRoutedEventArgs e) {
            //if (animating) return;
            if (questionId > 0)
                showQuestion(questionId - 1);
            else
                Storyboards.DissapearRight(rootGrid, backToHome);
        }

        private void backToHome(object sender, object e) {
            this.Frame.Navigate(typeof(HomePage));
        }

        private void toLoadingPage(object sender, object e) {
            ProductPage.BackPage = typeof(Questions);
            ProductPage.FadeoutCircle = true;
            App.rootFrame.ContentTransitions.Clear();
            this.Frame.Navigate(typeof(ProductPage));
        }

        private void infoBoxCollapse(object sender, object e) {
            infoBox.Visibility = Visibility.Collapsed;
        }

        private void infoBox_Tapped(object sender, TappedRoutedEventArgs e) {
            Storyboards.FadeOut(infoBox, AnswerPopInTime, infoBoxCollapse);
        }
    }
}
