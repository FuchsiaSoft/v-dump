using GalaSoft.MvvmLight;
using IWF.V_Dump.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace IWF.V_Dump.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            //IEnumerable<string> images = Directory.EnumerateFiles(@"C:\tmp\gpvid\frames", "*.jpg");
            //foreach (string image in images)
            //{
            //    Frames.Add(new VideoFrame()
            //    {
            //        FullPath = image,
            //        ThumbnailPath = image
            //    });
            //}
        }


        private ObservableCollection<VideoFrame> _Frames = new ObservableCollection<VideoFrame>();
        public ObservableCollection<VideoFrame> Frames
        {
            get { return _Frames; }
            set
            {
                _Frames = value;
                RaisePropertyChanged("Frames");
            }
        }

    }
}