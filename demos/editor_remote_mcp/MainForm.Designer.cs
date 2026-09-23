namespace Demos
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeRuntimeResources();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            serverPanel = new Panel();
            tokenTextBox = new TextBox();
            tokenLabel = new Label();
            endpointTextBox = new TextBox();
            endpointLabel = new Label();
            statusValueLabel = new Label();
            statusLabel = new Label();
            editorControl = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
            serverPanel.SuspendLayout();
            SuspendLayout();
            //
            // serverPanel
            //
            serverPanel.Controls.Add(tokenTextBox);
            serverPanel.Controls.Add(tokenLabel);
            serverPanel.Controls.Add(endpointTextBox);
            serverPanel.Controls.Add(endpointLabel);
            serverPanel.Controls.Add(statusValueLabel);
            serverPanel.Controls.Add(statusLabel);
            serverPanel.Dock = DockStyle.Top;
            serverPanel.Location = new Point(0, 0);
            serverPanel.Name = "serverPanel";
            serverPanel.Padding = new Padding(12, 8, 12, 8);
            serverPanel.Size = new Size(1280, 96);
            serverPanel.TabIndex = 0;
            //
            // tokenTextBox
            //
            tokenTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tokenTextBox.Location = new Point(92, 64);
            tokenTextBox.Name = "tokenTextBox";
            tokenTextBox.ReadOnly = true;
            tokenTextBox.Size = new Size(350, 22);
            tokenTextBox.TabIndex = 5;
            //
            // tokenLabel
            //
            tokenLabel.AutoSize = true;
            tokenLabel.Location = new Point(12, 68);
            tokenLabel.Name = "tokenLabel";
            tokenLabel.Size = new Size(76, 13);
            tokenLabel.TabIndex = 4;
            tokenLabel.Text = "Bearer token:";
            //
            // endpointTextBox
            //
            endpointTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            endpointTextBox.Location = new Point(92, 36);
            endpointTextBox.Name = "endpointTextBox";
            endpointTextBox.ReadOnly = true;
            endpointTextBox.Size = new Size(350, 22);
            endpointTextBox.TabIndex = 3;
            //
            // endpointLabel
            //
            endpointLabel.AutoSize = true;
            endpointLabel.Location = new Point(12, 40);
            endpointLabel.Name = "endpointLabel";
            endpointLabel.Size = new Size(58, 13);
            endpointLabel.TabIndex = 2;
            endpointLabel.Text = "Endpoint:";
            //
            // statusValueLabel
            //
            statusValueLabel.AutoSize = true;
            statusValueLabel.Location = new Point(92, 12);
            statusValueLabel.Name = "statusValueLabel";
            statusValueLabel.Size = new Size(52, 13);
            statusValueLabel.TabIndex = 1;
            statusValueLabel.Text = "Disabled";
            //
            // statusLabel
            //
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(12, 12);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(67, 13);
            statusLabel.TabIndex = 0;
            statusLabel.Text = "MCP status:";
            //
            // editorControl
            //
            editorControl.AliasName = "MCP Demo Editor";
            editorControl.BackColor = SystemColors.Control;
            editorControl.Dock = DockStyle.Fill;
            editorControl.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            editorControl.IsPropertyGridWindow = true;
            editorControl.IsShowLogWindow = true;
            editorControl.IsShowPen = true;
            editorControl.IsShowTreeViewAndPen = true;
            editorControl.Location = new Point(0, 96);
            editorControl.Margin = new Padding(0);
            editorControl.Name = "editorControl";
            editorControl.Size = new Size(1280, 804);
            editorControl.TabIndex = 1;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 900);
            Controls.Add(editorControl);
            Controls.Add(serverPanel);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sirius3 MCP Editor Demo - (c)SpiralLab";
            Load += MainForm_Load;
            serverPanel.ResumeLayout(false);
            serverPanel.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel serverPanel;
        private System.Windows.Forms.TextBox tokenTextBox;
        private System.Windows.Forms.Label tokenLabel;
        private System.Windows.Forms.TextBox endpointTextBox;
        private System.Windows.Forms.Label endpointLabel;
        private System.Windows.Forms.Label statusValueLabel;
        private System.Windows.Forms.Label statusLabel;
        private SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl editorControl;
    }
}
