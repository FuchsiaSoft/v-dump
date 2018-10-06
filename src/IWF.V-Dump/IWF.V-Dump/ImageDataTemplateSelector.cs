using IWF.V_Dump.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IWF.V_Dump
{
    public class ImageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate DuplicateTemplate { get; set; }
        public DataTemplate FaceTemplate { get; set; }
        public DataTemplate DuplicateFaceTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            VideoFrame videoFrame = (VideoFrame)item;

            if (videoFrame.IsDuplicate && videoFrame.HasFace) return DuplicateFaceTemplate;
            if (videoFrame.IsDuplicate) return DuplicateTemplate;
            if (videoFrame.HasFace) return FaceTemplate;
            return DefaultTemplate;
        }
    }
}
