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
            this.btnRhombus = new System.Windows.Forms.Button();
            this.btnFiducial = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.btnDrillHoles = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnRhombus);
            this.flowLayoutPanel1.Controls.Add(this.btnFiducial);
            this.flowLayoutPanel1.Controls.Add(this.btnDrillHoles);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1436, 38);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnRhombus
            // 
            this.btnRhombus.Location = new System.Drawing.Point(3, 3);
            this.btnRhombus.Name = "btnRhombus";
            this.btnRhombus.Size = new System.Drawing.Size(100, 32);
            this.btnRhombus.TabIndex = 0;
            this.btnRhombus.Text = "Rhombus";
            this.btnRhombus.UseVisualStyleBackColor = true;
            // 
            // btnFiducial
            // 
            this.btnFiducial.Location = new System.Drawing.Point(109, 3);
            this.btnFiducial.Name = "btnFiducial";
            this.btnFiducial.Size = new System.Drawing.Size(100, 32);
            this.btnFiducial.TabIndex = 1;
            this.btnFiducial.Text = "Fiducial";
            this.btnFiducial.UseVisualStyleBackColor = true;
            // 
            // siriusEditorControl1
            // 
            this.siriusEditorControl1.AliasName = "NoName";
            this.siriusEditorControl1.BackColor = System.Drawing.SystemColors.Control;
            this.siriusEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusEditorControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorControl1.IsPropertyGridWindow = true;
            this.siriusEditorControl1.IsShowLogWindow = true;
            this.siriusEditorControl1.IsShowPen = true;
            this.siriusEditorControl1.IsShowTreeViewAndPen = true;
            this.siriusEditorControl1.Location = new System.Drawing.Point(0, 38);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.Size = new System.Drawing.Size(1436, 762);
            this.siriusEditorControl1.TabIndex = 2;
            // 
            // btnDrillHoles
            // 
            this.btnDrillHoles.Location = new System.Drawing.Point(215, 3);
            this.btnDrillHoles.Name = "btnDrillHoles";
            this.btnDrillHoles.Size = new System.Drawing.Size(100, 32);
            this.btnDrillHoles.TabIndex = 2;
            this.btnDrillHoles.Text = "Drill Holes";
            this.btnDrillHoles.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1436, 800);
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
        private Button btnRhombus;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnFiducial;
        private Button btnDrillHoles;
    }
}
