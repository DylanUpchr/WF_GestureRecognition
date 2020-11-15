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
        const float BOUNDING_RECT_FINGER_SIZE_SCALING = 0.27F;
        const float BOUNDING_RECT_NEIGHBOR_DISTANCE_SCALING = 0.05F;
        const int LIMIT_ANGLE_SUP = 60;
        const int LIMIT_ANGLE_INF = 5;
        const int DRAW_THICKNESS = 5;
        public FingerCounter()
        {

        }
        public Mat FindFingersCount(Mat inputImage, Mat frame)
        {
            Mat contoursImage = Mat.Ones(inputImage.Height, inputImage.Width, DepthType.Cv8U, 3);

            if (inputImage.IsEmpty || inputImage.NumberOfChannels != 1)
            {
                return contoursImage;
            }

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
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
            CvInvoke.ConvexHull(contours[biggestContourIndex], hullInts, false);

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
            VectorOfPoint startPoints = new VectorOfPoint();
            VectorOfPoint farPoints = new VectorOfPoint();

            int[,,] defectsData = (int[,,])defects.GetData();
            for (int i = 0; i < defectsData.Length / 4; i++)
            {
                Point startPoint = contours[biggestContourIndex][defectsData[i, 0, 0]];
                if (!startPoints.ToArray().ToList().Any(p => Math.Abs(p.X - startPoint.X) < 30 && Math.Abs(p.Y - startPoint.Y) < 30))
                {
                    VectorOfPoint startPointVector = new VectorOfPoint(new Point[] { startPoint });
                    startPoints.Push(startPointVector);
                }
                Point farPoint = contours[biggestContourIndex][defectsData[i, 0, 2]];
                if (findPointsDistance(farPoint, centerBoundingRectangle) < boundingRectangle.Height * BOUNDING_RECT_FINGER_SIZE_SCALING)
                {
                    VectorOfPoint farPointVector = new VectorOfPoint(new Point[] { farPoint });
                    farPoints.Push(farPointVector);
                }
            }


            VectorOfPoint filteredStartPoints = CompactOnNeighborhoodMedian(startPoints, boundingRectangle.Height * BOUNDING_RECT_NEIGHBOR_DISTANCE_SCALING);
            VectorOfPoint filteredFarPoints = CompactOnNeighborhoodMedian(farPoints, boundingRectangle.Height * BOUNDING_RECT_NEIGHBOR_DISTANCE_SCALING);
            
            VectorOfPoint filteredFingerPoints = new VectorOfPoint();
            if (filteredFarPoints.Size > 1)
            {
                VectorOfPoint fingerPoints = new VectorOfPoint();

                for (int i = 0; i < filteredStartPoints.Size; i++)
                {
                    VectorOfPoint closestPoints = findClosestOnX(filteredFarPoints, filteredStartPoints[i]);

                    if (isFinger(closestPoints[0], filteredStartPoints[i], closestPoints[1], LIMIT_ANGLE_INF, LIMIT_ANGLE_SUP, centerBoundingRectangle, boundingRectangle.Height * BOUNDING_RECT_FINGER_SIZE_SCALING))
                    {
                        fingerPoints.Push(new Point[] { filteredStartPoints[i] });
                    }
                }
                if (fingerPoints.Size > 0)
                {
                    while (fingerPoints.Size > 5)
                    {
                        //Remove extra fingers
                        //Convert to list and remove last item
                        List<Point> points = new List<Point>(fingerPoints.ToArray());
                        points.Remove(points.Last());
                        fingerPoints = new VectorOfPoint(points.ToArray());
                    }
                    for (int i = 0; i < fingerPoints.Size - 1; i++)
                    {

                    }
                    filteredFingerPoints = fingerPoints;
                }
            }

            Bgr colorRed = new Bgr(Color.Red);
            Bgr colorGreen = new Bgr(Color.Green);
            Bgr colorBlue = new Bgr(Color.Blue);
            Bgr colorYellow= new Bgr(Color.Yellow);
            Bgr colorPurple = new Bgr(Color.Purple);
            Bgr colorWhite = new Bgr(Color.White);
            CvInvoke.DrawContours(contoursImage, contours, 0, colorGreen.MCvScalar, DRAW_THICKNESS, LineType.AntiAlias);
            CvInvoke.Polylines(contoursImage, hullPoints, true, colorBlue.MCvScalar, DRAW_THICKNESS);
            CvInvoke.Rectangle(contoursImage, boundingRectangle, colorRed.MCvScalar, DRAW_THICKNESS);
            CvInvoke.Circle(contoursImage, centerBoundingRectangle, 5, colorYellow.MCvScalar, DRAW_THICKNESS);
            drawVectorPoints(contoursImage, filteredStartPoints, colorRed.MCvScalar, true, 3);
            drawVectorPoints(contoursImage, filteredFarPoints, colorWhite.MCvScalar, true, 3);
            drawVectorPoints(contoursImage, filteredFingerPoints, colorYellow.MCvScalar, false, 3);
            CvInvoke.PutText(contoursImage, filteredFingerPoints.Size.ToString(), centerBoundingRectangle, FontFace.HersheyComplex, 2, colorYellow.MCvScalar);

            //Debug, draw defects
            /*defectsData = (int[,,])defects.GetData();
            for (int i = 0; i < defectsData.Length / 4; i++)
            {
                Point start = contours[biggestContourIndex][defectsData[i, 0, 0]];
                Point far = contours[biggestContourIndex][defectsData[i, 0, 2]];
                Point end = contours[biggestContourIndex][defectsData[i, 0, 1]];

                CvInvoke.Polylines(contoursImage, new Point[] { start, far, end }, true, colorPurple.MCvScalar);
                CvInvoke.Circle(contoursImage, start, 5, colorYellow.MCvScalar);
                CvInvoke.Circle(contoursImage, far, 5, colorRed.MCvScalar, 10);
                CvInvoke.Circle(contoursImage, end, 5, colorPurple.MCvScalar);
            }*/

            return contoursImage;
        }
        private VectorOfPoint CompactOnNeighborhoodMedian(VectorOfPoint points, double maxNeighborDistance)
        {
            VectorOfPoint medianPoints = new VectorOfPoint();
            if (points.Size == 0)
            {
                return medianPoints;
            }
            if (maxNeighborDistance <= 0)
            {
                return medianPoints;
            }

            Point reference = points[0];
            Point median = points[0];

            for (int i = 0; i < points.Size; i++)
            {
                if (findPointsDistance(reference, points[i]) > maxNeighborDistance)
                {
                    medianPoints.Push(new Point[] { median });
                    reference = points[i];
                    median = points[i];
                } else
                {
                    median = new Point((points[i].X + median.X) / 2, (points[i].Y + median.Y) / 2);
                }
            }

            medianPoints.Push(new Point[] { median });
            return medianPoints;
        }
        private VectorOfPoint findClosestOnX(VectorOfPoint points, Point pivot)
        {
            VectorOfPoint result = new VectorOfPoint();

            if (points.Size == 0)
            {
                return result;
            }

            double distanceX1 = Double.MaxValue;
            double distance1 = Double.MaxValue;
            double distanceX2 = Double.MaxValue;
            double distance2 = Double.MaxValue;
            int indexFound = 0;

            for (int i = 0; i < points.Size; i++)
            {
                double distanceX = findPointsDistanceOnX(pivot, points[i]);
                double distance = findPointsDistance(pivot, points[i]);

                if (distanceX < distanceX1 && distanceX != 0 && distance <= distance1)
                {
                    distanceX1 = distanceX;
                    distance1 = distance;
                    indexFound = i;
                }
            }
            result.Push(new Point[] { points[indexFound] });

            for (int i = 0; i < points.Size; i++)
            {
                double distanceX = findPointsDistanceOnX(pivot, points[i]);
                double distance = findPointsDistance(pivot, points[i]);

                if (distanceX < distanceX2 && distanceX != 0 && distance <= distance2 && distanceX != distanceX1)
                {
                    distanceX2 = distanceX;
                    distance2 = distance;
                    indexFound = i;
                }
            }
            result.Push(new Point[] { points[indexFound] });

            return result;
        }
        private bool isFinger(Point a, Point b, Point c, double limitAngleInf, double limitAngleSup, Point palmCenter, double minDistanceFromPalm)
        {
            double angle = findAngle(a, b, c);
            if (angle > limitAngleSup || angle < limitAngleInf)
            {
                return false;
            }

            int deltaY1 = b.Y - a.Y;
            int deltaY2= b.Y - c.Y;
            if (deltaY1 > 0 && deltaY2 > 0)
            {
                return false;
            }

            int deltaY3 = palmCenter.Y - a.Y;
            int deltaY4 = palmCenter.Y - c.Y;
            if (deltaY3 < 0 && deltaY4 < 0)
            {
                return false;
            }

            double distanceFromPalm = findPointsDistance(b, palmCenter);
            if (distanceFromPalm < minDistanceFromPalm)
            {
                return false;
            }

            double distanceFromPalmFar1 = findPointsDistance(a, palmCenter);
            double distanceFromPalmFar2 = findPointsDistance(c, palmCenter);
            if (distanceFromPalmFar1 < distanceFromPalm / 4 || distanceFromPalmFar2 < minDistanceFromPalm / 4)
            {
                return false;
            }
            return true;
        }
        private void drawVectorPoints(Mat image, VectorOfPoint points, MCvScalar color, bool withNumbers, int thickness)
        {
            for (int i = 0; i < points.Size; i++)
            {
                CvInvoke.Circle(image, points[i], 5, color, thickness);
                if (withNumbers)
                {
                    CvInvoke.PutText(image, i.ToString(), points[i], FontFace.HersheyComplex , 2, color, thickness);
                }
            }
        }
        private double findPointsDistanceOnX(Point a, Point b)
        {
            double result = 0D;
            if (a.X > b.X)
            {
                result = a.X - b.X;
            } else
            {
                result = b.X - a.X;
            }
            return result;
        }
        private double findPointsDistance(Point a, Point b)
        {
            var xDiff = Math.Abs(a.X - b.X);
            var yDiff = Math.Abs(a.Y - b.Y);

            return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
        }
        private double findAngle(Point a, Point b, Point c)
        {
            double ab = findPointsDistance(a, b);
            double bc = findPointsDistance(b, c);
            double ac = findPointsDistance(a, c);
            return Math.Acos((ab * ab + bc * bc - ac * ac) / (2 * ab * bc)) * 180 / Math.PI;
        }
    }
}
