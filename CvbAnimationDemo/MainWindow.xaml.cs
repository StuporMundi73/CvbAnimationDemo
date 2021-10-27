using System.Windows;

namespace CvbAnimationDemo
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    /// <summary>
    /// View Model. 
    /// In more complex cases this notation is often used to economize on
    /// typecasts...
    /// </summary>
    public MainViewModel ViewModel
    { get { return DataContext as MainViewModel; } }

    /// <summary>
    /// Closing notification
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      // it's usually okay to propagate events to the view model in code behind; alternatives
      // exist, but are complex to handle and debug and offer little merit in return
      ViewModel?.OnAppClosing();
    }
  }
}
