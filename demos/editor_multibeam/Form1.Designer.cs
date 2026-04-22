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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbModeNone = new System.Windows.Forms.RadioButton();
            this.rbModeHead1 = new System.Windows.Forms.RadioButton();
            this.rbModeHead2 = new System.Windows.Forms.RadioButton();
            this.rbModeBoth = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbHead1Side = new System.Windows.Forms.RadioButton();
            this.rbHead2Side = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnReady = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnCheckPins = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.siriusEditorControl2 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.groupBox2);
            this.flowLayoutPanel1.Controls.Add(this.groupBox3);
            this.flowLayoutPanel1.Controls.Add(this.btnCheckPins);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1264, 85);
            this.flowLayoutPanel1.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbModeNone);
            this.groupBox1.Controls.Add(this.rbModeHead1);
            this.groupBox1.Controls.Add(this.rbModeHead2);
            this.groupBox1.Controls.Add(this.rbModeBoth);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(298, 76);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mode";
            // 
            // rbModeNone
            // 
            this.rbModeNone.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeNone.Checked = true;
            this.rbModeNone.Location = new System.Drawing.Point(10, 24);
            this.rbModeNone.Name = "rbModeNone";
            this.rbModeNone.Size = new System.Drawing.Size(64, 42);
            this.rbModeNone.TabIndex = 12;
            this.rbModeNone.TabStop = true;
            this.rbModeNone.Text = "None";
            this.rbModeNone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeNone.UseVisualStyleBackColor = true;
            // 
            // rbModeHead1
            // 
            this.rbModeHead1.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeHead1.Location = new System.Drawing.Point(80, 24);
            this.rbModeHead1.Name = "rbModeHead1";
            this.rbModeHead1.Size = new System.Drawing.Size(64, 42);
            this.rbModeHead1.TabIndex = 13;
            this.rbModeHead1.TabStop = true;
            this.rbModeHead1.Text = "Head 1";
            this.rbModeHead1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeHead1.UseVisualStyleBackColor = true;
            // 
            // rbModeHead2
            // 
            this.rbModeHead2.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeHead2.Location = new System.Drawing.Point(150, 24);
            this.rbModeHead2.Name = "rbModeHead2";
            this.rbModeHead2.Size = new System.Drawing.Size(64, 42);
            this.rbModeHead2.TabIndex = 14;
            this.rbModeHead2.TabStop = true;
            this.rbModeHead2.Text = "Head 2";
            this.rbModeHead2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeHead2.UseVisualStyleBackColor = true;
            // 
            // rbModeBoth
            // 
            this.rbModeBoth.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeBoth.Location = new System.Drawing.Point(220, 24);
            this.rbModeBoth.Name = "rbModeBoth";
            this.rbModeBoth.Size = new System.Drawing.Size(64, 42);
            this.rbModeBoth.TabIndex = 15;
            this.rbModeBoth.TabStop = true;
            this.rbModeBoth.Text = "Both";
            this.rbModeBoth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeBoth.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbHead1Side);
            this.groupBox2.Controls.Add(this.rbHead2Side);
            this.groupBox2.Location = new System.Drawing.Point(307, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(168, 76);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Prefer (Both Only)";
            // 
            // rbHead1Side
            // 
            this.rbHead1Side.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbHead1Side.Checked = true;
            this.rbHead1Side.Image = ((System.Drawing.Image)(resources.GetObject("rbHead1Side.Image")));
            this.rbHead1Side.Location = new System.Drawing.Point(20, 24);
            this.rbHead1Side.Name = "rbHead1Side";
            this.rbHead1Side.Size = new System.Drawing.Size(64, 42);
            this.rbHead1Side.TabIndex = 6;
            this.rbHead1Side.TabStop = true;
            this.rbHead1Side.Text = "Head 1";
            this.rbHead1Side.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbHead1Side.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.rbHead1Side.UseVisualStyleBackColor = true;
            // 
            // rbHead2Side
            // 
            this.rbHead2Side.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbHead2Side.Image = ((System.Drawing.Image)(resources.GetObject("rbHead2Side.Image")));
            this.rbHead2Side.Location = new System.Drawing.Point(90, 24);
            this.rbHead2Side.Name = "rbHead2Side";
            this.rbHead2Side.Size = new System.Drawing.Size(64, 42);
            this.rbHead2Side.TabIndex = 7;
            this.rbHead2Side.TabStop = true;
            this.rbHead2Side.Text = "Head 2";
            this.rbHead2Side.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbHead2Side.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.rbHead2Side.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnReady);
            this.groupBox3.Controls.Add(this.btnReset);
            this.groupBox3.Controls.Add(this.btnStop);
            this.groupBox3.Controls.Add(this.btnStart);
            this.groupBox3.Location = new System.Drawing.Point(481, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(311, 76);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Control";
            // 
            // btnReady
            // 
            this.btnReady.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnReady.Image = ((System.Drawing.Image)(resources.GetObject("btnReady.Image")));
            this.btnReady.Location = new System.Drawing.Point(16, 24);
            this.btnReady.Name = "btnReady";
            this.btnReady.Size = new System.Drawing.Size(64, 42);
            this.btnReady.TabIndex = 72;
            this.btnReady.Text = "Ready";
            this.btnReady.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReady.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnReady.UseVisualStyleBackColor = false;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.Image = ((System.Drawing.Image)(resources.GetObject("btnReset.Image")));
            this.btnReset.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReset.Location = new System.Drawing.Point(231, 24);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 8, 4, 8);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(64, 42);
            this.btnReset.TabIndex = 71;
            this.btnReset.Text = "Reset";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnReset.UseVisualStyleBackColor = false;
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStop.Location = new System.Drawing.Point(159, 24);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 8, 4, 8);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(64, 42);
            this.btnStop.TabIndex = 70;
            this.btnStop.Text = "Stop";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStop.UseVisualStyleBackColor = false;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStart.Location = new System.Drawing.Point(87, 24);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 8, 4, 8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(64, 42);
            this.btnStart.TabIndex = 69;
            this.btnStart.Text = "Start";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStart.UseVisualStyleBackColor = false;
            // 
            // btnCheckPins
            // 
            this.btnCheckPins.Location = new System.Drawing.Point(798, 3);
            this.btnCheckPins.Name = "btnCheckPins";
            this.btnCheckPins.Size = new System.Drawing.Size(64, 76);
            this.btnCheckPins.TabIndex = 17;
            this.btnCheckPins.Text = "Check Pins";
            this.btnCheckPins.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.ItemSize = new System.Drawing.Size(100, 28);
            this.tabControl1.Location = new System.Drawing.Point(0, 85);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1264, 776);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.siriusEditorControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 32);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1256, 740);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "EDITOR 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // siriusEditorControl1
            // 
            this.siriusEditorControl1.AliasName = "NoName";
            this.siriusEditorControl1.BackColor = System.Drawing.SystemColors.Control;
            this.siriusEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusEditorControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorControl1.IsPropertyGridWindow = true;
            this.siriusEditorControl1.IsShowLogWindow = true;
            this.siriusEditorControl1.IsShowTreeViewAndPen = true;
            this.siriusEditorControl1.Location = new System.Drawing.Point(3, 3);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.Size = new System.Drawing.Size(1250, 734);
            this.siriusEditorControl1.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.siriusEditorControl2);
            this.tabPage2.Location = new System.Drawing.Point(4, 32);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1256, 740);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "EDITOR 2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // siriusEditorControl2
            // 
            this.siriusEditorControl2.AliasName = "NoName";
            this.siriusEditorControl2.BackColor = System.Drawing.SystemColors.Control;
            this.siriusEditorControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusEditorControl2.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorControl2.IsPropertyGridWindow = true;
            this.siriusEditorControl2.IsShowLogWindow = true;
            this.siriusEditorControl2.IsShowTreeViewAndPen = true;
            this.siriusEditorControl2.Location = new System.Drawing.Point(3, 3);
            this.siriusEditorControl2.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl2.Name = "siriusEditorControl2";
            this.siriusEditorControl2.Size = new System.Drawing.Size(1250, 734);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private GroupBox groupBox1;
        private RadioButton rbModeNone;
        private RadioButton rbModeHead1;
        private RadioButton rbModeHead2;
        private RadioButton rbModeBoth;
        private GroupBox groupBox2;
        private RadioButton rbHead1Side;
        private RadioButton rbHead2Side;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private TabPage tabPage2;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl2;
        private GroupBox groupBox3;
        private Button btnReady;
        private Button btnReset;
        private Button btnStop;
        private Button btnStart;
        private Button btnCheckPins;
    }
}
