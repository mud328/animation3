using Avalonia.Controls;
using Avalonia;
using Avalonia.Media;


namespace MyAnimationApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
       
          WindowState = WindowState.FullScreen;
            Background = Brushes.Black;
            Content = new Grid
            {
                Children =
                {
                    new LizardAnimation(),
                    new SevenSegmentClock
                    {
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                        Margin = new Thickness(70, 0, 0, 30)
                    }
                }
            };
    }
}