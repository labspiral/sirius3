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
            this.btnSetVelocity = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.btnActualVelocity = new System.Windows.Forms.Button();
            this.btnPositionDependent = new System.Windows.Forms.Button();
            this.btnSpotDistance = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnSetVelocity);
            this.flowLayoutPanel1.Controls.Add(this.btnActualVelocity);
            this.flowLayoutPanel1.Controls.Add(this.btnPositionDependent);
            this.flowLayoutPanel1.Controls.Add(this.btnSpotDistance);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(2254, 55);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnSetVelocity
            // 
            this.btnSetVelocity.Location = new System.Drawing.Point(3, 3);
            this.btnSetVelocity.Name = "btnSetVelocity";
            this.btnSetVelocity.Size = new System.Drawing.Size(118, 49);
            this.btnSetVelocity.TabIndex = 0;
            this.btnSetVelocity.Text = "Set velocity";
            this.btnSetVelocity.UseVisualStyleBackColor = true;
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
            this.siriusEditorControl1.Location = new System.Drawing.Point(0, 55);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Marker = null;
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.PowerMeter = null;
            this.siriusEditorControl1.Scanner = null;
            this.siriusEditorControl1.Size = new System.Drawing.Size(2254, 1583);
            this.siriusEditorControl1.TabIndex = 2;
            // 
            // btnActualVelocity
            // 
            this.btnActualVelocity.Location = new System.Drawing.Point(127, 3);
            this.btnActualVelocity.Name = "btnActualVelocity";
            this.btnActualVelocity.Size = new System.Drawing.Size(118, 49);
            this.btnActualVelocity.TabIndex = 1;
            this.btnActualVelocity.Text = "Actual velocity";
            this.btnActualVelocity.UseVisualStyleBackColor = true;
            // 
            // btnPositionDependent
            // 
            this.btnPositionDependent.Location = new System.Drawing.Point(251, 3);
            this.btnPositionDependent.Name = "btnPositionDependent";
            this.btnPositionDependent.Size = new System.Drawing.Size(185, 49);
            this.btnPositionDependent.TabIndex = 2;
            this.btnPositionDependent.Text = "Position dependent";
            this.btnPositionDependent.UseVisualStyleBackColor = true;
            // 
            // btnSpotDistance
            // 
            this.btnSpotDistance.Location = new System.Drawing.Point(442, 3);
            this.btnSpotDistance.Name = "btnSpotDistance";
            this.btnSpotDistance.Size = new System.Drawing.Size(132, 49);
            this.btnSpotDistance.TabIndex = 3;
            this.btnSpotDistance.Text = "Spot distance";
            this.btnSpotDistance.UseVisualStyleBackColor = true;
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
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnSetVelocity;
        private Button btnActualVelocity;
        private Button btnPositionDependent;
        private Button btnSpotDistance;
    }
}
