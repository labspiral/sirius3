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
            this.btnAddHatch1 = new System.Windows.Forms.Button();
            this.btnAddHatch2 = new System.Windows.Forms.Button();
            this.btnAddHatch3 = new System.Windows.Forms.Button();
            this.btnHatchOrder = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnPrepare);
            this.flowLayoutPanel1.Controls.Add(this.btnAddHatch1);
            this.flowLayoutPanel1.Controls.Add(this.btnAddHatch2);
            this.flowLayoutPanel1.Controls.Add(this.btnAddHatch3);
            this.flowLayoutPanel1.Controls.Add(this.btnHatchOrder);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1264, 38);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnPrepare
            // 
            this.btnPrepare.AutoSize = true;
            this.btnPrepare.Location = new System.Drawing.Point(3, 3);
            this.btnPrepare.Name = "btnPrepare";
            this.btnPrepare.Size = new System.Drawing.Size(120, 32);
            this.btnPrepare.TabIndex = 0;
            this.btnPrepare.Text = "Prepare";
            this.btnPrepare.UseVisualStyleBackColor = true;
            // 
            // btnAddHatch1
            // 
            this.btnAddHatch1.AutoSize = true;
            this.btnAddHatch1.Location = new System.Drawing.Point(129, 3);
            this.btnAddHatch1.Name = "btnAddHatch1";
            this.btnAddHatch1.Size = new System.Drawing.Size(120, 32);
            this.btnAddHatch1.TabIndex = 2;
            this.btnAddHatch1.Text = "Add Hatch1";
            this.btnAddHatch1.UseVisualStyleBackColor = true;
            // 
            // btnAddHatch2
            // 
            this.btnAddHatch2.AutoSize = true;
            this.btnAddHatch2.Location = new System.Drawing.Point(255, 3);
            this.btnAddHatch2.Name = "btnAddHatch2";
            this.btnAddHatch2.Size = new System.Drawing.Size(120, 32);
            this.btnAddHatch2.TabIndex = 3;
            this.btnAddHatch2.Text = "Add Hatch2";
            this.btnAddHatch2.UseVisualStyleBackColor = true;
            // 
            // btnAddHatch3
            // 
            this.btnAddHatch3.AutoSize = true;
            this.btnAddHatch3.Location = new System.Drawing.Point(381, 3);
            this.btnAddHatch3.Name = "btnAddHatch3";
            this.btnAddHatch3.Size = new System.Drawing.Size(120, 32);
            this.btnAddHatch3.TabIndex = 4;
            this.btnAddHatch3.Text = "Add Hatch3";
            this.btnAddHatch3.UseVisualStyleBackColor = true;
            // 
            // btnHatchOrder
            // 
            this.btnHatchOrder.AutoSize = true;
            this.btnHatchOrder.Location = new System.Drawing.Point(507, 3);
            this.btnHatchOrder.Name = "btnHatchOrder";
            this.btnHatchOrder.Size = new System.Drawing.Size(120, 32);
            this.btnHatchOrder.TabIndex = 5;
            this.btnHatchOrder.Text = "Harch Order";
            this.btnHatchOrder.UseVisualStyleBackColor = true;
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
        private Button btnPrepare;
        private Button btnAddHatch1;
        private Button btnAddHatch2;
        private Button btnAddHatch3;
        private Button btnHatchOrder;
    }
}
