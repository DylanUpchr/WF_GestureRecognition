using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Cuda;
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
            CvInvoke.CvtColor(input, hsvImage, ColorConversion.Bgr2Hsv);
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
            skinMask = Mat.Zeros(input.Height, input.Width, DepthType.Cv8U, 3);

            if (!this.Calibrated)
            {
                return skinMask;
            }

            Mat hsvInput = new Mat();
            CvInvoke.CvtColor(input, hsvInput, ColorConversion.Rgb2Hsv);

            CvInvoke.InRange(
                hsvInput,
                new ScalarArray(new MCvScalar(hLowThreshold, sLowThreshold, vLowThreshold)),
                new ScalarArray(new MCvScalar(hHighThreshold, sHighThreshold, vHighThreshold)),
                skinMask);

            performOpening(skinMask, ElementShape.Ellipse, new Size(1, 1));
            CvInvoke.Dilate(skinMask, skinMask, new Mat(), new Point(1, 1), 5, BorderType.Default, new MCvScalar());

            return skinMask;
        }
        private void performOpening(Mat binaryImage, ElementShape kernelShape, Size kernelSize)
        {
           Mat structuringElement = CvInvoke.GetStructuringElement(kernelShape, kernelSize, new Point(0,0));

            CvInvoke.MorphologyEx(binaryImage, binaryImage, MorphOp.Open, structuringElement, new Point(0,0), 3, BorderType.Default, new MCvScalar());
        }
    }
}
