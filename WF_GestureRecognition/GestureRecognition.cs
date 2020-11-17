using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using System.Drawing;

namespace WF_GestureRecognition
{
    public enum Gesture
    {
        None,
        Pointing,
        Sign_2,
        Sign_3,
        Sign_4,
        OpenHand
    }
    static class GestureRecognition
    {
        /// <summary>
        /// Calibrate skin mask and detect gesture
        /// </summary>
        /// <param name="img">Input Image</param>
        /// <param name="sampleArea1">Skin mask sample area 1</param>
        /// <param name="sampleArea2">Skin mask sample area 1</param>
        /// <returns>Image with debug information and detected gesture</returns>
        public static Mat GetGestureFromImage(Mat img, Rectangle sampleArea1, Rectangle sampleArea2)
        {
            SkinDetector skinDetector = new SkinDetector();
            FingerCounter fingerCounter = new FingerCounter();
            Gesture gesture = Gesture.None;

            if (img != null)
            {
                skinDetector.Calibrate(img, sampleArea1, sampleArea2);
                Mat skinMask = skinDetector.GetSkinMask(img);
                var newImage = fingerCounter.FindFingersCount(skinMask);
                img = newImage;
                switch (fingerCounter.NumberOfFingersRaised)
                {
                    case 0:
                        gesture = Gesture.None;
                        break;
                    case 1:
                        gesture = Gesture.Pointing;
                        break;
                    case 2:
                        gesture = Gesture.Sign_2;
                        break;
                    case 3:
                        gesture = Gesture.Sign_3;
                        break;
                    case 4:
                        gesture = Gesture.Sign_4;
                        break;
                    case 5:
                        gesture = Gesture.OpenHand;
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine(gesture.ToString());
            CvInvoke.PutText(img, gesture.ToString(), new Point(50, 50), FontFace.HersheyComplex, 1, new Bgr(255, 255, 255).MCvScalar, 3);
            return img;
        }
    }
}
