﻿using GalaSoft.MvvmLight;
using Shipwreck.Phash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWF.V_Dump.Model
{
    public class VideoFrame : ObservableObject
    {
        public string ThumbnailPath { get; set; }
        public string FullPath { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public Digest PHash { get; set; }


        private bool _Show = true;
        public bool Show
        {
            get { return _Show; }
            set
            {
                _Show = value;
                RaisePropertyChanged("Show");
            }
        }


        private bool _IsDuplicate;
        public bool IsDuplicate
        {
            get { return _IsDuplicate; }
            set
            {
                _IsDuplicate = value;
                RaisePropertyChanged("IsDuplicate");
            }
        }


        private bool _HasFace;
        public bool HasFace
        {
            get { return _HasFace; }
            set
            {
                _HasFace = value;
                RaisePropertyChanged("HasFace");
            }
        }



        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public static VideoFrame Default
        {
            get
            {
                return new VideoFrame()
                {
                    FullPath = "Resources\\NoImage.png"
                };
            }
        }
    }
}
