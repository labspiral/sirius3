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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.siriusEditorControl2 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.ItemSize = new System.Drawing.Size(100, 42);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(2254, 1638);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.siriusEditorControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 46);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(2246, 1588);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "EDITOR 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.siriusEditorControl2);
            this.tabPage2.Location = new System.Drawing.Point(4, 46);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(2246, 1588);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "EDITOR 2";
            this.tabPage2.UseVisualStyleBackColor = true;
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
            this.siriusEditorControl1.Location = new System.Drawing.Point(3, 3);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Marker = null;
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.PowerMeter = null;
            this.siriusEditorControl1.Scanner = null;
            this.siriusEditorControl1.Size = new System.Drawing.Size(2240, 1582);
            this.siriusEditorControl1.TabIndex = 3;
            // 
            // siriusEditorControl2
            // 
            this.siriusEditorControl2.BackColor = System.Drawing.SystemColors.Control;
            this.siriusEditorControl2.DIExt1 = null;
            this.siriusEditorControl2.DILaserPort = null;
            this.siriusEditorControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusEditorControl2.DOExt1 = null;
            this.siriusEditorControl2.DOExt2 = null;
            this.siriusEditorControl2.DOLaserPort = null;
            this.siriusEditorControl2.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorControl2.Laser = null;
            this.siriusEditorControl2.Location = new System.Drawing.Point(3, 3);
            this.siriusEditorControl2.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl2.Marker = null;
            this.siriusEditorControl2.Name = "siriusEditorControl2";
            this.siriusEditorControl2.PowerMeter = null;
            this.siriusEditorControl2.Scanner = null;
            this.siriusEditorControl2.Size = new System.Drawing.Size(2240, 1582);
            this.siriusEditorControl2.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2254, 1638);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Sirius3 Demo - (c)SpiralLAB";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl2;
    }
}
