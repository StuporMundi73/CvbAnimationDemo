using Stemmer.Cvb;
using Stemmer.Cvb.Async;
using Stemmer.Cvb.Driver;

namespace CvbAnimationDemo
{
  /// <summary>
  /// Model representing the continually updated image source from CVB;
  /// might as well be tailored to be a GenICam.vin or other...
  /// </summary>
  public class CvbSourceModel
  {
    public CvbSourceModel()
    {
      // for simplicity we just open a dumb emu file...
      Device = DeviceFactory.Open(@"%cvb%\Tutorial\ClassicSwitch.emu");
      Image = Device.DeviceImage;
      StartStreaming();
    }

    /// <summary>
    /// Streaming function. Will not actually return the way it is implemented here,
    /// but that won't hurt as it is async and will be called only once; a nicer
    /// way to do it would of course be by adding a termination to the while loop
    /// </summary>
    private async void StartStreaming()
    {
      // of course the grab can and should be made switchable, but to keep the
      // demo simply we just hard-enable it...
      Device.Stream.Start();
      while (true)
      {
        using (StreamImage image = await Device.Stream.WaitAsync())
        {
          // ... processing could go here...
          // btw: Display will be notified of the changes through the image's
          // PixelContentChanged event, so you won't have to care of display update
          // as long as the stream's "Wait" method is called (which happens in the
          // "WaitAsync" call...)
        }
      }
    }

    public Device Device
    { get; private set; }

    public Image Image
    { get; private set; }
  }
}
