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
            this.btnCreateBarcode = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.btnMark4Offset = new System.Windows.Forms.Button();
            this.btnMark4OffsetWithChangeData = new System.Windows.Forms.Button();
            this.btnMark4OffsetWithRotate = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCreateBarcode);
            this.flowLayoutPanel1.Controls.Add(this.btnMark4Offset);
            this.flowLayoutPanel1.Controls.Add(this.btnMark4OffsetWithRotate);
            this.flowLayoutPanel1.Controls.Add(this.btnMark4OffsetWithChangeData);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(2254, 48);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnCreateBarcode
            // 
            this.btnCreateBarcode.Location = new System.Drawing.Point(3, 3);
            this.btnCreateBarcode.Name = "btnCreateBarcode";
            this.btnCreateBarcode.Size = new System.Drawing.Size(109, 42);
            this.btnCreateBarcode.TabIndex = 0;
            this.btnCreateBarcode.Text = "Create";
            this.btnCreateBarcode.UseVisualStyleBackColor = true;
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
            // btnMark4Offset
            // 
            this.btnMark4Offset.Location = new System.Drawing.Point(118, 3);
            this.btnMark4Offset.Name = "btnMark4Offset";
            this.btnMark4Offset.Size = new System.Drawing.Size(146, 42);
            this.btnMark4Offset.TabIndex = 1;
            this.btnMark4Offset.Text = "Mark (4 Offsets)";
            this.btnMark4Offset.UseVisualStyleBackColor = true;
            // 
            // btnMark4OffsetWithChangeData
            // 
            this.btnMark4OffsetWithChangeData.Location = new System.Drawing.Point(501, 3);
            this.btnMark4OffsetWithChangeData.Name = "btnMark4OffsetWithChangeData";
            this.btnMark4OffsetWithChangeData.Size = new System.Drawing.Size(276, 42);
            this.btnMark4OffsetWithChangeData.TabIndex = 2;
            this.btnMark4OffsetWithChangeData.Text = "Mark (4 Offsets + Change Data)";
            this.btnMark4OffsetWithChangeData.UseVisualStyleBackColor = true;
            // 
            // btnMark4OffsetWithRotate
            // 
            this.btnMark4OffsetWithRotate.Location = new System.Drawing.Point(270, 3);
            this.btnMark4OffsetWithRotate.Name = "btnMark4OffsetWithRotate";
            this.btnMark4OffsetWithRotate.Size = new System.Drawing.Size(225, 42);
            this.btnMark4OffsetWithRotate.TabIndex = 3;
            this.btnMark4OffsetWithRotate.Text = "Mark (4 Offsets + Rotate)";
            this.btnMark4OffsetWithRotate.UseVisualStyleBackColor = true;
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
        private Button btnCreateBarcode;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnMark4Offset;
        private Button btnMark4OffsetWithChangeData;
        private Button btnMark4OffsetWithRotate;
    }
}
