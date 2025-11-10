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
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnStartEncoderSimulation = new System.Windows.Forms.Button();
            this.btnStopEncoderSimulation = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCreateEntities);
            this.flowLayoutPanel1.Controls.Add(this.btnStartEncoderSimulation);
            this.flowLayoutPanel1.Controls.Add(this.btnStopEncoderSimulation);
            this.flowLayoutPanel1.Controls.Add(this.txtSerialNo);
            this.flowLayoutPanel1.Controls.Add(this.btnStartStop);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(2254, 48);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnCreateEntities
            // 
            this.btnCreateEntities.Location = new System.Drawing.Point(3, 3);
            this.btnCreateEntities.Name = "btnCreateEntities";
            this.btnCreateEntities.Size = new System.Drawing.Size(155, 42);
            this.btnCreateEntities.TabIndex = 0;
            this.btnCreateEntities.Text = "Create Entities";
            this.btnCreateEntities.UseVisualStyleBackColor = true;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(724, 3);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(221, 42);
            this.btnStartStop.TabIndex = 3;
            this.btnStartStop.Text = "Start/Stop";
            this.btnStartStop.UseVisualStyleBackColor = true;
            // 
            // btnStartEncoderSimulation
            // 
            this.btnStartEncoderSimulation.Location = new System.Drawing.Point(164, 3);
            this.btnStartEncoderSimulation.Name = "btnStartEncoderSimulation";
            this.btnStartEncoderSimulation.Size = new System.Drawing.Size(221, 42);
            this.btnStartEncoderSimulation.TabIndex = 1;
            this.btnStartEncoderSimulation.Text = "Start Encoder (Simulation)";
            this.btnStartEncoderSimulation.UseVisualStyleBackColor = true;
            // 
            // btnStopEncoderSimulation
            // 
            this.btnStopEncoderSimulation.Location = new System.Drawing.Point(391, 3);
            this.btnStopEncoderSimulation.Name = "btnStopEncoderSimulation";
            this.btnStopEncoderSimulation.Size = new System.Drawing.Size(221, 42);
            this.btnStopEncoderSimulation.TabIndex = 2;
            this.btnStopEncoderSimulation.Text = "Stop Encoder (Simulation)";
            this.btnStopEncoderSimulation.UseVisualStyleBackColor = true;
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
            this.siriusEditorControl1.Location = new System.Drawing.Point(0, 48);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Marker = null;
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.PowerMeter = null;
            this.siriusEditorControl1.Scanner = null;
            this.siriusEditorControl1.Size = new System.Drawing.Size(2254, 1590);
            this.siriusEditorControl1.TabIndex = 2;
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerialNo.Location = new System.Drawing.Point(618, 3);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(100, 39);
            this.txtSerialNo.TabIndex = 4;
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
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnCreateEntities;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnStartEncoderSimulation;
        private Button btnStopEncoderSimulation;
        private Button btnStartStop;
        private TextBox txtSerialNo;
    }
}
