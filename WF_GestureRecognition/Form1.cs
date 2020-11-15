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
using System.Drawing.Imaging;

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

            ToolStripItem[] sampleImages = new ToolStripItem[] {
                new ToolStripMenuItem("Closed Hand", null, new EventHandler(LoadSample)),
                new ToolStripMenuItem("1 Finger", null, new EventHandler(LoadSample)),
                new ToolStripMenuItem("2 Fingers", null, new EventHandler(LoadSample)),
                new ToolStripMenuItem("3 Fingers", null, new EventHandler(LoadSample)),
                new ToolStripMenuItem("4 Fingers", null, new EventHandler(LoadSample)),
                new ToolStripMenuItem("Open Hand", null, new EventHandler(LoadSample)),
            };
            loadSampleToolStripMenuItem.DropDownItems.AddRange(sampleImages);

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
            }
            else
            {
                Console.WriteLine("Skin detector not calibrated");
            }
        }

        private void btnShowSampleArea_Click(object sender, EventArgs e)
        {
            if (this.ImageMat != null)
            {
                Rectangle sampleArea1, sampleArea2;
                sampleArea1 = new Rectangle(this.pbxImage.Width / 2, this.pbxImage.Height / 2, 50, 50);
                sampleArea2 = new Rectangle(this.pbxImage.Width / 3, this.pbxImage.Height / 2, 50, 50);
                this.ImageMat = SkinDetector.DrawSkinColorSampler(this.ImageMat, sampleArea1, sampleArea2);
                pbxImage.Image = this.ImageMat.ToBitmap();
            }
        }

        private void btnSample_Click(object sender, EventArgs e)
        {
            if (this.ImageMat != null)
            {
                Rectangle sampleArea1, sampleArea2;
                /*sampleArea1 = new Rectangle(this.ImageMat.Width / 6, this.ImageMat.Height / 2, 50, 50);
                sampleArea2 = new Rectangle(this.ImageMat.Width / 3, this.ImageMat.Height / 2, 50, 50);*/
                sampleArea1 = new Rectangle(this.pbxImage.Width / 2, this.pbxImage.Height / 2, 50, 50);
                sampleArea2 = new Rectangle(this.pbxImage.Width / 3, this.pbxImage.Height / 2, 50, 50);
                this.SkinDetector.Calibrate(this.ImageMat, sampleArea1, sampleArea2);
                Mat skinMask = this.SkinDetector.GetSkinMask(this.ImageMat);
                var newImage = FingerCounter.FindFingersCount(skinMask, this.ImageMat);
                //var newImage = skinMask;
                this.pbxImage.Image = newImage.ToBitmap();
            }
        }
        private void LoadSample(object sender, EventArgs e)
        {
            Bitmap sample = null;
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            switch (toolStripMenuItem.Text)
            {
                case "Closed Hand":
                    sample = Properties.Resources.hand_closed;
                    break;
                case "1 Finger":
                    sample = Properties.Resources._1_finger;
                    break;
                case "2 Fingers":
                    sample = Properties.Resources._2_fingers;
                    break;
                case "3 Fingers":
                    sample = Properties.Resources._3_fingers;
                    break;
                case "4 Fingers":
                    sample = Properties.Resources._4_fingers;
                    break;
                case "Open Hand":
                    sample = Properties.Resources.hand_open;
                    break;
                default:
                    break;
            }

            if (sample != null)
            {
                /*ImageConverter imageConverter = new ImageConverter();
                Image<Bgr, byte> image = new Image<Bgr, byte>(sample.Width, sample.Height);
                image.Bytes = (byte[])imageConverter.ConvertTo(sample, typeof(byte[]));
                this.ImageMat = image.Mat;*/
                BitmapData bitmapData = sample.LockBits(new Rectangle(0, 0, sample.Width, sample.Height), ImageLockMode.ReadOnly, sample.PixelFormat);

                Image<Bgr, byte> image = new Image<Bgr, byte>(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.Scan0);
                this.ImageMat = image.Mat;
                this.pbxImage.Image = (Bitmap)sample;
                sample.UnlockBits(bitmapData);
            }
        }
    }
}
