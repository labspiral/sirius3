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
            this.btnCreateGrids0 = new System.Windows.Forms.Button();
            this.btnCorrection3D = new System.Windows.Forms.Button();
            this.btnSelectTable = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.btnCreateGrids5 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCreateGrids0);
            this.flowLayoutPanel1.Controls.Add(this.btnCreateGrids5);
            this.flowLayoutPanel1.Controls.Add(this.btnCorrection3D);
            this.flowLayoutPanel1.Controls.Add(this.btnSelectTable);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(2254, 48);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnCreateGrids0
            // 
            this.btnCreateGrids0.Location = new System.Drawing.Point(3, 3);
            this.btnCreateGrids0.Name = "btnCreateGrids0";
            this.btnCreateGrids0.Size = new System.Drawing.Size(174, 42);
            this.btnCreateGrids0.TabIndex = 0;
            this.btnCreateGrids0.Text = "Create Grids (Z=0)";
            this.btnCreateGrids0.UseVisualStyleBackColor = true;
            // 
            // btnCorrection3D
            // 
            this.btnCorrection3D.Location = new System.Drawing.Point(363, 3);
            this.btnCorrection3D.Name = "btnCorrection3D";
            this.btnCorrection3D.Size = new System.Drawing.Size(145, 42);
            this.btnCorrection3D.TabIndex = 1;
            this.btnCorrection3D.Text = "Correction 3D";
            this.btnCorrection3D.UseVisualStyleBackColor = true;
            // 
            // btnSelectTable
            // 
            this.btnSelectTable.Location = new System.Drawing.Point(514, 3);
            this.btnSelectTable.Name = "btnSelectTable";
            this.btnSelectTable.Size = new System.Drawing.Size(145, 42);
            this.btnSelectTable.TabIndex = 2;
            this.btnSelectTable.Text = "Select Table";
            this.btnSelectTable.UseVisualStyleBackColor = true;
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
            // btnCreateGrids5
            // 
            this.btnCreateGrids5.Location = new System.Drawing.Point(183, 3);
            this.btnCreateGrids5.Name = "btnCreateGrids5";
            this.btnCreateGrids5.Size = new System.Drawing.Size(174, 42);
            this.btnCreateGrids5.TabIndex = 3;
            this.btnCreateGrids5.Text = "Create Grids (Z=5)";
            this.btnCreateGrids5.UseVisualStyleBackColor = true;
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
        private Button btnCreateGrids0;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnCorrection3D;
        private Button btnSelectTable;
        private Button btnCreateGrids5;
    }
}
