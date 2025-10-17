namespace Demos
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnMark = new System.Windows.Forms.Button();
            this.btnMarkSw1 = new System.Windows.Forms.Button();
            this.btnMarkSw2 = new System.Windows.Forms.Button();
            this.btnMarkSw3 = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.btnMarkSw4 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnMark);
            this.flowLayoutPanel1.Controls.Add(this.btnMarkSw1);
            this.flowLayoutPanel1.Controls.Add(this.btnMarkSw2);
            this.flowLayoutPanel1.Controls.Add(this.btnMarkSw3);
            this.flowLayoutPanel1.Controls.Add(this.btnMarkSw4);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(2254, 48);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnMark
            // 
            this.btnMark.Location = new System.Drawing.Point(3, 3);
            this.btnMark.Name = "btnMark";
            this.btnMark.Size = new System.Drawing.Size(258, 42);
            this.btnMark.TabIndex = 0;
            this.btnMark.Text = "Measurement (No skywriting)";
            this.btnMark.UseVisualStyleBackColor = true;
            // 
            // btnMarkSw1
            // 
            this.btnMarkSw1.Location = new System.Drawing.Point(267, 3);
            this.btnMarkSw1.Name = "btnMarkSw1";
            this.btnMarkSw1.Size = new System.Drawing.Size(230, 42);
            this.btnMarkSw1.TabIndex = 1;
            this.btnMarkSw1.Text = "Measurement (Skywriting 1)";
            this.btnMarkSw1.UseVisualStyleBackColor = true;
            // 
            // btnMarkSw2
            // 
            this.btnMarkSw2.Location = new System.Drawing.Point(503, 3);
            this.btnMarkSw2.Name = "btnMarkSw2";
            this.btnMarkSw2.Size = new System.Drawing.Size(230, 42);
            this.btnMarkSw2.TabIndex = 2;
            this.btnMarkSw2.Text = "Measurement (Skywriting 2)";
            this.btnMarkSw2.UseVisualStyleBackColor = true;
            // 
            // btnMarkSw3
            // 
            this.btnMarkSw3.Location = new System.Drawing.Point(739, 3);
            this.btnMarkSw3.Name = "btnMarkSw3";
            this.btnMarkSw3.Size = new System.Drawing.Size(230, 42);
            this.btnMarkSw3.TabIndex = 3;
            this.btnMarkSw3.Text = "Measurement (Skywriting 3)";
            this.btnMarkSw3.UseVisualStyleBackColor = true;
            // 
            // siriusEditorControl1
            // 
            this.siriusEditorControl1.BackColor = System.Drawing.SystemColors.Control;
            this.siriusEditorControl1.DIExt1 = null;
            this.siriusEditorControl1.DILaserPort = null;
            this.siriusEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusEditorControl1.DOExt1 = null;
            this.siriusEditorControl1.DOExt2 = null;
            this.siriusEditorControl1.DOLaserPort = null;
            this.siriusEditorControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorControl1.Laser = null;
            this.siriusEditorControl1.Location = new System.Drawing.Point(0, 48);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Marker = null;
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.PowerMeter = null;
            this.siriusEditorControl1.Scanner = null;
            this.siriusEditorControl1.Size = new System.Drawing.Size(2254, 1590);
            this.siriusEditorControl1.TabIndex = 2;
            // 
            // btnMarkSw4
            // 
            this.btnMarkSw4.Location = new System.Drawing.Point(975, 3);
            this.btnMarkSw4.Name = "btnMarkSw4";
            this.btnMarkSw4.Size = new System.Drawing.Size(230, 42);
            this.btnMarkSw4.TabIndex = 4;
            this.btnMarkSw4.Text = "Measurement (Skywriting 3)";
            this.btnMarkSw4.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2254, 1638);
            this.Controls.Add(this.siriusEditorControl1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Sirius3 Demo - (c)SpiralLAB";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnMark;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnMarkSw1;
        private Button btnMarkSw2;
        private Button btnMarkSw3;
        private Button btnMarkSw4;
    }
}
