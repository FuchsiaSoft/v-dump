using Accord.Vision.Detection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWF.V_Dump.Model
{
    public class FaceHelper
    {
        public static bool HasFace(string imagePath)
        {
            var cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();

            var detector = new HaarObjectDetector(cascade, minSize: 50,
                searchMode: ObjectDetectorSearchMode.NoOverlap);

            using (Image image = Image.FromFile(imagePath))
            using (Bitmap bmp = new Bitmap(image))
            {
                detector.UseParallelProcessing = true;
                Rectangle[] result = detector.ProcessFrame(bmp);

                if (result != null && result.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
