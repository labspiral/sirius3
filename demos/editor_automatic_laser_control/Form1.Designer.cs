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
            this.btnDefinedVector = new System.Windows.Forms.Button();
            this.btnSetVelocity = new System.Windows.Forms.Button();
            this.btnActualVelocity = new System.Windows.Forms.Button();
            this.btnSpotDistanceControl = new System.Windows.Forms.Button();
            this.btnPositionDependent = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnDefinedVector);
            this.flowLayoutPanel1.Controls.Add(this.btnSetVelocity);
            this.flowLayoutPanel1.Controls.Add(this.btnActualVelocity);
            this.flowLayoutPanel1.Controls.Add(this.btnSpotDistanceControl);
            this.flowLayoutPanel1.Controls.Add(this.btnPositionDependent);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1000, 38);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnDefinedVector
            // 
            this.btnDefinedVector.Location = new System.Drawing.Point(3, 3);
            this.btnDefinedVector.Name = "btnDefinedVector";
            this.btnDefinedVector.Size = new System.Drawing.Size(166, 30);
            this.btnDefinedVector.TabIndex = 4;
            this.btnDefinedVector.Text = "Defined Vector";
            this.btnDefinedVector.UseVisualStyleBackColor = true;
            // 
            // btnSetVelocity
            // 
            this.btnSetVelocity.Location = new System.Drawing.Point(175, 3);
            this.btnSetVelocity.Name = "btnSetVelocity";
            this.btnSetVelocity.Size = new System.Drawing.Size(285, 30);
            this.btnSetVelocity.TabIndex = 0;
            this.btnSetVelocity.Text = "Speed Dependent: Set velocity";
            this.btnSetVelocity.UseVisualStyleBackColor = true;
            // 
            // btnActualVelocity
            // 
            this.btnActualVelocity.Location = new System.Drawing.Point(466, 3);
            this.btnActualVelocity.Name = "btnActualVelocity";
            this.btnActualVelocity.Size = new System.Drawing.Size(280, 30);
            this.btnActualVelocity.TabIndex = 1;
            this.btnActualVelocity.Text = "Speed Dependent: Actual velocity";
            this.btnActualVelocity.UseVisualStyleBackColor = true;
            // 
            // btnSpotDistanceControl
            // 
            this.btnSpotDistanceControl.Location = new System.Drawing.Point(3, 39);
            this.btnSpotDistanceControl.Name = "btnSpotDistanceControl";
            this.btnSpotDistanceControl.Size = new System.Drawing.Size(362, 49);
            this.btnSpotDistanceControl.TabIndex = 3;
            this.btnSpotDistanceControl.Text = "Speed Dependent: SDC (Spot Distance Control)";
            this.btnSpotDistanceControl.UseVisualStyleBackColor = true;
            // 
            // btnPositionDependent
            // 
            this.btnPositionDependent.Location = new System.Drawing.Point(371, 39);
            this.btnPositionDependent.Name = "btnPositionDependent";
            this.btnPositionDependent.Size = new System.Drawing.Size(185, 49);
            this.btnPositionDependent.TabIndex = 2;
            this.btnPositionDependent.Text = "Position Dependent";
            this.btnPositionDependent.UseVisualStyleBackColor = true;
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
            this.siriusEditorControl1.Location = new System.Drawing.Point(0, 38);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Marker = null;
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.PowerMeter = null;
            this.siriusEditorControl1.Scanner = null;
            this.siriusEditorControl1.Size = new System.Drawing.Size(1000, 762);
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
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnSetVelocity;
        private Button btnActualVelocity;
        private Button btnPositionDependent;
        private Button btnSpotDistanceControl;
        private Button btnDefinedVector;
    }
}
