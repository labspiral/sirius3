namespace Demos
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    try { DisposeEditorDocument(); }
                    finally { components?.Dispose(); }
                }
            }
            finally { base.Dispose(disposing); }
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.toolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.editor = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            this.samples = new System.Windows.Forms.ComboBox();
            this.btnDialog = new System.Windows.Forms.Button();
            this.toolbar.SuspendLayout();
            this.SuspendLayout();
            //
            // toolbar
            //
            this.toolbar.Controls.Add(this.samples);
            this.toolbar.Controls.Add(this.btnDialog);
            this.toolbar.AutoSize = true;
            this.toolbar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.toolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.toolbar.Name = "toolbar";
            this.toolbar.Padding = new System.Windows.Forms.Padding(3);
            this.toolbar.Size = new System.Drawing.Size(1440, 39);
            this.toolbar.TabIndex = 0;
            //
            // samples
            //
            this.samples.Name = "samples";
            this.samples.TabIndex = 0;
            this.samples.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.samples.Items.AddRange(new object[] {
                "2D Paths | Line, arc, ellipse, Bezier",
                "2D Paths | Spiral",
                "2D Regions | Hatched rectangle",
                "2D Regions | Nested contours / hole",
                "2D Regions | Rectangle, triangle, cross, trepan",
                "Generated | 2D Barcode / Data Matrix dots",
                "Generated | 2D Barcode / Data Matrix outline",
                "Generated | 2D Barcode / Data Matrix outline + hatch",
                "Generated | TrueType Text",
                "Generated | TrueType Text + hatch",
                "Groups | Transformed nested group",
                "3D | Polyline and points"});
            this.samples.Size = new System.Drawing.Size(320, 23);
            this.samples.DropDownWidth = 450;
            this.samples.SelectedIndexChanged += new System.EventHandler(this.Samples_SelectedIndexChanged);
            //
            // btnDialog
            //
            this.btnDialog.Name = "btnDialog";
            this.btnDialog.TabIndex = 1;
            this.btnDialog.Text = "Clipping dialog...";
            this.btnDialog.AutoSize = true;
            this.btnDialog.Size = new System.Drawing.Size(110, 27);
            this.btnDialog.UseVisualStyleBackColor = true;
            this.btnDialog.Click += new System.EventHandler(this.BtnDialog_Click);
            //
            // editor
            //
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.Location = new System.Drawing.Point(0, 39);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(1440, 861);
            this.editor.TabIndex = 1;
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1440, 900);
            this.Controls.Add(this.editor);
            this.Controls.Add(this.toolbar);
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Name = "Form1";
            this.Text = "IClippable - editor_clip";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloseAsync);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.FlowLayoutPanel toolbar;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl editor;
        private System.Windows.Forms.ComboBox samples;
        private System.Windows.Forms.Button btnDialog;
    }
}
