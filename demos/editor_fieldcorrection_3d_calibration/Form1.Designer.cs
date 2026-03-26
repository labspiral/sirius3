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
            this.btnGridCloud = new System.Windows.Forms.Button();
            this.btnFieldCorrection3D = new System.Windows.Forms.Button();
            this.btnRevertFieldCorrection = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnLoad3DModel);
            this.flowLayoutPanel1.Controls.Add(this.btnGridCloud);
            this.flowLayoutPanel1.Controls.Add(this.btnFieldCorrection3D);
            this.flowLayoutPanel1.Controls.Add(this.btnRevertFieldCorrection);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1264, 38);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnLoad3DModel
            // 
            this.btnLoad3DModel.AutoSize = true;
            this.btnLoad3DModel.Location = new System.Drawing.Point(3, 3);
            this.btnLoad3DModel.Name = "btnLoad3DModel";
            this.btnLoad3DModel.Size = new System.Drawing.Size(120, 32);
            this.btnLoad3DModel.TabIndex = 0;
            this.btnLoad3DModel.Text = "Load 3D Model";
            this.btnLoad3DModel.UseVisualStyleBackColor = true;
            // 
            // btnGridCloud
            // 
            this.btnGridCloud.AutoSize = true;
            this.btnGridCloud.Location = new System.Drawing.Point(129, 3);
            this.btnGridCloud.Name = "btnGridCloud";
            this.btnGridCloud.Size = new System.Drawing.Size(120, 32);
            this.btnGridCloud.TabIndex = 1;
            this.btnGridCloud.Text = "Extract Grid Cloud";
            this.btnGridCloud.UseVisualStyleBackColor = true;
            // 
            // btnFieldCorrection3D
            // 
            this.btnFieldCorrection3D.AutoSize = true;
            this.btnFieldCorrection3D.Location = new System.Drawing.Point(255, 3);
            this.btnFieldCorrection3D.Name = "btnFieldCorrection3D";
            this.btnFieldCorrection3D.Size = new System.Drawing.Size(220, 32);
            this.btnFieldCorrection3D.TabIndex = 2;
            this.btnFieldCorrection3D.Text = "Convert Field Correction (3D) and Apply";
            this.btnFieldCorrection3D.UseVisualStyleBackColor = true;
            // 
            // btnRevertFieldCorrection
            // 
            this.btnRevertFieldCorrection.AutoSize = true;
            this.btnRevertFieldCorrection.Location = new System.Drawing.Point(481, 3);
            this.btnRevertFieldCorrection.Name = "btnRevertFieldCorrection";
            this.btnRevertFieldCorrection.Size = new System.Drawing.Size(134, 32);
            this.btnRevertFieldCorrection.TabIndex = 3;
            this.btnRevertFieldCorrection.Text = "Revert Field Correction";
            this.btnRevertFieldCorrection.UseVisualStyleBackColor = true;
            // 
            // siriusEditorControl1
            // 
            this.siriusEditorControl1.AliasName = "NoName";
            this.siriusEditorControl1.AutoSize = true;
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
            this.siriusEditorControl1.Size = new System.Drawing.Size(1264, 823);
            this.siriusEditorControl1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 861);
            this.Controls.Add(this.siriusEditorControl1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Sirius3 Demo - (c)SpiralLAB";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnLoad3DModel;
        private Button btnGridCloud;
        private Button btnFieldCorrection3D;
        private Button btnRevertFieldCorrection;
    }
}
