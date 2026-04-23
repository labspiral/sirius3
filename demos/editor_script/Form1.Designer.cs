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
            this.btnCreateText = new System.Windows.Forms.Button();
            this.btnScriptShow = new System.Windows.Forms.Button();
            this.btnScriptSave = new System.Windows.Forms.Button();
            this.btnScriptOpen = new System.Windows.Forms.Button();
            this.btnScriptRevert = new System.Windows.Forms.Button();
            this.btnLoadCompile = new System.Windows.Forms.Button();
            this.siriusEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCreateText);
            this.flowLayoutPanel1.Controls.Add(this.btnScriptShow);
            this.flowLayoutPanel1.Controls.Add(this.btnScriptSave);
            this.flowLayoutPanel1.Controls.Add(this.btnScriptOpen);
            this.flowLayoutPanel1.Controls.Add(this.btnScriptRevert);
            this.flowLayoutPanel1.Controls.Add(this.btnLoadCompile);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1264, 38);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // btnCreateText
            // 
            this.btnCreateText.Location = new System.Drawing.Point(3, 3);
            this.btnCreateText.Name = "btnCreateText";
            this.btnCreateText.Size = new System.Drawing.Size(120, 32);
            this.btnCreateText.TabIndex = 0;
            this.btnCreateText.Text = "Create Script Text";
            this.btnCreateText.UseVisualStyleBackColor = true;
            // 
            // btnScriptShow
            // 
            this.btnScriptShow.Location = new System.Drawing.Point(129, 3);
            this.btnScriptShow.Name = "btnScriptShow";
            this.btnScriptShow.Size = new System.Drawing.Size(139, 32);
            this.btnScriptShow.TabIndex = 1;
            this.btnScriptShow.Text = "Script Instance Show";
            this.btnScriptShow.UseVisualStyleBackColor = true;
            // 
            // btnScriptSave
            // 
            this.btnScriptSave.Location = new System.Drawing.Point(274, 3);
            this.btnScriptSave.Name = "btnScriptSave";
            this.btnScriptSave.Size = new System.Drawing.Size(139, 32);
            this.btnScriptSave.TabIndex = 2;
            this.btnScriptSave.Text = "Script Instance Save";
            this.btnScriptSave.UseVisualStyleBackColor = true;
            // 
            // btnScriptOpen
            // 
            this.btnScriptOpen.Location = new System.Drawing.Point(419, 3);
            this.btnScriptOpen.Name = "btnScriptOpen";
            this.btnScriptOpen.Size = new System.Drawing.Size(139, 32);
            this.btnScriptOpen.TabIndex = 3;
            this.btnScriptOpen.Text = "Script Instance  Open";
            this.btnScriptOpen.UseVisualStyleBackColor = true;
            // 
            // btnScriptRevert
            // 
            this.btnScriptRevert.Location = new System.Drawing.Point(564, 3);
            this.btnScriptRevert.Name = "btnScriptRevert";
            this.btnScriptRevert.Size = new System.Drawing.Size(139, 32);
            this.btnScriptRevert.TabIndex = 4;
            this.btnScriptRevert.Text = "Script Instance Revert";
            this.btnScriptRevert.UseVisualStyleBackColor = true;
            // 
            // btnLoadCompile
            // 
            this.btnLoadCompile.Location = new System.Drawing.Point(709, 3);
            this.btnLoadCompile.Name = "btnLoadCompile";
            this.btnLoadCompile.Size = new System.Drawing.Size(139, 32);
            this.btnLoadCompile.TabIndex = 5;
            this.btnLoadCompile.Text = "Script Load/Compile";
            this.btnLoadCompile.UseVisualStyleBackColor = true;
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
            this.siriusEditorControl1.Location = new System.Drawing.Point(0, 38);
            this.siriusEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusEditorControl1.Name = "siriusEditorControl1";
            this.siriusEditorControl1.Size = new System.Drawing.Size(1264, 823);
            this.siriusEditorControl1.TabIndex = 4;
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
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnScriptShow;
        private Button btnScriptOpen;
        private Button btnScriptSave;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl siriusEditorControl1;
        private Button btnLoadCompile;
        private Button btnCreateText;
        private Button btnScriptRevert;
    }
}
