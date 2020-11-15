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
        OpenPalm
    }
    static class GestureRecognition
    {
        public static Mat GetGestureFromImage(Mat img, Rectangle sampleArea1, Rectangle sampleArea2)
        {
            SkinDetector skinDetector = new SkinDetector();
            FingerCounter fingerCounter = new FingerCounter();
            Gesture gesture = Gesture.None;

            if (img != null)
            {
                skinDetector.Calibrate(img, sampleArea1, sampleArea2);
                Mat skinMask = skinDetector.GetSkinMask(img);
                var newImage = fingerCounter.FindFingersCount(skinMask, img);
                img = newImage;
                switch (fingerCounter.NumberOfFingersRaised)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        gesture = Gesture.OpenPalm;
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
