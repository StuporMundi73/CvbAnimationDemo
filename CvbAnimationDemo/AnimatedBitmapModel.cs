using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CvbAnimationDemo
{
  /// <summary>
  /// The data model representing the animated overlay
  /// </summary>
  public class AnimatedBitmapModel
  {
    /// <summary>
    /// boring ctor
    /// </summary>
    /// <param name="dimensions"></param>
    public AnimatedBitmapModel(System.Drawing.Size dimensions)
    {
      InitAnimatedBitmap(dimensions.Width, dimensions.Height);
      _animationTimer = new Timer((x) => this.OnAnimationStep(), null, 0, 25);
    }

    /// <summary>
    /// initialize the data needed for the animated overlay
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    protected void InitAnimatedBitmap(int width, int height)
    {
      _pf = PixelFormats.Bgra32;
      _rawStride = (width * _pf.BitsPerPixel + 7) / 8;
      _rawPixels = new byte[_rawStride * height];
      _size = new System.Drawing.Size(width, height);
      OnAnimationStep();
    }

    /// <summary>
    /// prepare the next animation step
    /// </summary>
    protected void OnAnimationStep()
    {
      var xPos = _cycle % _size.Width;
      var yPos = _cycle % _size.Height;
      var pixelStride = _pf.BitsPerPixel / 8;
      for (var y = 0; y < _size.Height; ++y)
      {
        var lineBase = y * _rawStride;
        for (var x = 0; x < _size.Width; ++x)
        {
          var isInCrossHair = (x == xPos) || (y == yPos);
          var pixelBase = lineBase + x * pixelStride;
          _rawPixels[pixelBase] = (byte)(isInCrossHair ? 255 : 0); // blue
          _rawPixels[pixelBase + 1] = (byte)0; // green
          _rawPixels[pixelBase + 2] = (byte)0; // red
          _rawPixels[pixelBase + 3] = (byte)(isInCrossHair ? 255 : 0); // transparency
        }
      }
      try
      {
        Application.Current.Dispatcher.Invoke(() => AnimatedBitmap = BitmapSource.Create(_size.Width, _size.Height, 96, 96, _pf, null, _rawPixels, _rawStride));
      }
      catch(Exception)
      { }
      ++_cycle;
      AnimationStep?.Invoke(this, null);
    }

    /// <summary>
    /// stop the animation time for good
    /// </summary>
    public void TerminateAnimation()
    {
      _animationTimer.Dispose();
    }

    /// <summary>
    /// The actual bitmap source that will be accessible to the view model
    /// </summary>
    public ImageSource AnimatedBitmap
    { get; private set; }

    /// <summary>
    /// array with the raw pixel data
    /// </summary>
    private byte[] _rawPixels;

    /// <summary>
    /// memorized size of the image represented by _rawPixels
    /// </summary>
    private System.Drawing.Size _size;

    /// <summary>
    /// memorized stride of the image represented by _rawPixels
    /// </summary>
    private int _rawStride;

    /// <summary>
    /// pixel format of the image represented by _rawPixels
    /// </summary>
    private PixelFormat _pf;

    /// <summary>
    /// state variable for painting the "animation"
    /// </summary>
    private int _cycle = 0;

    /// <summary>
    /// Time object for the animation
    /// </summary>
    private System.Threading.Timer _animationTimer;

    /// <summary>
    /// for notifying consumers about new image content
    /// </summary>
    public event EventHandler AnimationStep;
  }
}
