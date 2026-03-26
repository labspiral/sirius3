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
            this.btnPowerMap = new System.Windows.Forms.Button();
            this.btnPowerVerify = new System.Windows.Forms.Button();
            this.btnPowerCompensate = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnPowerMap);
            this.flowLayoutPanel1.Controls.Add(this.btnPowerVerify);
            this.flowLayoutPanel1.Controls.Add(this.btnPowerCompensate);
            this.flowLayoutPanel1.Controls.Add(this.btnStop);
            this.flowLayoutPanel1.Controls.Add(this.btnReset);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1264, 38);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnPowerMap
            // 
            this.btnPowerMap.AutoSize = true;
            this.btnPowerMap.Location = new System.Drawing.Point(3, 3);
            this.btnPowerMap.Name = "btnPowerMap";
            this.btnPowerMap.Size = new System.Drawing.Size(120, 32);
            this.btnPowerMap.TabIndex = 0;
            this.btnPowerMap.Text = "Power mapping";
            this.btnPowerMap.UseVisualStyleBackColor = true;
            // 
            // btnPowerVerify
            // 
            this.btnPowerVerify.AutoSize = true;
            this.btnPowerVerify.Location = new System.Drawing.Point(129, 3);
            this.btnPowerVerify.Name = "btnPowerVerify";
            this.btnPowerVerify.Size = new System.Drawing.Size(120, 32);
            this.btnPowerVerify.TabIndex = 1;
            this.btnPowerVerify.Text = "Power verifying";
            this.btnPowerVerify.UseVisualStyleBackColor = true;
            // 
            // btnPowerCompensate
            // 
            this.btnPowerCompensate.AutoSize = true;
            this.btnPowerCompensate.Location = new System.Drawing.Point(255, 3);
            this.btnPowerCompensate.Name = "btnPowerCompensate";
            this.btnPowerCompensate.Size = new System.Drawing.Size(125, 32);
            this.btnPowerCompensate.TabIndex = 2;
            this.btnPowerCompensate.Text = "Power compensating";
            this.btnPowerCompensate.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.AutoSize = true;
            this.btnStop.Location = new System.Drawing.Point(386, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(120, 32);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.AutoSize = true;
            this.btnReset.Location = new System.Drawing.Point(512, 3);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 32);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
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
        private Button btnPowerMap;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnPowerVerify;
        private Button btnPowerCompensate;
        private Button btnStop;
        private Button btnReset;
    }
}
