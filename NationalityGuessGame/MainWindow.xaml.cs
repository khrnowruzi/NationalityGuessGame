using NationalityGuessGame.HelpClasses;
using NationalityGuessGame.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace NationalityGuessGame
{
    public partial class MainWindow : Window
    {
        //=========================================Fields========================================
        private readonly MainViewModel _viewModel;

        protected bool isDragging;
        private Point clickPosition;
        private TranslateTransform originTT;

        private Point startPosition;
        private double nationalityButtonStartXPoint;
        private double nationalityButtonStartYPoint;

        private string currentImagePath;
        private int score;
        private bool clickedNationalityButton;
        private int numberOfGeneratedImages;
        private List<int> distinctList;

        Storyboard opacityStoryBoard;

        private List<NationalityNameEnum> nationalityNames;
        public List<string> imagesPathList { get; private set; }

        //==========================================Ctor=========================================
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
        }

        //=========================================Methods=======================================
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            imagesPathList = _viewModel.GetPath();

            nationalityButton.Width = Constants.NationalityButtonWith;
            nationalityNames = new List<NationalityNameEnum>();
            distinctList = new List<int>();

            nationalityButtonStartXPoint = (canvas.ActualWidth - nationalityButton.Width) / 2;
            nationalityButtonStartYPoint = Constants.NationalityButtonCanvasTop;
            Canvas.SetLeft(nationalityButton, nationalityButtonStartXPoint);

            Canvas.SetLeft(startButton, (canvas.ActualWidth - startButton.Width) / 2);
            Canvas.SetTop(startButton, (canvas.ActualHeight - startButton.Height) / 2);

            Canvas.SetLeft(scoreBorder, (canvas.ActualWidth - scoreBorder.Width) / 2);
            Canvas.SetBottom(scoreBorder, 0);
        }

        private void GenerateNextImage()
        {
            if (++numberOfGeneratedImages > imagesPathList.Count)
            {
                EndGame();
                return;
            }

            clickedNationalityButton = false;
            GenerateImage();
            BeginFadeOutNationalityButton();
            BeginMoveNationalityButtonToBottom();
        }

        private void GenerateImage()
        {
            int index = GenerateRandomNumber(distinctList, imagesPathList.Count);
            distinctList.Add(index);
            currentImagePath = imagesPathList[index];
            Uri resourceUri = new Uri($"/{Constants.ImagesFolderName}/{currentImagePath}", UriKind.Relative);
            nationalityImage.Source = new BitmapImage(resourceUri);
        }

        private int GenerateRandomNumber(List<int> distinctList, int maxValue)
        {
            var random = new Random();
            int index = 0;
            do
            {
                index = random.Next(maxValue);
            }
            while (distinctList.Any(x => x == index));

            return index;
        }

        private void ResetAndStartGame()
        {
            isDragging = default;
            clickPosition = default;
            originTT = default;
            startPosition = default;
            currentImagePath = default;
            score = default;
            clickedNationalityButton = default;
            numberOfGeneratedImages = default;
            distinctList.Clear();
            opacityStoryBoard = default;
            nationalityNames.Clear();

            scoreBorder.Visibility = Visibility.Collapsed;
            nationalityButton.Visibility = Visibility.Visible;

            GenerateNextImage();
        }

        private void EndGame()
        {
            scoreBorder.Visibility = Visibility.Visible;
            scoreLable.Content = $"Score: {this.score}";
            nationalityButton.Visibility = Visibility.Collapsed;
        }

        private void CalculateScore()
        {
            if (!nationalityNames.Any())
                return;

            string currentNationalityName = currentImagePath.Split('/').FirstOrDefault();
            var currentNationalityNameEnum = Enum.Parse<NationalityNameEnum>(currentNationalityName);

            var destinationNationality = nationalityNames
                .GroupBy(nationalityName => nationalityName)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .FirstOrDefault();

            if (destinationNationality == currentNationalityNameEnum)
                score += Constants.ScoreInc;
            else
                score -= Constants.ScoreDec;
        }

        private void BeginFadeOutNationalityButton()
        {
            opacityStoryBoard = new Storyboard();
            DoubleAnimation opacityAnimation = new DoubleAnimation(
                1.0, 0.0, new Duration(new TimeSpan(0, 0, Constants.AnimateSecondsTime)), FillBehavior.Stop);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            opacityStoryBoard.Children.Add(opacityAnimation);
            opacityStoryBoard.Completed += delegate { GenerateNextImage(); };

            nationalityButton.BeginStoryboard(opacityStoryBoard);
        }

        //=========================================Moving========================================
        private void BeginMoveNationalityButtonToBottom()
        {
            TranslateTransform transform = new TranslateTransform();
            nationalityButton.RenderTransform = transform;
            MoveNationalityButtonOnYAxis(nationalityButtonStartYPoint,
                canvas.ActualHeight - nationalityButtonStartYPoint
                , transform);
        }

        private void MoveNationalityButtonOnXAxis(double xPointFrom, double xPointTo, TranslateTransform transform)
        {
            DoubleAnimation animationX = new DoubleAnimation(
                xPointFrom, xPointTo, new Duration(new TimeSpan(0, 0, Constants.AnimateSecondsTime)));

            transform.BeginAnimation(TranslateTransform.XProperty, animationX);
        }

        private void MoveNationalityButtonOnYAxis(double yPointFrom, double yPointTo, TranslateTransform transform)
        {
            DoubleAnimation animationY = new DoubleAnimation(
                yPointFrom, yPointTo, new Duration(new TimeSpan(0, 0, Constants.AnimateSecondsTime)));

            transform.BeginAnimation(TranslateTransform.YProperty, animationY);
        }

        private void CalculateForMovingNationalityButton()
        {
            Point currentPosition = nationalityButton.TransformToAncestor(canvas).Transform(new Point(0, 0));
            TranslateTransform transform = new TranslateTransform();
            nationalityButton.RenderTransform = transform;

            nationalityNames.Clear();
            if (currentPosition.X > startPosition.X) //go to right
            {
                MoveNationalityButtonOnXAxis(
                    currentPosition.X - nationalityButtonStartXPoint,
                    canvas.ActualWidth - nationalityButtonStartXPoint,
                    transform);

                nationalityNames.Add(NationalityNameEnum.Chinese);
                nationalityNames.Add(NationalityNameEnum.Thai);
            }
            else //go to left
            {
                MoveNationalityButtonOnXAxis(
                    currentPosition.X - nationalityButtonStartXPoint - nationalityButton.ActualWidth,
                    -nationalityButtonStartXPoint - nationalityButton.ActualWidth,
                    transform);

                nationalityNames.Add(NationalityNameEnum.Japanese);
                nationalityNames.Add(NationalityNameEnum.Korean);
            }

            if (currentPosition.Y > startPosition.Y) //go to bottom
            {
                MoveNationalityButtonOnYAxis(
                    currentPosition.Y - nationalityButtonStartYPoint,
                    canvas.ActualHeight - nationalityButtonStartYPoint,
                    transform);

                nationalityNames.Add(NationalityNameEnum.Korean);
                nationalityNames.Add(NationalityNameEnum.Thai);
            }
            else //go to top
            {
                MoveNationalityButtonOnYAxis(
                    currentPosition.Y - nationalityButtonStartYPoint - nationalityButton.ActualHeight,
                    -nationalityButtonStartYPoint - nationalityButton.ActualHeight,
                    transform);

                nationalityNames.Add(NationalityNameEnum.Japanese);
                nationalityNames.Add(NationalityNameEnum.Chinese);
            }
        }

        //=======================================MouseEvent======================================
        private void NationalityButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (clickedNationalityButton)
                return;
            var draggableControl = sender as Button;
            originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
            isDragging = true;
            clickPosition = e.GetPosition(this);
            draggableControl.CaptureMouse();
            startPosition = nationalityButton.TransformToAncestor(canvas).Transform(new Point(0, 0));
        }

        private void NationalityButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (clickedNationalityButton)
                return;
            isDragging = false;
            var draggableControl = sender as Button;
            draggableControl.ReleaseMouseCapture();

            CalculateForMovingNationalityButton();
            CalculateScore();
            clickedNationalityButton = true;
        }

        private void NationalityButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (clickedNationalityButton)
                return;
            var draggableControl = sender as Button;
            if (isDragging && draggableControl != null)
            {
                Point currentPosition = e.GetPosition(this);
                var transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);
                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
            }
        }

        //=======================================ClickEvent======================================
        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartAgain(object sender, RoutedEventArgs e)
        {
            ResetAndStartGame();
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            startButton.Visibility = Visibility.Collapsed;
            nationalityButton.Visibility = Visibility.Visible;
            GenerateNextImage();
        }
    }
}
