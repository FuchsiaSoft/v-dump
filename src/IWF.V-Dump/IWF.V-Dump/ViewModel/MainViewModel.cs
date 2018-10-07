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
using Shipwreck.Phash;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Ionic.Zip;
using CsvHelper;
using System.Text;
using System.Xml.Serialization;
using System.Windows;

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
            SelectedFrame = VideoFrame.Default;

            SetFree();

            for (int i = 1; i <= 60; i++)
            {
                FrameRates.Add(i);
            }

            SelectedFrameRate = FrameRates[2];

            
            if (IsInDesignMode)
            {

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

                for (int i = 0; i < 30; i++)
                {
                    if (i % 3 == 0) Frames[i].Show = false;
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
                _SelectedFrame = value ?? VideoFrame.Default;
                RaisePropertyChanged("SelectedFrame");

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(_SelectedFrame.FullPath, UriKind.Relative);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                ImageSource = image;
            }
        }


        private BitmapImage _ImageSource;
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            set
            {
                _ImageSource = value;
                RaisePropertyChanged("ImageSource");
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


        private double _ProgressMin;
        public double ProgressMin
        {
            get { return _ProgressMin; }
            set
            {
                _ProgressMin = value;
                RaisePropertyChanged("ProgressMin");
            }
        }


        private double _ProgressMax;
        public double ProgressMax
        {
            get { return _ProgressMax; }
            set
            {
                _ProgressMax = value;
                RaisePropertyChanged("ProgressMax");
            }
        }


        private double _ProgressValue;
        public double ProgressValue
        {
            get { return _ProgressValue; }
            set
            {
                _ProgressValue = value;
                RaisePropertyChanged("ProgressValue");
            }
        }


        private bool _ProgressIndeterminate = true;
        public bool ProgressIndeterminate
        {
            get { return _ProgressIndeterminate; }
            set
            {
                _ProgressIndeterminate = value;
                RaisePropertyChanged("ProgressIndeterminate");
            }
        }



        private bool _DupeShowAll = true;
        public bool DupeShowAll
        {
            get { return _DupeShowAll; }
            set
            {
                _DupeShowAll = value;
                RaisePropertyChanged("DupeShowAll");
            }
        }


        private bool _DupeShow;
        public bool DupeShow
        {
            get { return _DupeShow; }
            set
            {
                _DupeShow = value;
                RaisePropertyChanged("DupeShow");
            }
        }


        private bool _DupeHide;
        public bool DupeHide
        {
            get { return _DupeHide; }
            set
            {
                _DupeHide = value;
                RaisePropertyChanged("DupeHide");
            }
        }


        private bool _FaceAll = true;
        public bool FaceAll
        {
            get { return _FaceAll; }
            set
            {
                _FaceAll = value;
                RaisePropertyChanged("FaceAll");
            }
        }


        private bool _FaceShow;
        public bool FaceShow
        {
            get { return _FaceShow; }
            set
            {
                _FaceShow = value;
                RaisePropertyChanged("FaceShow");
            }
        }


        private bool _FaceHide;
        public bool FaceHide
        {
            get { return _FaceHide; }
            set
            {
                _FaceHide = value;
                RaisePropertyChanged("FaceHide");
            }
        }



        private string _ExportPath;
        public string ExportPath
        {
            get { return _ExportPath; }
            set
            {
                _ExportPath = value;
                RaisePropertyChanged("ExportPath");
            }
        }



        private bool _ExportCSV;
        public bool ExportCSV
        {
            get { return _ExportCSV; }
            set
            {
                _ExportCSV = value;
                RaisePropertyChanged("ExportCSV");
            }
        }


        private bool _ExportJson;
        public bool ExportJson
        {
            get { return _ExportJson; }
            set
            {
                _ExportJson = value;
                RaisePropertyChanged("ExportJson");
            }
        }


        private bool _ExportXml;
        public bool ExportXml
        {
            get { return _ExportXml; }
            set
            {
                _ExportXml = value;
                RaisePropertyChanged("ExportXml");
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

            int cpuCount = Environment.ProcessorCount;

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = ".\\ext\\ffmpeg.exe",
                Arguments = $"-i \"{path}\" -qscale:v 2 -vf fps={fps} -threads {cpuCount} \"{outDir}frame_%05d.jpg\"",
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

            int cpuCount = Environment.ProcessorCount;

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = ".\\ext\\ffmpeg.exe",
                Arguments = $"-i \"{path}\" -qscale:v 2 -vf fps={fps},scale=100:-1 -threads {cpuCount} \"{outDir}thumb_frame_%05d.jpg\"",
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

        public ICommand RunFacialDetectionCommand { get { return new RelayCommand(RunFacialDetection, CanRunFacialDetection); } }

        private bool CanRunFacialDetection()
        {
            if (IsBusy) return false;
            if (Frames != null && Frames.Count > 0) return true;
            return false;
        }

        private async void RunFacialDetection()
        {
            SetBusy("Detecting faces...");

            ProgressIndeterminate = false;
            ProgressMin = 0;
            ProgressMax = Frames.Count;
            ProgressValue = 0;

            await Task.Run(() =>
            {
                for (int i = 0; i < Frames.Count; i++)
                {
                    BusyMessage = $"Checking for faces {(i + 1).ToString("#,###")} of {Frames.Count.ToString("#,###")}";
                    ProgressValue = i;
                    Frames[i].HasFace = FaceHelper.HasFace(Frames[i].FullPath);
                }
            });

            ProgressIndeterminate = true;
            SetFree();
        }

        public ICommand CheckForDuplicatesCommand { get { return new RelayCommand(CheckForDuplicates, CanCheckForDuplicates); } }

        private bool CanCheckForDuplicates()
        {
            if (IsBusy) return false;
            if (Frames != null && Frames.Count > 0) return true;
            return false;
        }

        private async void CheckForDuplicates()
        {
            SetBusy("Detecting duplicates...");

            ProgressIndeterminate = false;
            ProgressMin = 0;
            ProgressMax = Frames.Count;
            ProgressValue = 0;

            await Task.Run(() =>
            {
                Digest currentReference = null;

                int progress = 0;

                Parallel.For(0, Frames.Count, i =>
                {
                    lock (BusyMessage)
                    {
                        BusyMessage = $"Computing phash {(progress + 1).ToString("#,###")} of {Frames.Count.ToString("#,###")}";
                        progress++;
                        ProgressValue = progress;
                    }

                    Frames[i].PHash = HashHelper.GetPHash(Frames[i].FullPath);
                });

                ProgressValue = 0;

                for (int i = 0; i < Frames.Count; i++)
                {
                    BusyMessage = $"Checking image {(i + 1).ToString("#,###")} of {Frames.Count.ToString("#,###")}";
                    ProgressValue = i;

                    //Frames[i].PHash = HashHelper.GetPHash(Frames[i].FullPath);

                    if (i == 0)
                    {
                        currentReference = Frames[i].PHash;
                    }
                    else
                    {
                        var score = ImagePhash.GetCrossCorrelation(currentReference, Frames[i].PHash);
                        if (score > 0.8)
                        {
                            Frames[i].IsDuplicate = true;
                        }
                        else
                        {
                            currentReference = Frames[i].PHash;
                        }
                    }
                }
            });

            ProgressIndeterminate = true;
            SetFree();
        }

        public ICommand DeleteCommand { get { return new RelayCommand(Delete, CanDelete); } }

        private bool CanDelete()
        {
            if (Frames != null && Frames.Any(f => f.IsSelected)) return true;
            return false;
        }

        private void Delete()
        {
            Frames = new ObservableCollection<VideoFrame>
                (Frames.Where(f => f.IsSelected == false));
        }


        public ICommand BrowseExportFileCommand { get { return new RelayCommand(BrowseExport, CanBrowseExport); } }

        private bool CanBrowseExport()
        {
            if (IsBusy) return false;
            if (Frames != null && Frames.Count > 0) return true;
            return false;
        }

        private void BrowseExport()
        {
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                DefaultExt = ".zip",
                Filter = "Zip file (*.zip)|*.zip",
                AddExtension = true,
                OverwritePrompt = true,
                Title = "Choose save location"
            };

            if (fileDialog.ShowDialog() == true)
            {
                ExportPath = fileDialog.FileName;
            }
        }

        public ICommand ExportCommand { get { return new RelayCommand(Export, CanExport); } }

        private bool CanExport()
        {
            return !string.IsNullOrWhiteSpace(ExportPath);
        }

        private async void Export()
        {
            SetBusy("Exporting... ");

            await Task.Run(() =>
            {
                List<OutputModel> output = Frames
                    .Select(f => new OutputModel()
                    {
                        Name = Path.GetFileName(f.FullPath),
                        MD5 = f.MD5,
                        SHA1 = f.SHA1,
                        HasFace = f.HasFace,
                        PHash = f.PHash == null ? string.Empty : f.PHash.ToString()
                    }).ToList();

                using (FileStream fileStream = File.Create(ExportPath))
                using (ZipFile zip = new ZipFile())
                {
                    if (ExportCSV)
                    {
                        string tempPath = Path.GetTempFileName();
                        using (TextWriter writer = File.CreateText(tempPath))
                        using (CsvWriter csvWriter = new CsvWriter(writer))
                        {
                            csvWriter.WriteHeader<OutputModel>();
                            csvWriter.WriteRecords(output);
                        }
                        zip.AddEntry("_output.csv", File.ReadAllBytes(tempPath));
                        File.Delete(tempPath);
                    }

                    if (ExportJson)
                    {
                        string json = JsonConvert.SerializeObject(output, Formatting.Indented);
                        zip.AddEntry("_output.json", Encoding.Unicode.GetBytes(json));
                    }

                    if (ExportXml)
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(output.GetType());

                        using (MemoryStream stream = new MemoryStream())
                        {
                            xmlSerializer.Serialize(stream, output);

                            zip.AddEntry("_output.xml", stream.ToArray());
                        }
                    }

                    foreach (VideoFrame frame in Frames)
                    {
                        zip.AddEntry(Path.GetFileName(frame.FullPath), File.ReadAllBytes(frame.FullPath));
                    }

                    zip.Save(fileStream);
                }
                
            });

            MessageBox.Show("Export completed");

            SetFree();
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