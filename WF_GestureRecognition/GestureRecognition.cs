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
        public static Gesture GetGestureFromImage(Mat img)
        {
            SkinDetector skinDetector = new SkinDetector();
            Gesture gesture = Gesture.None;
            //Get skin color sample
            skinDetector.GetSkinMask(img);
            //Remove background

            //Remove faces

            //Get hand

            //Detect gesture

            //Return gesture
            return gesture;
        }
    }
}
