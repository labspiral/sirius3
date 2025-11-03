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
            this.btnLoad3DModel = new System.Windows.Forms.Button();
            this.btnSliceContours = new System.Windows.Forms.Button();
            this.btnHatchGenerate = new System.Windows.Forms.Button();
            this.btnSimulationStart = new System.Windows.Forms.Button();
            this.btnSimulationStop = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.nudMin = new System.Windows.Forms.NumericUpDown();
            this.nudSlice = new System.Windows.Forms.NumericUpDown();
            this.nudMax = new System.Windows.Forms.NumericUpDown();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSlice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnLoad3DModel);
            this.flowLayoutPanel1.Controls.Add(this.nudMin);
            this.flowLayoutPanel1.Controls.Add(this.nudSlice);
            this.flowLayoutPanel1.Controls.Add(this.nudMax);
            this.flowLayoutPanel1.Controls.Add(this.btnSliceContours);
            this.flowLayoutPanel1.Controls.Add(this.btnHatchGenerate);
            this.flowLayoutPanel1.Controls.Add(this.btnSimulationStart);
            this.flowLayoutPanel1.Controls.Add(this.btnSimulationStop);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(2254, 55);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnLoad3DModel
            // 
            this.btnLoad3DModel.Location = new System.Drawing.Point(3, 3);
            this.btnLoad3DModel.Name = "btnLoad3DModel";
            this.btnLoad3DModel.Size = new System.Drawing.Size(147, 49);
            this.btnLoad3DModel.TabIndex = 0;
            this.btnLoad3DModel.Text = "Load 3D Model";
            this.btnLoad3DModel.UseVisualStyleBackColor = true;
            // 
            // btnSliceContours
            // 
            this.btnSliceContours.Location = new System.Drawing.Point(624, 3);
            this.btnSliceContours.Name = "btnSliceContours";
            this.btnSliceContours.Size = new System.Drawing.Size(165, 49);
            this.btnSliceContours.TabIndex = 2;
            this.btnSliceContours.Text = "Slice (Contours)";
            this.btnSliceContours.UseVisualStyleBackColor = true;
            // 
            // btnHatchGenerate
            // 
            this.btnHatchGenerate.Location = new System.Drawing.Point(795, 3);
            this.btnHatchGenerate.Name = "btnHatchGenerate";
            this.btnHatchGenerate.Size = new System.Drawing.Size(185, 49);
            this.btnHatchGenerate.TabIndex = 3;
            this.btnHatchGenerate.Text = "Add Hatch";
            this.btnHatchGenerate.UseVisualStyleBackColor = true;
            // 
            // btnSimulationStart
            // 
            this.btnSimulationStart.Location = new System.Drawing.Point(986, 3);
            this.btnSimulationStart.Name = "btnSimulationStart";
            this.btnSimulationStart.Size = new System.Drawing.Size(147, 49);
            this.btnSimulationStart.TabIndex = 4;
            this.btnSimulationStart.Text = "Simulation Start";
            this.btnSimulationStart.UseVisualStyleBackColor = true;
            // 
            // btnSimulationStop
            // 
            this.btnSimulationStop.Location = new System.Drawing.Point(1139, 3);
            this.btnSimulationStop.Name = "btnSimulationStop";
            this.btnSimulationStop.Size = new System.Drawing.Size(147, 49);
            this.btnSimulationStop.TabIndex = 5;
            this.btnSimulationStop.Text = "Simulation Stop";
            this.btnSimulationStop.UseVisualStyleBackColor = true;
            // 
            // siriusEditorControl1
            // 
            this.siriusEditorControl1.AliasName = "NoName";
            this.siriusEditorControl1.BackColor = System.Drawing.SystemColors.Control;
            this.siriusEditorControl1.DIExt1 = null;
            this.siriusEditorControl1.DILaserPort = null;
            this.siriusEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusEditorControl1.DOExt1 = null;
            this.siriusEditorControl1.DOExt2 = null;
            this.siriusEditorControl1.DOLaserPort = null;
            this.siriusEditorControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorControl1.Laser = null;
            this.siriusEditorControl1.Location = new System.Drawing.Point(0, 55);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Marker = null;
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.PowerMeter = null;
            this.siriusEditorControl1.Scanner = null;
            this.siriusEditorControl1.Size = new System.Drawing.Size(2254, 1583);
            this.siriusEditorControl1.TabIndex = 2;
            // 
            // nudMin
            // 
            this.nudMin.DecimalPlaces = 3;
            this.nudMin.Enabled = false;
            this.nudMin.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudMin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudMin.Location = new System.Drawing.Point(156, 3);
            this.nudMin.Name = "nudMin";
            this.nudMin.ReadOnly = true;
            this.nudMin.Size = new System.Drawing.Size(150, 45);
            this.nudMin.TabIndex = 6;
            // 
            // nudSlice
            // 
            this.nudSlice.DecimalPlaces = 3;
            this.nudSlice.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudSlice.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudSlice.Location = new System.Drawing.Point(312, 3);
            this.nudSlice.Name = "nudSlice";
            this.nudSlice.Size = new System.Drawing.Size(150, 45);
            this.nudSlice.TabIndex = 7;
            // 
            // nudMax
            // 
            this.nudMax.DecimalPlaces = 3;
            this.nudMax.Enabled = false;
            this.nudMax.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudMax.Location = new System.Drawing.Point(468, 3);
            this.nudMax.Name = "nudMax";
            this.nudMax.ReadOnly = true;
            this.nudMax.Size = new System.Drawing.Size(150, 45);
            this.nudMax.TabIndex = 8;
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
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSlice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnLoad3DModel;
        private Button btnSliceContours;
        private Button btnHatchGenerate;
        private Button btnSimulationStart;
        private Button btnSimulationStop;
        private NumericUpDown nudMin;
        private NumericUpDown nudSlice;
        private NumericUpDown nudMax;
    }
}
