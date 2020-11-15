namespace WF_GestureRecognition
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbxImage = new System.Windows.Forms.PictureBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnDetect = new System.Windows.Forms.Button();
            this.btnShowSampleArea = new System.Windows.Forms.Button();
            this.btnSample = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSampleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pbxImage)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbxImage
            // 
            this.pbxImage.Location = new System.Drawing.Point(12, 27);
            this.pbxImage.Name = "pbxImage";
            this.pbxImage.Size = new System.Drawing.Size(400, 400);
            this.pbxImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxImage.TabIndex = 0;
            this.pbxImage.TabStop = false;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(13, 434);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(399, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load Image";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnDetect
            // 
            this.btnDetect.Location = new System.Drawing.Point(13, 522);
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.Size = new System.Drawing.Size(399, 23);
            this.btnDetect.TabIndex = 2;
            this.btnDetect.Text = "Detect Gesture";
            this.btnDetect.UseVisualStyleBackColor = true;
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // btnShowSampleArea
            // 
            this.btnShowSampleArea.Location = new System.Drawing.Point(12, 463);
            this.btnShowSampleArea.Name = "btnShowSampleArea";
            this.btnShowSampleArea.Size = new System.Drawing.Size(400, 23);
            this.btnShowSampleArea.TabIndex = 3;
            this.btnShowSampleArea.Text = "Show sample area (skin color)";
            this.btnShowSampleArea.UseVisualStyleBackColor = true;
            this.btnShowSampleArea.Click += new System.EventHandler(this.btnShowSampleArea_Click);
            // 
            // btnSample
            // 
            this.btnSample.Location = new System.Drawing.Point(12, 493);
            this.btnSample.Name = "btnSample";
            this.btnSample.Size = new System.Drawing.Size(400, 23);
            this.btnSample.TabIndex = 4;
            this.btnSample.Text = "Sample skin color";
            this.btnSample.UseVisualStyleBackColor = true;
            this.btnSample.Click += new System.EventHandler(this.btnSample_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(440, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSampleToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadSampleToolStripMenuItem
            // 
            this.loadSampleToolStripMenuItem.Name = "loadSampleToolStripMenuItem";
            this.loadSampleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.loadSampleToolStripMenuItem.Text = "Load sample image";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 579);
            this.Controls.Add(this.btnSample);
            this.Controls.Add(this.btnShowSampleArea);
            this.Controls.Add(this.btnDetect);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.pbxImage);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pbxImage)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbxImage;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnDetect;
        private System.Windows.Forms.Button btnShowSampleArea;
        private System.Windows.Forms.Button btnSample;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSampleToolStripMenuItem;
    }
}

