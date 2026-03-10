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
            this.siriusMultiEditorControl1 = new SpiralLab.Sirius3.UI.WinForms.SiriusMultiEditorControl();
            this.SuspendLayout();
            // 
            // siriusMultiEditorControl1
            // 
            this.siriusMultiEditorControl1.AliasName = "NoName";
            this.siriusMultiEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusMultiEditorControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusMultiEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.siriusMultiEditorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusMultiEditorControl1.MaxDeviceCounts = 4;
            this.siriusMultiEditorControl1.Name = "siriusMultiEditorControl1";
            this.siriusMultiEditorControl1.Size = new System.Drawing.Size(2254, 1638);
            this.siriusMultiEditorControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 800);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Controls.Add(this.siriusMultiEditorControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Sirius3 Multi Demo - (c)SpiralLAB";
            this.ResumeLayout(false);

        }

        #endregion

        private SpiralLab.Sirius3.UI.WinForms.SiriusMultiEditorControl siriusMultiEditorControl1;
    }
}
