namespace Ploter
{
    partial class Form1
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SmoothingCheckBox = new System.Windows.Forms.CheckBox();
            this.OneByOneCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.OneByOneCheckBox);
            this.groupBox1.Controls.Add(this.SmoothingCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 231);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // SmoothingCheckBox
            // 
            this.SmoothingCheckBox.AutoSize = true;
            this.SmoothingCheckBox.Location = new System.Drawing.Point(7, 22);
            this.SmoothingCheckBox.Name = "SmoothingCheckBox";
            this.SmoothingCheckBox.Size = new System.Drawing.Size(157, 21);
            this.SmoothingCheckBox.TabIndex = 0;
            this.SmoothingCheckBox.Text = "Smoothing(BUGGY)";
            this.SmoothingCheckBox.UseVisualStyleBackColor = true;
            // 
            // OneByOneCheckBox
            // 
            this.OneByOneCheckBox.AutoSize = true;
            this.OneByOneCheckBox.Checked = true;
            this.OneByOneCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.OneByOneCheckBox.Location = new System.Drawing.Point(7, 50);
            this.OneByOneCheckBox.Name = "OneByOneCheckBox";
            this.OneByOneCheckBox.Size = new System.Drawing.Size(135, 21);
            this.OneByOneCheckBox.TabIndex = 1;
            this.OneByOneCheckBox.Text = "OneByOneMode";
            this.OneByOneCheckBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 384);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing_1);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.ClientSizeChanged += new System.EventHandler(this.Form1_ResizeEnd);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox SmoothingCheckBox;
        private System.Windows.Forms.CheckBox OneByOneCheckBox;
    }
}

