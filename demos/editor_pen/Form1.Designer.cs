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
            this.btnPrepare = new System.Windows.Forms.Button();
            this.btnEventHandler = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.btnMarkPage1 = new System.Windows.Forms.Button();
            this.btnMarkPage2 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnPrepare);
            this.flowLayoutPanel1.Controls.Add(this.btnEventHandler);
            this.flowLayoutPanel1.Controls.Add(this.btnMarkPage1);
            this.flowLayoutPanel1.Controls.Add(this.btnMarkPage2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(2254, 55);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnPrepare
            // 
            this.btnPrepare.Location = new System.Drawing.Point(3, 3);
            this.btnPrepare.Name = "btnPrepare";
            this.btnPrepare.Size = new System.Drawing.Size(118, 49);
            this.btnPrepare.TabIndex = 0;
            this.btnPrepare.Text = "Prepare";
            this.btnPrepare.UseVisualStyleBackColor = true;
            // 
            // btnEventHandler
            // 
            this.btnEventHandler.Location = new System.Drawing.Point(127, 3);
            this.btnEventHandler.Name = "btnEventHandler";
            this.btnEventHandler.Size = new System.Drawing.Size(156, 49);
            this.btnEventHandler.TabIndex = 5;
            this.btnEventHandler.Text = "Event for Pen";
            this.btnEventHandler.UseVisualStyleBackColor = true;
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
            this.siriusEditorControl1.Stage = null;
            this.siriusEditorControl1.TabIndex = 2;
            // 
            // btnMarkPage1
            // 
            this.btnMarkPage1.Location = new System.Drawing.Point(289, 3);
            this.btnMarkPage1.Name = "btnMarkPage1";
            this.btnMarkPage1.Size = new System.Drawing.Size(156, 49);
            this.btnMarkPage1.TabIndex = 6;
            this.btnMarkPage1.Text = "Mark Page1";
            this.btnMarkPage1.UseVisualStyleBackColor = true;
            // 
            // btnMarkPage2
            // 
            this.btnMarkPage2.Location = new System.Drawing.Point(451, 3);
            this.btnMarkPage2.Name = "btnMarkPage2";
            this.btnMarkPage2.Size = new System.Drawing.Size(156, 49);
            this.btnMarkPage2.TabIndex = 7;
            this.btnMarkPage2.Text = "Mark Page2";
            this.btnMarkPage2.UseVisualStyleBackColor = true;
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
        private Button btnPrepare;
        private Button btnEventHandler;
        private Button btnMarkPage1;
        private Button btnMarkPage2;
    }
}
