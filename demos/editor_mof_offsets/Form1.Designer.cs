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
            this.btnCreateEntities = new System.Windows.Forms.Button();
            this.btnSimulateEncoder = new System.Windows.Forms.Button();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.nudCounts = new System.Windows.Forms.NumericUpDown();
            this.btnResetEncoder = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCounts)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCreateEntities);
            this.flowLayoutPanel1.Controls.Add(this.btnSimulateEncoder);
            this.flowLayoutPanel1.Controls.Add(this.btnStartStop);
            this.flowLayoutPanel1.Controls.Add(this.nudCounts);
            this.flowLayoutPanel1.Controls.Add(this.btnResetEncoder);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1000, 31);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnCreateEntities
            // 
            this.btnCreateEntities.Location = new System.Drawing.Point(3, 3);
            this.btnCreateEntities.Name = "btnCreateEntities";
            this.btnCreateEntities.Size = new System.Drawing.Size(98, 25);
            this.btnCreateEntities.TabIndex = 0;
            this.btnCreateEntities.Text = "Create Entities";
            this.btnCreateEntities.UseVisualStyleBackColor = true;
            // 
            // btnSimulateEncoder
            // 
            this.btnSimulateEncoder.Location = new System.Drawing.Point(107, 3);
            this.btnSimulateEncoder.Name = "btnSimulateEncoder";
            this.btnSimulateEncoder.Size = new System.Drawing.Size(164, 25);
            this.btnSimulateEncoder.TabIndex = 5;
            this.btnSimulateEncoder.Text = "Simulate Encoder";
            this.btnSimulateEncoder.UseVisualStyleBackColor = true;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(277, 3);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(164, 25);
            this.btnStartStop.TabIndex = 3;
            this.btnStartStop.Text = "Start/Stop";
            this.btnStartStop.UseVisualStyleBackColor = true;
            // 
            // nudCounts
            // 
            this.nudCounts.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCounts.Location = new System.Drawing.Point(447, 3);
            this.nudCounts.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudCounts.Name = "nudCounts";
            this.nudCounts.ReadOnly = true;
            this.nudCounts.Size = new System.Drawing.Size(103, 29);
            this.nudCounts.TabIndex = 4;
            this.nudCounts.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnResetEncoder
            // 
            this.btnResetEncoder.Location = new System.Drawing.Point(556, 3);
            this.btnResetEncoder.Name = "btnResetEncoder";
            this.btnResetEncoder.Size = new System.Drawing.Size(164, 25);
            this.btnResetEncoder.TabIndex = 6;
            this.btnResetEncoder.Text = "Reset Encoder";
            this.btnResetEncoder.UseVisualStyleBackColor = true;
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
            this.siriusEditorControl1.Location = new System.Drawing.Point(0, 31);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Marker = null;
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.PowerMeter = null;
            this.siriusEditorControl1.Scanner = null;
            this.siriusEditorControl1.Size = new System.Drawing.Size(1000, 769);
            this.siriusEditorControl1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 800);
            this.Controls.Add(this.siriusEditorControl1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Sirius3 Demo - (c)SpiralLAB";
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudCounts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnCreateEntities;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnStartStop;
        private NumericUpDown nudCounts;
        private Button btnSimulateEncoder;
        private Button btnResetEncoder;
    }
}
