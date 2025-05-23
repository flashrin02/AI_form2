namespace AI_form2
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            textBox1 = new TextBox();
            listBox1 = new ListBox();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBoxSpinner = new PictureBox();
            progressBar1 = new ProgressBar();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSpinner).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.BurlyWood;
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Font = new Font("Britannic Bold", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(64, 149);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Inserisci i tuoi ingredienti";
            textBox1.Size = new Size(273, 33);
            textBox1.TabIndex = 0;
            // 
            // listBox1
            // 
            listBox1.BackColor = Color.MistyRose;
            listBox1.Font = new Font("Cooper Black", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 17;
            listBox1.Location = new Point(379, 40);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(382, 310);
            listBox1.TabIndex = 2;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(115, 136);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(169, 183);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 11;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_ClickAsync;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(115, 253);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(169, 66);
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(115, 166);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(169, 34);
            pictureBox3.TabIndex = 13;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(488, 306);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(169, 183);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 14;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // pictureBoxSpinner
            // 
            pictureBoxSpinner.Image = (Image)resources.GetObject("pictureBoxSpinner.Image");
            pictureBoxSpinner.Location = new Point(126, 265);
            pictureBoxSpinner.Name = "pictureBoxSpinner";
            pictureBoxSpinner.Size = new Size(137, 121);
            pictureBoxSpinner.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxSpinner.TabIndex = 19;
            pictureBoxSpinner.TabStop = false;
            pictureBoxSpinner.Visible = false;
            pictureBoxSpinner.Click += pictureBoxSpinner_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(147, 345);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(86, 29);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.TabIndex = 20;
            progressBar1.Visible = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Britannic Bold", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(97, 82);
            label1.Name = "label1";
            label1.Size = new Size(213, 26);
            label1.TabIndex = 21;
            label1.Text = "Cosa abbiamo oggi?";
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.MistyRose;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(pictureBoxSpinner);
            Controls.Add(progressBar1);
            Controls.Add(textBox1);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox1);
            Controls.Add(pictureBox2);
            Controls.Add(listBox1);
            Controls.Add(pictureBox4);
            MaximizeBox = false;
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSpinner).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private ListBox listBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBoxSpinner;
        private ProgressBar progressBar1;
        private Label label1;
    }
}