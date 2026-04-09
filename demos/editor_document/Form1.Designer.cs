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
            this.btnOriginal = new System.Windows.Forms.Button();
            this.btnDoc1 = new System.Windows.Forms.Button();
            this.btnDoc2 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.viewerControl1 = new SpiralLab.Sirius3.UI.WinForms.ViewerControl();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnOriginal);
            this.flowLayoutPanel1.Controls.Add(this.btnDoc1);
            this.flowLayoutPanel1.Controls.Add(this.btnDoc2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1683, 38);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnOriginal
            // 
            this.btnOriginal.Location = new System.Drawing.Point(3, 3);
            this.btnOriginal.Name = "btnOriginal";
            this.btnOriginal.Size = new System.Drawing.Size(118, 32);
            this.btnOriginal.TabIndex = 0;
            this.btnOriginal.Text = "Document Original";
            this.btnOriginal.UseVisualStyleBackColor = true;
            // 
            // btnDoc1
            // 
            this.btnDoc1.Location = new System.Drawing.Point(127, 3);
            this.btnDoc1.Name = "btnDoc1";
            this.btnDoc1.Size = new System.Drawing.Size(85, 32);
            this.btnDoc1.TabIndex = 1;
            this.btnDoc1.Text = "Document 1";
            this.btnDoc1.UseVisualStyleBackColor = true;
            // 
            // btnDoc2
            // 
            this.btnDoc2.Location = new System.Drawing.Point(218, 3);
            this.btnDoc2.Name = "btnDoc2";
            this.btnDoc2.Size = new System.Drawing.Size(85, 32);
            this.btnDoc2.TabIndex = 2;
            this.btnDoc2.Text = "Document 2";
            this.btnDoc2.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 38);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.viewerControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.siriusEditorControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1683, 723);
            this.splitContainer1.SplitterDistance = 450;
            this.splitContainer1.TabIndex = 6;
            // 
            // viewerControl1
            // 
            this.viewerControl1.AliasName = "NoName";
            this.viewerControl1.BackColor = System.Drawing.SystemColors.Control;
            this.viewerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewerControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewerControl1.Location = new System.Drawing.Point(0, 0);
            this.viewerControl1.Margin = new System.Windows.Forms.Padding(0);
            this.viewerControl1.Name = "viewerControl1";
            this.viewerControl1.Size = new System.Drawing.Size(450, 723);
            this.viewerControl1.TabIndex = 6;
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
            this.siriusEditorControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorControl1.IsPropertyGridWindow = true;
            this.siriusEditorControl1.IsShowLogWindow = false;
            this.siriusEditorControl1.IsShowTreeViewAndPen = true;
            this.siriusEditorControl1.Laser = null;
            this.siriusEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Marker = null;
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.PowerMeter = null;
            this.siriusEditorControl1.Scanner = null;
            this.siriusEditorControl1.Size = new System.Drawing.Size(1229, 723);
            this.siriusEditorControl1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1683, 761);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Sirius3 Demo - (c)SpiralLAB";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnDoc1;
        private SplitContainer splitContainer1;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnDoc2;
        private Button btnOriginal;
        private SpiralLab.Sirius3.UI.WinForms.ViewerControl viewerControl1;
    }
}
