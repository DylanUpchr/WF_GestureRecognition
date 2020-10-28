using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace WF_GestureRecognition
{
    public class SkinDetector
    {
        private double hLowThreshold;
        private double hHighThreshold;
        private double sLowThreshold;
        private double sHighThreshold;
        private double vLowThreshold;
        private double vHighThreshold;
        public bool Calibrated { get; private set; }
        public SkinDetector()
        {
            this.Calibrated = false;
            hLowThreshold = 0;
            hHighThreshold = 0;
            sLowThreshold = 0;
            sHighThreshold = 0;
            vLowThreshold = 0;
            vHighThreshold = 0;
        }
        /// <summary>
        /// Draw sampling rectangles on image
        /// </summary>
        /// <param name="input"></param>
        static public Mat DrawSkinColorSampler(Mat input, Rectangle r1, Rectangle r2)
        {
            CvInvoke.Rectangle(input, r1, new MCvScalar(0, 0, 255));
            CvInvoke.Rectangle(input, r2, new MCvScalar(0, 0, 255));
            return input;
        }
        public void Calibrate(Mat input, Rectangle r1, Rectangle r2)
        {
            Mat sample1, sample2, hsvImage = new Mat();
            CvInvoke.CvtColor(input, hsvImage, ColorConversion.Rgb2Hsv);
            sample1 = new Mat(hsvImage, r1);
            sample2 = new Mat(hsvImage, r2);
            CalculateThresholds(sample1, sample2);
            this.Calibrated = true;
        }
        private void CalculateThresholds(Mat sample1, Mat sample2)
        {
            int offsetLowThreshold = 80;
            int offsetHighThreshold = 30;

            MCvScalar hsvMeansSample1 = CvInvoke.Mean(sample1);
            MCvScalar hsvMeansSample2 = CvInvoke.Mean(sample2);
            hLowThreshold = Math.Min(hsvMeansSample1.V0, hsvMeansSample2.V0) - offsetLowThreshold;
            hHighThreshold = Math.Max(hsvMeansSample1.V0, hsvMeansSample2.V0) + offsetHighThreshold;
            
            sLowThreshold = Math.Min(hsvMeansSample1.V1, hsvMeansSample2.V1) - offsetLowThreshold;
            sHighThreshold = Math.Max(hsvMeansSample1.V1, hsvMeansSample2.V1) + offsetHighThreshold;

            // the V channel shouldn't be used. By ignoring it, shadows on the hand wouldn't interfire with segmentation.
            // Unfortunately there's a bug somewhere and not using the V channel causes some problem. This shouldn't be too hard to fix.
            vLowThreshold = Math.Min(hsvMeansSample1.V2, hsvMeansSample2.V2) - offsetLowThreshold;
            vHighThreshold = Math.Max(hsvMeansSample1.V2, hsvMeansSample2.V2) + offsetHighThreshold;
        }
        public Mat GetSkinMask(Mat input)
        {
            Mat skinMask;

            if (!this.Calibrated)
            {
                //skinMask = Mat::zeros(input.size(), CV_8UC1);
                skinMask = Mat.Zeros(input.Height, input.Width, DepthType.Cv8U, 3);
                return skinMask;
            }

            /*Mat hsvInput;
            cvtColor(input, hsvInput, CV_BGR2HSV);

            inRange(
                hsvInput,
                Scalar(hLowThreshold, sLowThreshold, vLowThreshold),
                Scalar(hHighThreshold, sHighThreshold, vHighThreshold),
                skinMask);

            performOpening(skinMask, MORPH_ELLIPSE, { 3, 3 });
            dilate(skinMask, skinMask, Mat(), Point(-1, -1), 3);*/

            return skinMask;
        }
    }
}
