using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using IWF.V_Dump.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

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
            SetFree();

            for (int i = 1; i <= 60; i++)
            {
                FrameRates.Add(i);
            }

            SelectedFrameRate = FrameRates[0];

            
            if (IsInDesignMode)
            {
                SelectedFrame = VideoFrame.Default;

                IEnumerable<string> images = Directory.EnumerateFiles(@"C:\tmp\gpvid\frames", "thumb*.jpg");
                foreach (string image in images)
                {
                    Frames.Add(new VideoFrame()
                    {
                        FullPath = image,
                        ThumbnailPath = image
                    });
                }

                for (int i = 0; i < 10; i++)
                {
                    Frames[i].IsDuplicate = true;   
                }

                for (int i = 0; i < 20; i++)
                {
                    if (i % 2 == 0) Frames[i].HasFace = true;
                }

            }
        }

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }


        private bool _EnableControls;
        public bool EnableControls
        {
            get { return _EnableControls; }
            set
            {
                _EnableControls = value;
                RaisePropertyChanged("EnableControls");
            }
        }



        private string _VideoFilePath;
        public string VideoFilePath
        {
            get { return _VideoFilePath; }
            set
            {
                _VideoFilePath = value;
                RaisePropertyChanged("VideoFilePath");
            }
        }


        private VideoFrame _SelectedFrame;
        public VideoFrame SelectedFrame
        {
            get { return _SelectedFrame; }
            set
            {
                _SelectedFrame = value;
                RaisePropertyChanged("SelectedFrame");
            }
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


        private ObservableCollection<int> _FrameRates = new ObservableCollection<int>();
        public ObservableCollection<int> FrameRates
        {
            get { return _FrameRates; }
            set
            {
                _FrameRates = value;
                RaisePropertyChanged("FrameRates");
            }
        }



        private int _SelectedFrameRate;
        public int SelectedFrameRate
        {
            get { return _SelectedFrameRate; }
            set
            {
                _SelectedFrameRate = value;
                RaisePropertyChanged("SelectedFrameRate");
            }
        }


        private string _BusyMessage;
        public string BusyMessage
        {
            get { return _BusyMessage; }
            set
            {
                _BusyMessage = value;
                RaisePropertyChanged("BusyMessage");
            }
        }



        public ICommand BrowseFileCommand { get { return new RelayCommand(BrowseFile, CanBrowseFile); } }

        private bool CanBrowseFile()
        {
            return !IsBusy;
        }

        private void BrowseFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                Filter = "Video Files|*.avi;*.mpg;*.mpeg;*.mp4;*.ogg;*.mkv",
                Multiselect = false,
                Title = "Choose a video file"
            };

            if (fileDialog.ShowDialog() == true)
            {
                VideoFilePath = fileDialog.FileName;
            }
        }

        public ICommand ExtractFramesCommand { get { return new RelayCommand(ExtractFrames, CanExtractFrames); } }

        private bool CanExtractFrames()
        {
            return File.Exists(VideoFilePath);
        }

        private async void ExtractFrames()
        {
            SelectedFrame = VideoFrame.Default;
            Frames.Clear();

            TempManager.SetNewWorkingDir();

            SetBusy();

            List<VideoFrame> frameObjects = new List<VideoFrame>();

            await Task.Run(() =>
            {
                SetBusy("Extracting frames...");
                PullFrames(VideoFilePath, SelectedFrameRate, TempManager.WorkingDir.FullName);

                SetBusy("Generating thumbnails...");
                MakeThumbnails(VideoFilePath, SelectedFrameRate, TempManager.WorkingDir.FullName);

                SetBusy("Finishing up...");
                IEnumerable<FileInfo> allOutputFiles = TempManager.WorkingDir.EnumerateFiles("*.jpg");

                IEnumerable<FileInfo> framesOnly = allOutputFiles.Where(f => f.Name.StartsWith("thumb") == false);

                foreach (FileInfo frameFile in framesOnly)
                {
                    VideoFrame videoFrame = new VideoFrame()
                    {
                        FullPath = frameFile.FullName,
                        ThumbnailPath = allOutputFiles.FirstOrDefault(f => f.Name == "thumb_" + frameFile.Name).FullName,
                        IsSelected = false,
                        MD5 = HashHelper.GetMD5(frameFile.FullName),
                        SHA1 = HashHelper.GetSHA1(frameFile.FullName)
                    };

                    frameObjects.Add(videoFrame);
                }
            });

            Frames = new ObservableCollection<VideoFrame>(frameObjects);
            Frames[0].IsSelected = true;

            SetFree();
        }

        private void PullFrames(string path, int fps, string outDir)
        {
            if (outDir[outDir.Length - 1] != '\\')
            {
                outDir += "\\";
            }

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = ".\\ext\\ffmpeg.exe",
                Arguments = $"-i \"{path}\" -qscale:v 2 -vf fps={fps} \"{outDir}frame_%05d.jpg\"",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process process = new Process()
            {
                StartInfo = startInfo
            };

            process.Start();

            process.WaitForExit();
        }

        private void MakeThumbnails(string path, int fps, string outDir)
        {
            //List<string> fileNames = Directory.EnumerateFiles(outDir, "*.jpg").ToList();

            if (outDir[outDir.Length - 1] != '\\')
            {
                outDir += "\\";
            }

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = ".\\ext\\ffmpeg.exe",
                Arguments = $"-i \"{path}\" -qscale:v 2 -vf fps={fps},scale=100:-1 \"{outDir}thumb_frame_%05d.jpg\"",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process process = new Process()
            {
                StartInfo = startInfo
            };

            process.Start();

            process.WaitForExit();

            //Taking this bit out as appears quicker to re-do whole vid

            //foreach (string fileName in fileNames)
            //{
            //    string thumbName = "thumb_" + Path.GetFileName(fileName);


            //    //ProcessStartInfo startInfo = new ProcessStartInfo()
            //    //{
            //    //    FileName = ".\\ext\\ffmpeg.exe",
            //    //    Arguments = $"-i \"{fileName}\" -vf scale=100:-1 \"{outDir}{thumbName}\"",
            //    //    WindowStyle = ProcessWindowStyle.Hidden
            //    //};



            //    Process process = new Process()
            //    {
            //        StartInfo = startInfo
            //    };

            //    process.Start();

            //    process.WaitForExit();
            //}
        }

        private void SetBusy(string message = null)
        {
            IsBusy = true;
            EnableControls = false;
            CommandManager.InvalidateRequerySuggested();
            BusyMessage = message;
        }

        private void SetFree()
        {
            IsBusy = false;
            EnableControls = true;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}