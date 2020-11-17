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
                /*new ToolStripMenuItem("Closed Hand", null, new EventHandler(LoadSample)), //Non-Functional samples
                new ToolStripMenuItem("1 Finger", null, new EventHandler(LoadSample)),
                new ToolStripMenuItem("2 Fingers", null, new EventHandler(LoadSample)),
                new ToolStripMenuItem("3 Fingers", null, new EventHandler(LoadSample)),*/
                new ToolStripMenuItem("4 Fingers", null, new EventHandler(LoadSample)),
                new ToolStripMenuItem("Open Hand", null, new EventHandler(LoadSample)),
            };
            loadSampleToolStripMenuItem.DropDownItems.AddRange(sampleImages);

            this.SkinDetector = new SkinDetector();
            this.FingerCounter = new FingerCounter();
        }
        /// <summary>
        /// Load image from file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Detect gesture from loaded image after calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetect_Click(object sender, EventArgs e)
        {
            Rectangle sampleArea1, sampleArea2;
            sampleArea1 = new Rectangle(this.pbxImage.Image.Width / 2, this.pbxImage.Image.Height / 2, 50, 50);
            sampleArea2 = new Rectangle(this.pbxImage.Image.Width / 2, this.pbxImage.Image.Height / 3, 50, 50);
            this.ImageMat = GestureRecognition.GetGestureFromImage(this.ImageMat, sampleArea1, sampleArea2);
            pbxImage.Image = this.ImageMat.ToBitmap();
        }
        /// <summary>
        /// Show calibration area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowSampleArea_Click(object sender, EventArgs e)
        {
            if (this.ImageMat != null)
            {
                Rectangle sampleArea1, sampleArea2;
                sampleArea1 = new Rectangle(this.pbxImage.Image.Width / 2, this.pbxImage.Image.Height / 2, 50, 50);
                sampleArea2 = new Rectangle(this.pbxImage.Image.Width / 2, this.pbxImage.Image.Height / 3, 50, 50);
                this.ImageMat = SkinDetector.DrawSkinColorSampler(this.ImageMat, sampleArea1, sampleArea2);
                pbxImage.Image = this.ImageMat.ToBitmap();
            }
        }
        /// <summary>
        /// Load sample image from resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadSample(object sender, EventArgs e)
        {
            Bitmap sample = null;
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            switch (toolStripMenuItem.Text)
            {
                /*case "Closed Hand":                           //Non-Functional samples
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
                     break;*/
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
                //Create OpenCV Mat from bitmap byte array
                BitmapData bitmapData = sample.LockBits(new Rectangle(0, 0, sample.Width, sample.Height), ImageLockMode.ReadOnly, sample.PixelFormat);

                Image<Bgr, byte> image = new Image<Bgr, byte>(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.Scan0);
                /*this.SkinDetector.Calibrated = false;
                this.btnDetect.Enabled = false;*/
                this.ImageMat = image.Mat;
                this.pbxImage.Image = (Bitmap)sample;
                sample.UnlockBits(bitmapData);
            }
        }
    }
}
