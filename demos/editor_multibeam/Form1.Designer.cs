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
            this.lblMode = new System.Windows.Forms.Label();
            this.btnCheckPins = new System.Windows.Forms.Button();
            this.btnNone = new System.Windows.Forms.Button();
            this.btnHead1 = new System.Windows.Forms.Button();
            this.btnHead2 = new System.Windows.Forms.Button();
            this.btnHead12 = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.siriusEditorControl2 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lblMode);
            this.flowLayoutPanel1.Controls.Add(this.btnCheckPins);
            this.flowLayoutPanel1.Controls.Add(this.btnNone);
            this.flowLayoutPanel1.Controls.Add(this.btnHead1);
            this.flowLayoutPanel1.Controls.Add(this.btnHead2);
            this.flowLayoutPanel1.Controls.Add(this.btnHead12);
            this.flowLayoutPanel1.Controls.Add(this.btnStop);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1264, 32);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // lblMode
            // 
            this.lblMode.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMode.Location = new System.Drawing.Point(3, 0);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(200, 29);
            this.lblMode.TabIndex = 5;
            this.lblMode.Text = "MultiBeam Mode: ";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCheckPins
            // 
            this.btnCheckPins.Location = new System.Drawing.Point(209, 3);
            this.btnCheckPins.Name = "btnCheckPins";
            this.btnCheckPins.Size = new System.Drawing.Size(47, 26);
            this.btnCheckPins.TabIndex = 6;
            this.btnCheckPins.Text = "Check Pins";
            this.btnCheckPins.UseVisualStyleBackColor = true;
            // 
            // btnNone
            // 
            this.btnNone.Location = new System.Drawing.Point(262, 3);
            this.btnNone.Name = "btnNone";
            this.btnNone.Size = new System.Drawing.Size(98, 26);
            this.btnNone.TabIndex = 4;
            this.btnNone.Text = "Select NONE";
            this.btnNone.UseVisualStyleBackColor = true;
            // 
            // btnHead1
            // 
            this.btnHead1.Location = new System.Drawing.Point(366, 3);
            this.btnHead1.Name = "btnHead1";
            this.btnHead1.Size = new System.Drawing.Size(98, 26);
            this.btnHead1.TabIndex = 0;
            this.btnHead1.Text = "Select HEAD1";
            this.btnHead1.UseVisualStyleBackColor = true;
            // 
            // btnHead2
            // 
            this.btnHead2.Location = new System.Drawing.Point(470, 3);
            this.btnHead2.Name = "btnHead2";
            this.btnHead2.Size = new System.Drawing.Size(98, 26);
            this.btnHead2.TabIndex = 2;
            this.btnHead2.Text = "Select HEAD2";
            this.btnHead2.UseVisualStyleBackColor = true;
            // 
            // btnHead12
            // 
            this.btnHead12.Location = new System.Drawing.Point(574, 3);
            this.btnHead12.Name = "btnHead12";
            this.btnHead12.Size = new System.Drawing.Size(98, 26);
            this.btnHead12.TabIndex = 3;
            this.btnHead12.Text = "Select Both";
            this.btnHead12.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(678, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(47, 26);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.ItemSize = new System.Drawing.Size(80, 28);
            this.tabControl1.Location = new System.Drawing.Point(0, 32);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1264, 829);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.siriusEditorControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 32);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1256, 793);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "EDITOR 1";
            this.tabPage1.UseVisualStyleBackColor = true;
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
            this.siriusEditorControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorControl1.Laser = null;
            this.siriusEditorControl1.Location = new System.Drawing.Point(3, 3);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Marker = null;
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.PowerMeter = null;
            this.siriusEditorControl1.Scanner = null;
            this.siriusEditorControl1.Size = new System.Drawing.Size(1250, 787);
            this.siriusEditorControl1.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.siriusEditorControl2);
            this.tabPage2.Location = new System.Drawing.Point(4, 32);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1256, 793);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "EDITOR 2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // siriusEditorControl2
            // 
            this.siriusEditorControl2.AliasName = "NoName";
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
            this.siriusEditorControl2.Size = new System.Drawing.Size(1250, 787);
            this.siriusEditorControl2.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 861);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Sirius3 Demo - (c)SpiralLAB";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnHead1;
        private Button btnHead2;
        private Button btnHead12;
        private Button btnStop;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private TabPage tabPage2;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl2;
        private Button btnNone;
        private Label lblMode;
        private Button btnCheckPins;
    }
}
