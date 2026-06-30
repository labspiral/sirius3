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
            this.btnZPL1 = new System.Windows.Forms.Button();
            this.btnZPL2 = new System.Windows.Forms.Button();
            this.btnZPL3 = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.btnFontLoader = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnZPL1);
            this.flowLayoutPanel1.Controls.Add(this.btnZPL2);
            this.flowLayoutPanel1.Controls.Add(this.btnZPL3);
            this.flowLayoutPanel1.Controls.Add(this.btnFontLoader);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1264, 38);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnZPL1
            // 
            this.btnZPL1.AutoSize = true;
            this.btnZPL1.Location = new System.Drawing.Point(3, 3);
            this.btnZPL1.Name = "btnZPL1";
            this.btnZPL1.Size = new System.Drawing.Size(120, 32);
            this.btnZPL1.TabIndex = 0;
            this.btnZPL1.Text = "ZPL 1";
            this.btnZPL1.UseVisualStyleBackColor = true;
            // 
            // btnZPL2
            // 
            this.btnZPL2.AutoSize = true;
            this.btnZPL2.Location = new System.Drawing.Point(129, 3);
            this.btnZPL2.Name = "btnZPL2";
            this.btnZPL2.Size = new System.Drawing.Size(120, 32);
            this.btnZPL2.TabIndex = 2;
            this.btnZPL2.Text = "ZPL 2";
            this.btnZPL2.UseVisualStyleBackColor = true;
            // 
            // btnZPL3
            // 
            this.btnZPL3.AutoSize = true;
            this.btnZPL3.Location = new System.Drawing.Point(255, 3);
            this.btnZPL3.Name = "btnZPL3";
            this.btnZPL3.Size = new System.Drawing.Size(120, 32);
            this.btnZPL3.TabIndex = 3;
            this.btnZPL3.Text = "ZPL 3";
            this.btnZPL3.UseVisualStyleBackColor = true;
            // 
            // siriusEditorControl1
            // 
            this.siriusEditorControl1.AliasName = "NoName";
            this.siriusEditorControl1.AutoSize = true;
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
            this.siriusEditorControl1.Size = new System.Drawing.Size(1264, 823);
            this.siriusEditorControl1.TabIndex = 2;
            // 
            // btnFontLoader
            // 
            this.btnFontLoader.AutoSize = true;
            this.btnFontLoader.Location = new System.Drawing.Point(381, 3);
            this.btnFontLoader.Name = "btnFontLoader";
            this.btnFontLoader.Size = new System.Drawing.Size(120, 32);
            this.btnFontLoader.TabIndex = 4;
            this.btnFontLoader.Text = "Font Loader";
            this.btnFontLoader.UseVisualStyleBackColor = true;
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
        private Button btnZPL1;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnZPL2;
        private Button btnZPL3;
        private Button btnFontLoader;
    }
}
