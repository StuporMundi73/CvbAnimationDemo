using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Stemmer.Cvb;
using Stemmer.Cvb.Extensions;

namespace CvbAnimationDemo
{
  /// <summary>
  /// our view model...
  /// </summary>
  public class MainViewModel : INotifyPropertyChanged
  {
    public MainViewModel()
    {
      // with a dependency injection framework like Prism this would actually be nicer and more modular;
      // but for the purpose of this demo I guess this will suffice...
      CvbSource = new CvbSourceModel();
      Animation = new AnimatedBitmapModel(CvbSource.Image.Size.ToSize());
      Animation.AnimationStep += (s, a) =>
      {
        // note: the timer is in a thread of its own, so we need to sync to the UI thread here
        Application.Current.Dispatcher.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnimatedBitmap))));
      };
    }

    /// <summary>
    /// Closing event needs to be announced to the model, otherwise we
    /// will have a runaway timer handler accessing invalid objects
    /// </summary>
    public void OnAppClosing()
    {
      Animation?.TerminateAnimation();
    }

    /// <summary>
    /// The animated bitmap for overlaying...
    /// Note that we could actually access this through "Animation.AnimatedBitmap" 
    /// as well in the binding expression, but having this as a separate property on the
    /// view model allows us to separate it for the purpose of the 
    /// INotifyPropertyChanged interface, which can help save resources
    /// </summary>
    public ImageSource AnimatedBitmap
    { get { return Animation?.AnimatedBitmap; } }

    /// <summary>
    /// The connection to the image source (device)
    /// </summary>
    public CvbSourceModel CvbSource
    {
      get { return _cvbSource; }
      private set
      {
        if (value == _cvbSource)
          return;
        _cvbSource = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CvbSource)));
      }
    }
    private CvbSourceModel _cvbSource;

    /// <summary>
    /// The connection to the animated bitmap
    /// </summary>
    public AnimatedBitmapModel Animation
    {
      get { return _animatedBitmapModel; }
      private set
      {
        if (value == _animatedBitmapModel)
          return;
        _animatedBitmapModel = value;
        if (_animatedBitmapModel != null)
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnimatedBitmap))); // this will trigger an update of the binding!
      }
    }
    private AnimatedBitmapModel _animatedBitmapModel;

    /// <summary>
    /// INotifyPropertyChanged implementation (just one event...)
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
  }
}
