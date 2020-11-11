using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;

namespace WF_GestureRecognition
{
    public partial class Form1 : Form
    {
        public Mat ImageMat { get; set; }
        private SkinDetector SkinDetector { get; set; }
        private FingerCounter FingerCounter { get; set; }
        public Form1()
        {
            InitializeComponent();
            this.SkinDetector = new SkinDetector();
            this.FingerCounter = new FingerCounter();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    this.ImageMat = new Mat(filePath);
                    pbxImage.Image = this.ImageMat.ToBitmap();
                }
            }
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            //Image<Bgr, byte> imageCV = new Image<Bgr, byte>(bmp); //Image Class from Emgu.CV
            //img = imageCV.Mat; //This is your Image converted to Mat
            if (this.SkinDetector.Calibrated)
            {
                Gesture g = GestureRecognition.GetGestureFromImage(this.ImageMat);
                Console.WriteLine(g.ToString());
            } else
            {
                Console.WriteLine("Skin detector not calibrated");
            }
        }

        private void btnShowSampleArea_Click(object sender, EventArgs e)
        {
            if (this.ImageMat != null)
            {
                Rectangle sampleArea1, sampleArea2;
                sampleArea1 = new Rectangle(this.ImageMat.Width / 2, this.ImageMat.Height / 2, 150, 150);
                sampleArea2 = new Rectangle(this.ImageMat.Width / 2, this.ImageMat.Height / 3, 150, 150);
                this.ImageMat = SkinDetector.DrawSkinColorSampler(this.ImageMat, sampleArea1, sampleArea2);
                pbxImage.Image = this.ImageMat.ToBitmap();
            }
        }

        private void btnSample_Click(object sender, EventArgs e)
        {
            if (this.ImageMat != null)
            {
                Rectangle sampleArea1, sampleArea2;
                sampleArea1 = new Rectangle(this.ImageMat.Width / 6, this.ImageMat.Height / 2, 50, 50);
                sampleArea2 = new Rectangle(this.ImageMat.Width / 3, this.ImageMat.Height / 2, 50, 50);
                this.SkinDetector.Calibrate(this.ImageMat, sampleArea1, sampleArea2);
                Mat skinMask = this.SkinDetector.GetSkinMask(this.ImageMat);
                this.ImageMat = FingerCounter.FindFingersCount(skinMask, this.ImageMat);
                this.pbxImage.Image = this.ImageMat.ToBitmap();
            }
        }
    }
}
