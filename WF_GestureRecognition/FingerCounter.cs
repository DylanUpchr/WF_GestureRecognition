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
using Emgu.CV.Util;

namespace WF_GestureRecognition
{
    class FingerCounter
    {
        public FingerCounter()
        {

        }
        public Mat FindFingersCount(Mat inputImage, Mat frame)
        {
            Mat contoursImage = Mat.Zeros(inputImage.Height, inputImage.Width, DepthType.Cv8U, 1);

            if (inputImage.IsEmpty || inputImage.NumberOfChannels != 1)
            {
                return contoursImage;
            }

            VectorOfVectorOfPointF contours = new VectorOfVectorOfPointF();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(inputImage, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);

            if (contours.Size <= 0)
            {
                return contoursImage;
            }

            int biggestContourIndex = -1;
            double biggestArea = 0;

            for (int i = 0; i < contours.Size; i++)
            {
                double area = CvInvoke.ContourArea(contours[i], false);
                if (area > biggestArea)
                {
                    biggestArea = area;
                    biggestContourIndex = i;
                }
            }
            if (biggestContourIndex < 0)
            {
                return contoursImage;
            }

            VectorOfPoint hullPoints = new VectorOfPoint();
            VectorOfInt hullInts = new VectorOfInt();

            CvInvoke.ConvexHull(contours[biggestContourIndex], hullPoints, true);
            CvInvoke.ConvexHull(contours[biggestContourIndex], hullInts, true);

            Mat defects = new Mat();
            if (hullInts.Size > 3)
            {
                CvInvoke.ConvexityDefects(contours[biggestContourIndex], hullInts, defects);
            } else
            {
                return contoursImage;
            }

            Rectangle boundingRectangle = CvInvoke.BoundingRectangle(hullPoints);

            Point centerBoundingRectangle = new Point((boundingRectangle.X + boundingRectangle.Right) / 2, (boundingRectangle.Y + boundingRectangle.Bottom) / 2);
            VectorOfPoint startPoints;
            VectorOfPoint farPoints;

            /*for (int i = 0; i < defects.Size; i++)
            {
                startPoints.Push(contours[biggestContourIndex][defects[i]]);
            }*/


            //VectorOfPoint filteredStartPoints = CompactOnNeighborhoodMedian()
            CvInvoke.DrawContours(contoursImage, contours, biggestContourIndex, new MCvScalar(0, 255, 0));
            return contoursImage;
        }
        private VectorOfPoint CompactOnNeighborhoodMedian(VectorOfPoint points, double maxNeighborDistance)
        {

            return new VectorOfPoint();
        }
    }
}
