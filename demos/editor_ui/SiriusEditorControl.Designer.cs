namespace Demos
{
    partial class SiriusEditorControl
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SiriusEditorControl));
            this.stsBottom = new System.Windows.Forms.StatusStrip();
            this.lblName = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblProcessTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPowerWatt = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblFileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnLogWindow = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblEncoder = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblReady = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel7 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBusy = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel9 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblError = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel8 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlsTop1 = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ddbOpenNewOptions = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuIncludePage1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIncludePage2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIncludePage3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIncludePage4 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIncludeBlocks = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuIncludeLayerPens = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIncludeScannerPens = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuIncludeWafers = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIncludeSubstrates = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLock = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.splitContainer8 = new System.Windows.Forms.SplitContainer();
            this.tbcLeft = new System.Windows.Forms.TabControl();
            this.tabDocPage1 = new System.Windows.Forms.TabPage();
            this.treeViewPageControl1 = new SpiralLab.Sirius3.UI.WinForms.TreeViewPageControl();
            this.tabDocPage2 = new System.Windows.Forms.TabPage();
            this.treeViewPageControl2 = new SpiralLab.Sirius3.UI.WinForms.TreeViewPageControl();
            this.tabBlockPage = new System.Windows.Forms.TabPage();
            this.treeViewBlockControl1 = new SpiralLab.Sirius3.UI.WinForms.TreeViewBlockControl();
            this.tabWaferPage = new System.Windows.Forms.TabPage();
            this.treeViewWaferControl1 = new SpiralLab.Sirius3.UI.WinForms.TreeViewWaferControl();
            this.tabSubstratePage = new System.Windows.Forms.TabPage();
            this.treeViewSubstrateControl1 = new SpiralLab.Sirius3.UI.WinForms.TreeViewSubstrateControl();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tlcPen = new System.Windows.Forms.TabControl();
            this.tabEntityPen = new System.Windows.Forms.TabPage();
            this.entityPenControl1 = new SpiralLab.Sirius3.UI.WinForms.EntityPenControl();
            this.tabLayerPen = new System.Windows.Forms.TabPage();
            this.layerPenControl1 = new SpiralLab.Sirius3.UI.WinForms.LayerPenControl();
            this.splitContainer7 = new System.Windows.Forms.SplitContainer();
            this.tbcMain = new System.Windows.Forms.TabControl();
            this.tabEditor = new System.Windows.Forms.TabPage();
            this.tabMarker = new System.Windows.Forms.TabPage();
            this.splitContainer10 = new System.Windows.Forms.SplitContainer();
            this.offsetControl1 = new SpiralLab.Sirius3.UI.WinForms.OffsetControl();
            this.markerControl1 = new SpiralLab.Sirius3.UI.WinForms.MarkerControl();
            this.tabManual = new System.Windows.Forms.TabPage();
            this.manualControl1 = new SpiralLab.Sirius3.UI.WinForms.ManualControl();
            this.tabScanner = new System.Windows.Forms.TabPage();
            this.scannerControl1 = new SpiralLab.Sirius3.UI.WinForms.ScannerControl();
            this.tabLaser = new System.Windows.Forms.TabPage();
            this.laserControl1 = new SpiralLab.Sirius3.UI.WinForms.LaserControl();
            this.tabDIO = new System.Windows.Forms.TabPage();
            this.splitContainer9 = new System.Windows.Forms.SplitContainer();
            this.rtcDIControl1 = new SpiralLab.Sirius3.UI.WinForms.RtcDIControl();
            this.rtcDOControl1 = new SpiralLab.Sirius3.UI.WinForms.RtcDOControl();
            this.tabPower = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage18 = new System.Windows.Forms.TabPage();
            this.powerMeterControl1 = new SpiralLab.Sirius3.UI.WinForms.PowerMeterControl();
            this.tabPage19 = new System.Windows.Forms.TabPage();
            this.powerMapControl1 = new SpiralLab.Sirius3.UI.WinForms.PowerMapControl();
            this.logControl1 = new SpiralLab.Sirius3.UI.WinForms.LogControl();
            this.tlcRight = new System.Windows.Forms.TabControl();
            this.tabProperty = new System.Windows.Forms.TabPage();
            this.propertyGridControl1 = new SpiralLab.Sirius3.UI.WinForms.PropertyGridControl();
            this.stsBottom.SuspendLayout();
            this.tlsTop1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).BeginInit();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).BeginInit();
            this.splitContainer8.Panel1.SuspendLayout();
            this.splitContainer8.Panel2.SuspendLayout();
            this.splitContainer8.SuspendLayout();
            this.tbcLeft.SuspendLayout();
            this.tabDocPage1.SuspendLayout();
            this.tabDocPage2.SuspendLayout();
            this.tabBlockPage.SuspendLayout();
            this.tabWaferPage.SuspendLayout();
            this.tabSubstratePage.SuspendLayout();
            this.tlcPen.SuspendLayout();
            this.tabEntityPen.SuspendLayout();
            this.tabLayerPen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
            this.splitContainer7.Panel1.SuspendLayout();
            this.splitContainer7.Panel2.SuspendLayout();
            this.splitContainer7.SuspendLayout();
            this.tbcMain.SuspendLayout();
            this.tabMarker.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer10)).BeginInit();
            this.splitContainer10.Panel1.SuspendLayout();
            this.splitContainer10.Panel2.SuspendLayout();
            this.splitContainer10.SuspendLayout();
            this.tabManual.SuspendLayout();
            this.tabScanner.SuspendLayout();
            this.tabLaser.SuspendLayout();
            this.tabDIO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer9)).BeginInit();
            this.splitContainer9.Panel1.SuspendLayout();
            this.splitContainer9.Panel2.SuspendLayout();
            this.splitContainer9.SuspendLayout();
            this.tabPower.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage18.SuspendLayout();
            this.tabPage19.SuspendLayout();
            this.tlcRight.SuspendLayout();
            this.tabProperty.SuspendLayout();
            this.SuspendLayout();
            // 
            // stsBottom
            // 
            this.stsBottom.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stsBottom.GripMargin = new System.Windows.Forms.Padding(0);
            this.stsBottom.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.stsBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblName,
            this.lblProcessTime,
            this.lblPowerWatt,
            this.lblFileName,
            this.toolStripStatusLabel1,
            this.btnLogWindow,
            this.lblEncoder,
            this.lblReady,
            this.toolStripStatusLabel7,
            this.lblBusy,
            this.toolStripStatusLabel9,
            this.lblError,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel8});
            this.stsBottom.Location = new System.Drawing.Point(0, 1488);
            this.stsBottom.Name = "stsBottom";
            this.stsBottom.Padding = new System.Windows.Forms.Padding(0);
            this.stsBottom.ShowItemToolTips = true;
            this.stsBottom.Size = new System.Drawing.Size(2303, 42);
            this.stsBottom.SizingGrip = false;
            this.stsBottom.TabIndex = 35;
            this.stsBottom.Text = "statusStrip1";
            // 
            // lblName
            // 
            this.lblName.Image = global::Demos.Properties.Resources.Top_View2;
            this.lblName.Margin = new System.Windows.Forms.Padding(8, 4, 0, 3);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblName.Size = new System.Drawing.Size(113, 35);
            this.lblName.Text = "NoName";
            this.lblName.ToolTipText = "Name";
            // 
            // lblProcessTime
            // 
            this.lblProcessTime.Image = global::Demos.Properties.Resources.timer_32;
            this.lblProcessTime.Name = "lblProcessTime";
            this.lblProcessTime.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblProcessTime.Size = new System.Drawing.Size(99, 35);
            this.lblProcessTime.Text = "0 msec";
            this.lblProcessTime.ToolTipText = "Processing Time (msec)";
            // 
            // lblPowerWatt
            // 
            this.lblPowerWatt.Image = global::Demos.Properties.Resources.lightning_bolt_26px;
            this.lblPowerWatt.Name = "lblPowerWatt";
            this.lblPowerWatt.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblPowerWatt.Size = new System.Drawing.Size(90, 35);
            this.lblPowerWatt.Text = "0.0 W";
            this.lblPowerWatt.ToolTipText = "Measured Laser Power (W)";
            // 
            // lblFileName
            // 
            this.lblFileName.Image = global::Demos.Properties.Resources.micro_sd_26px;
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblFileName.Size = new System.Drawing.Size(123, 35);
            this.lblFileName.Text = "(NoName)";
            this.lblFileName.ToolTipText = "File Name";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(1520, 35);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // btnLogWindow
            // 
            this.btnLogWindow.Image = global::Demos.Properties.Resources.pens_32px;
            this.btnLogWindow.Name = "btnLogWindow";
            this.btnLogWindow.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnLogWindow.Size = new System.Drawing.Size(40, 35);
            this.btnLogWindow.ToolTipText = "Show/Hide Log Window";
            // 
            // lblEncoder
            // 
            this.lblEncoder.Image = global::Demos.Properties.Resources.Counter;
            this.lblEncoder.Name = "lblEncoder";
            this.lblEncoder.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblEncoder.Size = new System.Drawing.Size(100, 35);
            this.lblEncoder.Text = "XY: 0, 0";
            this.lblEncoder.ToolTipText = "Encoder Value(s)";
            // 
            // lblReady
            // 
            this.lblReady.BackColor = System.Drawing.Color.Green;
            this.lblReady.ForeColor = System.Drawing.Color.White;
            this.lblReady.Name = "lblReady";
            this.lblReady.Size = new System.Drawing.Size(66, 35);
            this.lblReady.Text = " READY ";
            this.lblReady.ToolTipText = "Ready Status";
            // 
            // toolStripStatusLabel7
            // 
            this.toolStripStatusLabel7.AutoSize = false;
            this.toolStripStatusLabel7.Name = "toolStripStatusLabel7";
            this.toolStripStatusLabel7.Size = new System.Drawing.Size(4, 35);
            // 
            // lblBusy
            // 
            this.lblBusy.ActiveLinkColor = System.Drawing.Color.Red;
            this.lblBusy.BackColor = System.Drawing.Color.Olive;
            this.lblBusy.ForeColor = System.Drawing.Color.White;
            this.lblBusy.Name = "lblBusy";
            this.lblBusy.Size = new System.Drawing.Size(56, 35);
            this.lblBusy.Text = " BUSY ";
            this.lblBusy.ToolTipText = "Busy Status";
            // 
            // toolStripStatusLabel9
            // 
            this.toolStripStatusLabel9.AutoSize = false;
            this.toolStripStatusLabel9.Name = "toolStripStatusLabel9";
            this.toolStripStatusLabel9.Size = new System.Drawing.Size(4, 35);
            // 
            // lblError
            // 
            this.lblError.BackColor = System.Drawing.Color.Maroon;
            this.lblError.ForeColor = System.Drawing.Color.White;
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(68, 35);
            this.lblError.Text = " ERROR ";
            this.lblError.ToolTipText = "Error Status";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(4, 35);
            // 
            // toolStripStatusLabel8
            // 
            this.toolStripStatusLabel8.AutoSize = false;
            this.toolStripStatusLabel8.Name = "toolStripStatusLabel8";
            this.toolStripStatusLabel8.Size = new System.Drawing.Size(8, 35);
            // 
            // tlsTop1
            // 
            this.tlsTop1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tlsTop1.GripMargin = new System.Windows.Forms.Padding(0);
            this.tlsTop1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tlsTop1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tlsTop1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.toolStripSeparator4,
            this.ddbOpenNewOptions,
            this.toolStripSeparator2,
            this.btnSave,
            this.toolStripSeparator3,
            this.btnLock,
            this.toolStripSeparator1});
            this.tlsTop1.Location = new System.Drawing.Point(0, 0);
            this.tlsTop1.Name = "tlsTop1";
            this.tlsTop1.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.tlsTop1.Size = new System.Drawing.Size(2303, 33);
            this.tlsTop1.TabIndex = 36;
            this.tlsTop1.Text = "tlsTop1";
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNew.Image = global::Demos.Properties.Resources.File_new;
            this.btnNew.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(34, 28);
            this.btnNew.Text = "toolStripButton2";
            this.btnNew.ToolTipText = "New";
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = global::Demos.Properties.Resources.Import_Import;
            this.btnOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(34, 28);
            this.btnOpen.Text = "Open";
            this.btnOpen.ToolTipText = "Open";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.AutoSize = false;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(12, 6);
            // 
            // ddbOpenNewOptions
            // 
            this.ddbOpenNewOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ddbOpenNewOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuIncludePage1,
            this.mnuIncludePage2,
            this.mnuIncludePage3,
            this.mnuIncludePage4,
            this.mnuIncludeBlocks,
            this.toolStripMenuItem1,
            this.mnuIncludeLayerPens,
            this.mnuIncludeScannerPens,
            this.toolStripMenuItem3,
            this.mnuIncludeWafers,
            this.mnuIncludeSubstrates});
            this.ddbOpenNewOptions.Image = global::Demos.Properties.Resources.Slider;
            this.ddbOpenNewOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddbOpenNewOptions.Name = "ddbOpenNewOptions";
            this.ddbOpenNewOptions.Size = new System.Drawing.Size(42, 28);
            this.ddbOpenNewOptions.Text = "Open(or New) Options";
            this.ddbOpenNewOptions.ToolTipText = "Open(or New) Options";
            // 
            // mnuIncludePage1
            // 
            this.mnuIncludePage1.Checked = true;
            this.mnuIncludePage1.CheckOnClick = true;
            this.mnuIncludePage1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuIncludePage1.Image = global::Demos.Properties.Resources.cal;
            this.mnuIncludePage1.Name = "mnuIncludePage1";
            this.mnuIncludePage1.Size = new System.Drawing.Size(202, 34);
            this.mnuIncludePage1.Text = "Page1";
            this.mnuIncludePage1.ToolTipText = "Include Page1";
            // 
            // mnuIncludePage2
            // 
            this.mnuIncludePage2.Checked = true;
            this.mnuIncludePage2.CheckOnClick = true;
            this.mnuIncludePage2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuIncludePage2.Image = global::Demos.Properties.Resources._3dcal;
            this.mnuIncludePage2.Name = "mnuIncludePage2";
            this.mnuIncludePage2.Size = new System.Drawing.Size(202, 34);
            this.mnuIncludePage2.Text = "Page2";
            this.mnuIncludePage2.ToolTipText = "Include Page2";
            // 
            // mnuIncludePage3
            // 
            this.mnuIncludePage3.Checked = true;
            this.mnuIncludePage3.CheckOnClick = true;
            this.mnuIncludePage3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuIncludePage3.Enabled = false;
            this.mnuIncludePage3.Image = global::Demos.Properties.Resources._3dcal;
            this.mnuIncludePage3.Name = "mnuIncludePage3";
            this.mnuIncludePage3.Size = new System.Drawing.Size(202, 34);
            this.mnuIncludePage3.Text = "Page3";
            this.mnuIncludePage3.ToolTipText = "Include Page3";
            // 
            // mnuIncludePage4
            // 
            this.mnuIncludePage4.Checked = true;
            this.mnuIncludePage4.CheckOnClick = true;
            this.mnuIncludePage4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuIncludePage4.Enabled = false;
            this.mnuIncludePage4.Image = global::Demos.Properties.Resources._3dcal;
            this.mnuIncludePage4.Name = "mnuIncludePage4";
            this.mnuIncludePage4.Size = new System.Drawing.Size(202, 34);
            this.mnuIncludePage4.Text = "Page4";
            this.mnuIncludePage4.ToolTipText = "Include Page4";
            // 
            // mnuIncludeBlocks
            // 
            this.mnuIncludeBlocks.Checked = true;
            this.mnuIncludeBlocks.CheckOnClick = true;
            this.mnuIncludeBlocks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuIncludeBlocks.Image = global::Demos.Properties.Resources.chain_intermediate_24px;
            this.mnuIncludeBlocks.Name = "mnuIncludeBlocks";
            this.mnuIncludeBlocks.Size = new System.Drawing.Size(202, 34);
            this.mnuIncludeBlocks.Text = "Blocks";
            this.mnuIncludeBlocks.ToolTipText = "Include Blocks";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(199, 6);
            // 
            // mnuIncludeLayerPens
            // 
            this.mnuIncludeLayerPens.Checked = true;
            this.mnuIncludeLayerPens.CheckOnClick = true;
            this.mnuIncludeLayerPens.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuIncludeLayerPens.Image = global::Demos.Properties.Resources.Index;
            this.mnuIncludeLayerPens.Name = "mnuIncludeLayerPens";
            this.mnuIncludeLayerPens.Size = new System.Drawing.Size(202, 34);
            this.mnuIncludeLayerPens.Text = "Layer Pens";
            this.mnuIncludeLayerPens.ToolTipText = "Include Layer Pens";
            // 
            // mnuIncludeScannerPens
            // 
            this.mnuIncludeScannerPens.Checked = true;
            this.mnuIncludeScannerPens.CheckOnClick = true;
            this.mnuIncludeScannerPens.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuIncludeScannerPens.Image = global::Demos.Properties.Resources.Index;
            this.mnuIncludeScannerPens.Name = "mnuIncludeScannerPens";
            this.mnuIncludeScannerPens.Size = new System.Drawing.Size(202, 34);
            this.mnuIncludeScannerPens.Text = "Scanner Pens";
            this.mnuIncludeScannerPens.ToolTipText = "Include Scanner Pens";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(199, 6);
            // 
            // mnuIncludeWafers
            // 
            this.mnuIncludeWafers.Checked = true;
            this.mnuIncludeWafers.CheckOnClick = true;
            this.mnuIncludeWafers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuIncludeWafers.Image = global::Demos.Properties.Resources.Grid4;
            this.mnuIncludeWafers.Name = "mnuIncludeWafers";
            this.mnuIncludeWafers.Size = new System.Drawing.Size(202, 34);
            this.mnuIncludeWafers.Text = "Wafers";
            this.mnuIncludeWafers.ToolTipText = "Include Wafers";
            // 
            // mnuIncludeSubstrates
            // 
            this.mnuIncludeSubstrates.Checked = true;
            this.mnuIncludeSubstrates.CheckOnClick = true;
            this.mnuIncludeSubstrates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuIncludeSubstrates.Image = global::Demos.Properties.Resources.Grid4;
            this.mnuIncludeSubstrates.Name = "mnuIncludeSubstrates";
            this.mnuIncludeSubstrates.Size = new System.Drawing.Size(202, 34);
            this.mnuIncludeSubstrates.Text = "Substrates";
            this.mnuIncludeSubstrates.ToolTipText = "Include Substrates";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(16, 32);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::Demos.Properties.Resources.Save_as;
            this.btnSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(34, 28);
            this.btnSave.Text = "toolStripButton1";
            this.btnSave.ToolTipText = "Save";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AutoSize = false;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(12, 32);
            // 
            // btnLock
            // 
            this.btnLock.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnLock.CheckOnClick = true;
            this.btnLock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLock.Image = global::Demos.Properties.Resources.lock_24px;
            this.btnLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLock.Name = "btnLock";
            this.btnLock.Size = new System.Drawing.Size(34, 28);
            this.btnLock.Text = "Lock";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 33);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 33);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer6);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tlcRight);
            this.splitContainer1.Panel2MinSize = 100;
            this.splitContainer1.Size = new System.Drawing.Size(2303, 1455);
            this.splitContainer1.SplitterDistance = 1839;
            this.splitContainer1.TabIndex = 37;
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer6.Location = new System.Drawing.Point(0, 0);
            this.splitContainer6.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer6.Name = "splitContainer6";
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.splitContainer8);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.splitContainer7);
            this.splitContainer6.Size = new System.Drawing.Size(1839, 1455);
            this.splitContainer6.SplitterDistance = 444;
            this.splitContainer6.TabIndex = 0;
            // 
            // splitContainer8
            // 
            this.splitContainer8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer8.Location = new System.Drawing.Point(0, 0);
            this.splitContainer8.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer8.Name = "splitContainer8";
            this.splitContainer8.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer8.Panel1
            // 
            this.splitContainer8.Panel1.Controls.Add(this.tbcLeft);
            // 
            // splitContainer8.Panel2
            // 
            this.splitContainer8.Panel2.Controls.Add(this.tlcPen);
            this.splitContainer8.Panel2MinSize = 100;
            this.splitContainer8.Size = new System.Drawing.Size(444, 1455);
            this.splitContainer8.SplitterDistance = 1183;
            this.splitContainer8.TabIndex = 3;
            // 
            // tbcLeft
            // 
            this.tbcLeft.Controls.Add(this.tabDocPage1);
            this.tbcLeft.Controls.Add(this.tabDocPage2);
            this.tbcLeft.Controls.Add(this.tabBlockPage);
            this.tbcLeft.Controls.Add(this.tabWaferPage);
            this.tbcLeft.Controls.Add(this.tabSubstratePage);
            this.tbcLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcLeft.HotTrack = true;
            this.tbcLeft.ImageList = this.imageList1;
            this.tbcLeft.ItemSize = new System.Drawing.Size(80, 38);
            this.tbcLeft.Location = new System.Drawing.Point(0, 0);
            this.tbcLeft.Margin = new System.Windows.Forms.Padding(0);
            this.tbcLeft.Name = "tbcLeft";
            this.tbcLeft.Padding = new System.Drawing.Point(3, 3);
            this.tbcLeft.SelectedIndex = 0;
            this.tbcLeft.Size = new System.Drawing.Size(444, 1183);
            this.tbcLeft.TabIndex = 3;
            // 
            // tabDocPage1
            // 
            this.tabDocPage1.Controls.Add(this.treeViewPageControl1);
            this.tabDocPage1.ImageKey = "1.png";
            this.tabDocPage1.Location = new System.Drawing.Point(4, 42);
            this.tabDocPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabDocPage1.Name = "tabDocPage1";
            this.tabDocPage1.Size = new System.Drawing.Size(436, 1137);
            this.tabDocPage1.TabIndex = 0;
            this.tabDocPage1.Text = "Page1 ";
            this.tabDocPage1.UseVisualStyleBackColor = true;
            // 
            // treeViewPageControl1
            // 
            this.treeViewPageControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.treeViewPageControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewPageControl1.Document = null;
            this.treeViewPageControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewPageControl1.Location = new System.Drawing.Point(0, 0);
            this.treeViewPageControl1.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewPageControl1.Name = "treeViewPageControl1";
            this.treeViewPageControl1.Page = SpiralLab.Sirius3.Document.DocumentPages.Page1;
            this.treeViewPageControl1.Size = new System.Drawing.Size(436, 1137);
            this.treeViewPageControl1.TabIndex = 2;
            this.treeViewPageControl1.View = null;
            // 
            // tabDocPage2
            // 
            this.tabDocPage2.Controls.Add(this.treeViewPageControl2);
            this.tabDocPage2.ImageKey = "2.png";
            this.tabDocPage2.Location = new System.Drawing.Point(4, 42);
            this.tabDocPage2.Margin = new System.Windows.Forms.Padding(0);
            this.tabDocPage2.Name = "tabDocPage2";
            this.tabDocPage2.Size = new System.Drawing.Size(436, 1137);
            this.tabDocPage2.TabIndex = 4;
            this.tabDocPage2.Text = "Page2 ";
            this.tabDocPage2.UseVisualStyleBackColor = true;
            // 
            // treeViewPageControl2
            // 
            this.treeViewPageControl2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.treeViewPageControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewPageControl2.Document = null;
            this.treeViewPageControl2.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewPageControl2.Location = new System.Drawing.Point(0, 0);
            this.treeViewPageControl2.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewPageControl2.Name = "treeViewPageControl2";
            this.treeViewPageControl2.Page = SpiralLab.Sirius3.Document.DocumentPages.Page2;
            this.treeViewPageControl2.Size = new System.Drawing.Size(436, 1137);
            this.treeViewPageControl2.TabIndex = 3;
            this.treeViewPageControl2.View = null;
            // 
            // tabBlockPage
            // 
            this.tabBlockPage.Controls.Add(this.treeViewBlockControl1);
            this.tabBlockPage.ImageKey = "chain_intermediate_24px.png";
            this.tabBlockPage.Location = new System.Drawing.Point(4, 42);
            this.tabBlockPage.Margin = new System.Windows.Forms.Padding(0);
            this.tabBlockPage.Name = "tabBlockPage";
            this.tabBlockPage.Size = new System.Drawing.Size(436, 1137);
            this.tabBlockPage.TabIndex = 1;
            this.tabBlockPage.Text = "Block ";
            this.tabBlockPage.UseVisualStyleBackColor = true;
            // 
            // treeViewBlockControl1
            // 
            this.treeViewBlockControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.treeViewBlockControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewBlockControl1.Document = null;
            this.treeViewBlockControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewBlockControl1.Location = new System.Drawing.Point(0, 0);
            this.treeViewBlockControl1.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewBlockControl1.Name = "treeViewBlockControl1";
            this.treeViewBlockControl1.Size = new System.Drawing.Size(436, 1137);
            this.treeViewBlockControl1.TabIndex = 0;
            this.treeViewBlockControl1.View = null;
            // 
            // tabWaferPage
            // 
            this.tabWaferPage.Controls.Add(this.treeViewWaferControl1);
            this.tabWaferPage.ImageKey = "Grid4.png";
            this.tabWaferPage.Location = new System.Drawing.Point(4, 42);
            this.tabWaferPage.Margin = new System.Windows.Forms.Padding(0);
            this.tabWaferPage.Name = "tabWaferPage";
            this.tabWaferPage.Size = new System.Drawing.Size(436, 1137);
            this.tabWaferPage.TabIndex = 2;
            this.tabWaferPage.Text = "Wafer ";
            this.tabWaferPage.UseVisualStyleBackColor = true;
            // 
            // treeViewWaferControl1
            // 
            this.treeViewWaferControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.treeViewWaferControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewWaferControl1.Document = null;
            this.treeViewWaferControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewWaferControl1.Location = new System.Drawing.Point(0, 0);
            this.treeViewWaferControl1.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewWaferControl1.Name = "treeViewWaferControl1";
            this.treeViewWaferControl1.Size = new System.Drawing.Size(436, 1137);
            this.treeViewWaferControl1.TabIndex = 1;
            this.treeViewWaferControl1.View = null;
            // 
            // tabSubstratePage
            // 
            this.tabSubstratePage.Controls.Add(this.treeViewSubstrateControl1);
            this.tabSubstratePage.ImageKey = "Grid3.png";
            this.tabSubstratePage.Location = new System.Drawing.Point(4, 42);
            this.tabSubstratePage.Margin = new System.Windows.Forms.Padding(0);
            this.tabSubstratePage.Name = "tabSubstratePage";
            this.tabSubstratePage.Size = new System.Drawing.Size(436, 1137);
            this.tabSubstratePage.TabIndex = 3;
            this.tabSubstratePage.Text = "Substrate";
            this.tabSubstratePage.UseVisualStyleBackColor = true;
            // 
            // treeViewSubstrateControl1
            // 
            this.treeViewSubstrateControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.treeViewSubstrateControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSubstrateControl1.Document = null;
            this.treeViewSubstrateControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewSubstrateControl1.Location = new System.Drawing.Point(0, 0);
            this.treeViewSubstrateControl1.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewSubstrateControl1.Name = "treeViewSubstrateControl1";
            this.treeViewSubstrateControl1.Size = new System.Drawing.Size(436, 1137);
            this.treeViewSubstrateControl1.TabIndex = 0;
            this.treeViewSubstrateControl1.View = null;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "cube_24px.png");
            this.imageList1.Images.SetKeyName(1, "binary_code2_24px.png");
            this.imageList1.Images.SetKeyName(2, "welder_24px.png");
            this.imageList1.Images.SetKeyName(3, "sheets_24px.png");
            this.imageList1.Images.SetKeyName(4, "candy_cane_pattern_24px.png");
            this.imageList1.Images.SetKeyName(5, "spreadsheet_file_24px.png");
            this.imageList1.Images.SetKeyName(6, "Electricity Hazard_24px.png");
            this.imageList1.Images.SetKeyName(7, "paint_palette_24px.png");
            this.imageList1.Images.SetKeyName(8, "sheets2_24px.png");
            this.imageList1.Images.SetKeyName(9, "chain_intermediate_24px.png");
            this.imageList1.Images.SetKeyName(10, "Stacked Organizational Chart.png");
            this.imageList1.Images.SetKeyName(11, "Login.png");
            this.imageList1.Images.SetKeyName(12, "Binary Code.png");
            this.imageList1.Images.SetKeyName(13, "Ctrl.png");
            this.imageList1.Images.SetKeyName(14, "Video Card.png");
            this.imageList1.Images.SetKeyName(15, "Picture.png");
            this.imageList1.Images.SetKeyName(16, "Property.png");
            this.imageList1.Images.SetKeyName(17, "3D Object.png");
            this.imageList1.Images.SetKeyName(18, "Circled Play.png");
            this.imageList1.Images.SetKeyName(19, "Paint Palette.png");
            this.imageList1.Images.SetKeyName(20, "Vending Machine.png");
            this.imageList1.Images.SetKeyName(21, "Processor.png");
            this.imageList1.Images.SetKeyName(22, "Processor2.png");
            this.imageList1.Images.SetKeyName(23, "Pencil.png");
            this.imageList1.Images.SetKeyName(24, "Design.png");
            this.imageList1.Images.SetKeyName(25, "Video Card.png");
            this.imageList1.Images.SetKeyName(26, "light_on_24px.png");
            this.imageList1.Images.SetKeyName(27, "Broadcasting.png");
            this.imageList1.Images.SetKeyName(28, "RS-232 Male.png");
            this.imageList1.Images.SetKeyName(29, "bar_chart_30px.png");
            this.imageList1.Images.SetKeyName(30, "Graph2.png");
            this.imageList1.Images.SetKeyName(31, "Graph.png");
            this.imageList1.Images.SetKeyName(32, "line_chart_24px.png");
            this.imageList1.Images.SetKeyName(33, "line_chart_26px.png");
            this.imageList1.Images.SetKeyName(34, "stocks_24px.png");
            this.imageList1.Images.SetKeyName(35, "stocks_32px.png");
            this.imageList1.Images.SetKeyName(36, "C Sharp Logo.png");
            this.imageList1.Images.SetKeyName(37, "7077517_csharp_file_icon.png");
            this.imageList1.Images.SetKeyName(38, "free-icon-file-and-folder-2807467.png");
            this.imageList1.Images.SetKeyName(39, "csharp.ico");
            this.imageList1.Images.SetKeyName(40, "Voltage.png");
            this.imageList1.Images.SetKeyName(41, "Microscope.png");
            this.imageList1.Images.SetKeyName(42, "roll_of_tickets_24px.png");
            this.imageList1.Images.SetKeyName(43, "paper_24px.png");
            this.imageList1.Images.SetKeyName(44, "driving_directions_24px.png");
            this.imageList1.Images.SetKeyName(45, "Layout.png");
            this.imageList1.Images.SetKeyName(46, "2.png");
            this.imageList1.Images.SetKeyName(47, "1.png");
            this.imageList1.Images.SetKeyName(48, "Grid4.png");
            this.imageList1.Images.SetKeyName(49, "Grid3.png");
            // 
            // tlcPen
            // 
            this.tlcPen.Controls.Add(this.tabEntityPen);
            this.tlcPen.Controls.Add(this.tabLayerPen);
            this.tlcPen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlcPen.HotTrack = true;
            this.tlcPen.ImageList = this.imageList1;
            this.tlcPen.ItemSize = new System.Drawing.Size(74, 38);
            this.tlcPen.Location = new System.Drawing.Point(0, 0);
            this.tlcPen.Multiline = true;
            this.tlcPen.Name = "tlcPen";
            this.tlcPen.SelectedIndex = 0;
            this.tlcPen.Size = new System.Drawing.Size(444, 268);
            this.tlcPen.TabIndex = 4;
            // 
            // tabEntityPen
            // 
            this.tabEntityPen.Controls.Add(this.entityPenControl1);
            this.tabEntityPen.ImageKey = "driving_directions_24px.png";
            this.tabEntityPen.Location = new System.Drawing.Point(4, 42);
            this.tabEntityPen.Margin = new System.Windows.Forms.Padding(0);
            this.tabEntityPen.Name = "tabEntityPen";
            this.tabEntityPen.Size = new System.Drawing.Size(436, 222);
            this.tabEntityPen.TabIndex = 0;
            this.tabEntityPen.Text = "Entity";
            this.tabEntityPen.UseVisualStyleBackColor = true;
            // 
            // entityPenControl1
            // 
            this.entityPenControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.entityPenControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityPenControl1.Document = null;
            this.entityPenControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.entityPenControl1.Location = new System.Drawing.Point(0, 0);
            this.entityPenControl1.Margin = new System.Windows.Forms.Padding(0);
            this.entityPenControl1.Name = "entityPenControl1";
            this.entityPenControl1.Size = new System.Drawing.Size(436, 222);
            this.entityPenControl1.TabIndex = 2;
            // 
            // tabLayerPen
            // 
            this.tabLayerPen.Controls.Add(this.layerPenControl1);
            this.tabLayerPen.ImageKey = "driving_directions_24px.png";
            this.tabLayerPen.Location = new System.Drawing.Point(4, 42);
            this.tabLayerPen.Margin = new System.Windows.Forms.Padding(0);
            this.tabLayerPen.Name = "tabLayerPen";
            this.tabLayerPen.Size = new System.Drawing.Size(436, 222);
            this.tabLayerPen.TabIndex = 1;
            this.tabLayerPen.Text = "Layer";
            this.tabLayerPen.UseVisualStyleBackColor = true;
            // 
            // layerPenControl1
            // 
            this.layerPenControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.layerPenControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layerPenControl1.Document = null;
            this.layerPenControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layerPenControl1.Location = new System.Drawing.Point(0, 0);
            this.layerPenControl1.Margin = new System.Windows.Forms.Padding(16, 15, 16, 15);
            this.layerPenControl1.Name = "layerPenControl1";
            this.layerPenControl1.Size = new System.Drawing.Size(436, 222);
            this.layerPenControl1.TabIndex = 0;
            // 
            // splitContainer7
            // 
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer7.Location = new System.Drawing.Point(0, 0);
            this.splitContainer7.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer7.Name = "splitContainer7";
            this.splitContainer7.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer7.Panel1
            // 
            this.splitContainer7.Panel1.Controls.Add(this.tbcMain);
            this.splitContainer7.Panel1MinSize = 100;
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this.logControl1);
            this.splitContainer7.Panel2MinSize = 100;
            this.splitContainer7.Size = new System.Drawing.Size(1391, 1455);
            this.splitContainer7.SplitterDistance = 1183;
            this.splitContainer7.TabIndex = 0;
            // 
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tabEditor);
            this.tbcMain.Controls.Add(this.tabMarker);
            this.tbcMain.Controls.Add(this.tabManual);
            this.tbcMain.Controls.Add(this.tabScanner);
            this.tbcMain.Controls.Add(this.tabLaser);
            this.tbcMain.Controls.Add(this.tabDIO);
            this.tbcMain.Controls.Add(this.tabPower);
            this.tbcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMain.HotTrack = true;
            this.tbcMain.ImageList = this.imageList1;
            this.tbcMain.ItemSize = new System.Drawing.Size(80, 38);
            this.tbcMain.Location = new System.Drawing.Point(0, 0);
            this.tbcMain.Margin = new System.Windows.Forms.Padding(0);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(1391, 1183);
            this.tbcMain.TabIndex = 0;
            // 
            // tabEditor
            // 
            this.tabEditor.ImageKey = "cube_24px.png";
            this.tabEditor.Location = new System.Drawing.Point(4, 42);
            this.tabEditor.Margin = new System.Windows.Forms.Padding(0);
            this.tabEditor.Name = "tabEditor";
            this.tabEditor.Size = new System.Drawing.Size(1383, 1137);
            this.tabEditor.TabIndex = 0;
            this.tabEditor.Text = "Editor";
            // 
            // tabMarker
            // 
            this.tabMarker.Controls.Add(this.splitContainer10);
            this.tabMarker.ImageKey = "Design.png";
            this.tabMarker.Location = new System.Drawing.Point(4, 42);
            this.tabMarker.Margin = new System.Windows.Forms.Padding(0);
            this.tabMarker.Name = "tabMarker";
            this.tabMarker.Size = new System.Drawing.Size(1383, 1137);
            this.tabMarker.TabIndex = 1;
            this.tabMarker.Text = "Marker";
            this.tabMarker.UseVisualStyleBackColor = true;
            // 
            // splitContainer10
            // 
            this.splitContainer10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer10.Location = new System.Drawing.Point(0, 0);
            this.splitContainer10.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer10.Name = "splitContainer10";
            // 
            // splitContainer10.Panel1
            // 
            this.splitContainer10.Panel1.Controls.Add(this.offsetControl1);
            // 
            // splitContainer10.Panel2
            // 
            this.splitContainer10.Panel2.Controls.Add(this.markerControl1);
            this.splitContainer10.Size = new System.Drawing.Size(1383, 1137);
            this.splitContainer10.SplitterDistance = 446;
            this.splitContainer10.TabIndex = 4;
            // 
            // offsetControl1
            // 
            this.offsetControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.offsetControl1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.offsetControl1.Location = new System.Drawing.Point(0, 0);
            this.offsetControl1.Margin = new System.Windows.Forms.Padding(0);
            this.offsetControl1.Marker = null;
            this.offsetControl1.Name = "offsetControl1";
            this.offsetControl1.Size = new System.Drawing.Size(446, 1137);
            this.offsetControl1.TabIndex = 4;
            // 
            // markerControl1
            // 
            this.markerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.markerControl1.Document = null;
            this.markerControl1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.markerControl1.Laser = null;
            this.markerControl1.Location = new System.Drawing.Point(0, 0);
            this.markerControl1.Margin = new System.Windows.Forms.Padding(0);
            this.markerControl1.Marker = null;
            this.markerControl1.Name = "markerControl1";
            this.markerControl1.PowerMeter = null;
            this.markerControl1.Rtc = null;
            this.markerControl1.Size = new System.Drawing.Size(933, 1137);
            this.markerControl1.TabIndex = 4;
            this.markerControl1.View = null;
            // 
            // tabManual
            // 
            this.tabManual.Controls.Add(this.manualControl1);
            this.tabManual.ImageKey = "Voltage.png";
            this.tabManual.Location = new System.Drawing.Point(4, 42);
            this.tabManual.Margin = new System.Windows.Forms.Padding(0);
            this.tabManual.Name = "tabManual";
            this.tabManual.Size = new System.Drawing.Size(1383, 1137);
            this.tabManual.TabIndex = 2;
            this.tabManual.Text = "Manual";
            this.tabManual.UseVisualStyleBackColor = true;
            // 
            // manualControl1
            // 
            this.manualControl1.BackColor = System.Drawing.SystemColors.Control;
            this.manualControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manualControl1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.manualControl1.Laser = null;
            this.manualControl1.Location = new System.Drawing.Point(0, 0);
            this.manualControl1.Margin = new System.Windows.Forms.Padding(0);
            this.manualControl1.Marker = null;
            this.manualControl1.Name = "manualControl1";
            this.manualControl1.Rtc = null;
            this.manualControl1.Size = new System.Drawing.Size(1383, 1137);
            this.manualControl1.TabIndex = 1;
            // 
            // tabScanner
            // 
            this.tabScanner.Controls.Add(this.scannerControl1);
            this.tabScanner.ImageKey = "Video Card.png";
            this.tabScanner.Location = new System.Drawing.Point(4, 42);
            this.tabScanner.Margin = new System.Windows.Forms.Padding(0);
            this.tabScanner.Name = "tabScanner";
            this.tabScanner.Size = new System.Drawing.Size(1383, 1137);
            this.tabScanner.TabIndex = 3;
            this.tabScanner.Text = "Scanner";
            this.tabScanner.UseVisualStyleBackColor = true;
            // 
            // scannerControl1
            // 
            this.scannerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerControl1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.scannerControl1.Location = new System.Drawing.Point(0, 0);
            this.scannerControl1.Margin = new System.Windows.Forms.Padding(0);
            this.scannerControl1.Name = "scannerControl1";
            this.scannerControl1.Scanner = null;
            this.scannerControl1.Size = new System.Drawing.Size(1383, 1137);
            this.scannerControl1.TabIndex = 0;
            // 
            // tabLaser
            // 
            this.tabLaser.Controls.Add(this.laserControl1);
            this.tabLaser.ImageKey = "Processor2.png";
            this.tabLaser.Location = new System.Drawing.Point(4, 42);
            this.tabLaser.Margin = new System.Windows.Forms.Padding(0);
            this.tabLaser.Name = "tabLaser";
            this.tabLaser.Size = new System.Drawing.Size(1383, 1137);
            this.tabLaser.TabIndex = 4;
            this.tabLaser.Text = "Laser";
            this.tabLaser.UseVisualStyleBackColor = true;
            // 
            // laserControl1
            // 
            this.laserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.laserControl1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.laserControl1.Laser = null;
            this.laserControl1.Location = new System.Drawing.Point(0, 0);
            this.laserControl1.Margin = new System.Windows.Forms.Padding(0);
            this.laserControl1.Name = "laserControl1";
            this.laserControl1.Size = new System.Drawing.Size(1383, 1137);
            this.laserControl1.TabIndex = 0;
            // 
            // tabDIO
            // 
            this.tabDIO.Controls.Add(this.splitContainer9);
            this.tabDIO.ImageKey = "RS-232 Male.png";
            this.tabDIO.Location = new System.Drawing.Point(4, 42);
            this.tabDIO.Margin = new System.Windows.Forms.Padding(0);
            this.tabDIO.Name = "tabDIO";
            this.tabDIO.Size = new System.Drawing.Size(1383, 1137);
            this.tabDIO.TabIndex = 5;
            this.tabDIO.Text = "DIO";
            this.tabDIO.UseVisualStyleBackColor = true;
            // 
            // splitContainer9
            // 
            this.splitContainer9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer9.Location = new System.Drawing.Point(0, 0);
            this.splitContainer9.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer9.Name = "splitContainer9";
            // 
            // splitContainer9.Panel1
            // 
            this.splitContainer9.Panel1.Controls.Add(this.rtcDIControl1);
            // 
            // splitContainer9.Panel2
            // 
            this.splitContainer9.Panel2.Controls.Add(this.rtcDOControl1);
            this.splitContainer9.Size = new System.Drawing.Size(1383, 1137);
            this.splitContainer9.SplitterDistance = 663;
            this.splitContainer9.TabIndex = 0;
            // 
            // rtcDIControl1
            // 
            this.rtcDIControl1.DIExt1 = null;
            this.rtcDIControl1.DILaserPort = null;
            this.rtcDIControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtcDIControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtcDIControl1.Location = new System.Drawing.Point(0, 0);
            this.rtcDIControl1.Margin = new System.Windows.Forms.Padding(0);
            this.rtcDIControl1.Name = "rtcDIControl1";
            this.rtcDIControl1.Size = new System.Drawing.Size(663, 1137);
            this.rtcDIControl1.TabIndex = 0;
            this.rtcDIControl1.UpdateTimerInterval = 100;
            // 
            // rtcDOControl1
            // 
            this.rtcDOControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtcDOControl1.DOExt1 = null;
            this.rtcDOControl1.DOExt2 = null;
            this.rtcDOControl1.DOLaserPort = null;
            this.rtcDOControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtcDOControl1.Location = new System.Drawing.Point(0, 0);
            this.rtcDOControl1.Margin = new System.Windows.Forms.Padding(0);
            this.rtcDOControl1.Marker = null;
            this.rtcDOControl1.Name = "rtcDOControl1";
            this.rtcDOControl1.Size = new System.Drawing.Size(716, 1137);
            this.rtcDOControl1.TabIndex = 0;
            this.rtcDOControl1.UpdateTimerInterval = 100;
            // 
            // tabPower
            // 
            this.tabPower.Controls.Add(this.tabControl2);
            this.tabPower.ImageKey = "Graph2.png";
            this.tabPower.Location = new System.Drawing.Point(4, 42);
            this.tabPower.Margin = new System.Windows.Forms.Padding(0);
            this.tabPower.Name = "tabPower";
            this.tabPower.Size = new System.Drawing.Size(1383, 1137);
            this.tabPower.TabIndex = 6;
            this.tabPower.Text = "Power";
            this.tabPower.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage18);
            this.tabControl2.Controls.Add(this.tabPage19);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.ItemSize = new System.Drawing.Size(106, 32);
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1383, 1137);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage18
            // 
            this.tabPage18.Controls.Add(this.powerMeterControl1);
            this.tabPage18.Location = new System.Drawing.Point(4, 36);
            this.tabPage18.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage18.Name = "tabPage18";
            this.tabPage18.Size = new System.Drawing.Size(1375, 1097);
            this.tabPage18.TabIndex = 0;
            this.tabPage18.Text = "PowerMeter";
            this.tabPage18.UseVisualStyleBackColor = true;
            // 
            // powerMeterControl1
            // 
            this.powerMeterControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.powerMeterControl1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.powerMeterControl1.Laser = null;
            this.powerMeterControl1.Location = new System.Drawing.Point(0, 0);
            this.powerMeterControl1.Margin = new System.Windows.Forms.Padding(0);
            this.powerMeterControl1.Name = "powerMeterControl1";
            this.powerMeterControl1.PowerMeter = null;
            this.powerMeterControl1.Size = new System.Drawing.Size(1375, 1097);
            this.powerMeterControl1.TabIndex = 0;
            // 
            // tabPage19
            // 
            this.tabPage19.Controls.Add(this.powerMapControl1);
            this.tabPage19.Location = new System.Drawing.Point(4, 36);
            this.tabPage19.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage19.Name = "tabPage19";
            this.tabPage19.Size = new System.Drawing.Size(1375, 1097);
            this.tabPage19.TabIndex = 1;
            this.tabPage19.Text = "PowerMap";
            this.tabPage19.UseVisualStyleBackColor = true;
            // 
            // powerMapControl1
            // 
            this.powerMapControl1.BackColor = System.Drawing.SystemColors.Control;
            this.powerMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.powerMapControl1.Document = null;
            this.powerMapControl1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.powerMapControl1.Laser = null;
            this.powerMapControl1.Location = new System.Drawing.Point(0, 0);
            this.powerMapControl1.Margin = new System.Windows.Forms.Padding(0);
            this.powerMapControl1.Name = "powerMapControl1";
            this.powerMapControl1.PowerMeter = null;
            this.powerMapControl1.Rtc = null;
            this.powerMapControl1.Size = new System.Drawing.Size(1375, 1097);
            this.powerMapControl1.TabIndex = 0;
            // 
            // logControl1
            // 
            this.logControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.logControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logControl1.IsDetailLog = false;
            this.logControl1.Location = new System.Drawing.Point(0, 0);
            this.logControl1.Margin = new System.Windows.Forms.Padding(0);
            this.logControl1.Name = "logControl1";
            this.logControl1.Size = new System.Drawing.Size(1391, 268);
            this.logControl1.TabIndex = 0;
            // 
            // tlcRight
            // 
            this.tlcRight.Controls.Add(this.tabProperty);
            this.tlcRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlcRight.HotTrack = true;
            this.tlcRight.ImageList = this.imageList1;
            this.tlcRight.ItemSize = new System.Drawing.Size(67, 38);
            this.tlcRight.Location = new System.Drawing.Point(0, 0);
            this.tlcRight.Margin = new System.Windows.Forms.Padding(0);
            this.tlcRight.Name = "tlcRight";
            this.tlcRight.SelectedIndex = 0;
            this.tlcRight.Size = new System.Drawing.Size(460, 1455);
            this.tlcRight.TabIndex = 1;
            // 
            // tabProperty
            // 
            this.tabProperty.Controls.Add(this.propertyGridControl1);
            this.tabProperty.ImageKey = "Property.png";
            this.tabProperty.Location = new System.Drawing.Point(4, 42);
            this.tabProperty.Margin = new System.Windows.Forms.Padding(0);
            this.tabProperty.Name = "tabProperty";
            this.tabProperty.Size = new System.Drawing.Size(452, 1409);
            this.tabProperty.TabIndex = 0;
            this.tabProperty.Text = " Property ";
            this.tabProperty.UseVisualStyleBackColor = true;
            // 
            // propertyGridControl1
            // 
            this.propertyGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridControl1.Document = null;
            this.propertyGridControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGridControl1.Location = new System.Drawing.Point(0, 0);
            this.propertyGridControl1.Margin = new System.Windows.Forms.Padding(0);
            this.propertyGridControl1.Marker = null;
            this.propertyGridControl1.Name = "propertyGridControl1";
            this.propertyGridControl1.SelecteObject = null;
            this.propertyGridControl1.Size = new System.Drawing.Size(452, 1409);
            this.propertyGridControl1.TabIndex = 1;
            this.propertyGridControl1.View = null;
            // 
            // SiriusEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.tlsTop1);
            this.Controls.Add(this.stsBottom);
            this.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SiriusEditorControl";
            this.Size = new System.Drawing.Size(2303, 1530);
            this.stsBottom.ResumeLayout(false);
            this.stsBottom.PerformLayout();
            this.tlsTop1.ResumeLayout(false);
            this.tlsTop1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).EndInit();
            this.splitContainer6.ResumeLayout(false);
            this.splitContainer8.Panel1.ResumeLayout(false);
            this.splitContainer8.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).EndInit();
            this.splitContainer8.ResumeLayout(false);
            this.tbcLeft.ResumeLayout(false);
            this.tabDocPage1.ResumeLayout(false);
            this.tabDocPage2.ResumeLayout(false);
            this.tabBlockPage.ResumeLayout(false);
            this.tabWaferPage.ResumeLayout(false);
            this.tabSubstratePage.ResumeLayout(false);
            this.tlcPen.ResumeLayout(false);
            this.tabEntityPen.ResumeLayout(false);
            this.tabLayerPen.ResumeLayout(false);
            this.splitContainer7.Panel1.ResumeLayout(false);
            this.splitContainer7.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
            this.splitContainer7.ResumeLayout(false);
            this.tbcMain.ResumeLayout(false);
            this.tabMarker.ResumeLayout(false);
            this.splitContainer10.Panel1.ResumeLayout(false);
            this.splitContainer10.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer10)).EndInit();
            this.splitContainer10.ResumeLayout(false);
            this.tabManual.ResumeLayout(false);
            this.tabScanner.ResumeLayout(false);
            this.tabLaser.ResumeLayout(false);
            this.tabDIO.ResumeLayout(false);
            this.splitContainer9.Panel1.ResumeLayout(false);
            this.splitContainer9.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer9)).EndInit();
            this.splitContainer9.ResumeLayout(false);
            this.tabPower.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage18.ResumeLayout(false);
            this.tabPage19.ResumeLayout(false);
            this.tlcRight.ResumeLayout(false);
            this.tabProperty.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private StatusStrip stsBottom;
        private ToolStripStatusLabel lblName;
        private ToolStripStatusLabel lblProcessTime;
        private ToolStripStatusLabel lblPowerWatt;
        private ToolStripStatusLabel lblFileName;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel btnLogWindow;
        private ToolStripStatusLabel lblEncoder;
        private ToolStripStatusLabel lblReady;
        private ToolStripStatusLabel toolStripStatusLabel7;
        private ToolStripStatusLabel lblBusy;
        private ToolStripStatusLabel toolStripStatusLabel9;
        private ToolStripStatusLabel lblError;
        private ToolStripStatusLabel toolStripStatusLabel8;
        private ToolStrip tlsTop1;
        private ToolStripButton btnNew;
        private ToolStripButton btnOpen;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripDropDownButton ddbOpenNewOptions;
        private ToolStripMenuItem mnuIncludePage1;
        private ToolStripMenuItem mnuIncludePage2;
        private ToolStripMenuItem mnuIncludePage3;
        private ToolStripMenuItem mnuIncludePage4;
        private ToolStripMenuItem mnuIncludeBlocks;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mnuIncludeLayerPens;
        private ToolStripMenuItem mnuIncludeScannerPens;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem mnuIncludeWafers;
        private ToolStripMenuItem mnuIncludeSubstrates;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnSave;
        private ToolStripSeparator toolStripSeparator3;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer6;
        private SplitContainer splitContainer8;
        private TabControl tbcLeft;
        private TabPage tabDocPage1;
        private SpiralLab.Sirius3.UI.WinForms.TreeViewPageControl treeViewPageControl1;
        private TabPage tabDocPage2;
        private SpiralLab.Sirius3.UI.WinForms.TreeViewPageControl treeViewPageControl2;
        private TabPage tabBlockPage;
        private SpiralLab.Sirius3.UI.WinForms.TreeViewBlockControl treeViewBlockControl1;
        private TabPage tabWaferPage;
        private SpiralLab.Sirius3.UI.WinForms.TreeViewWaferControl treeViewWaferControl1;
        private TabPage tabSubstratePage;
        private SpiralLab.Sirius3.UI.WinForms.TreeViewSubstrateControl treeViewSubstrateControl1;
        private TabControl tlcPen;
        private TabPage tabEntityPen;
        private SpiralLab.Sirius3.UI.WinForms.EntityPenControl entityPenControl1;
        private TabPage tabLayerPen;
        private SpiralLab.Sirius3.UI.WinForms.LayerPenControl layerPenControl1;
        private SplitContainer splitContainer7;
        private TabControl tbcMain;
        private TabPage tabEditor;
        private TabPage tabMarker;
        private SplitContainer splitContainer10;
        private SpiralLab.Sirius3.UI.WinForms.OffsetControl offsetControl1;
        private SpiralLab.Sirius3.UI.WinForms.MarkerControl markerControl1;
        private TabPage tabManual;
        private SpiralLab.Sirius3.UI.WinForms.ManualControl manualControl1;
        private TabPage tabScanner;
        private SpiralLab.Sirius3.UI.WinForms.ScannerControl scannerControl1;
        private TabPage tabLaser;
        private SpiralLab.Sirius3.UI.WinForms.LaserControl laserControl1;
        private TabPage tabDIO;
        private SplitContainer splitContainer9;
        private SpiralLab.Sirius3.UI.WinForms.RtcDIControl rtcDIControl1;
        private SpiralLab.Sirius3.UI.WinForms.RtcDOControl rtcDOControl1;
        private TabPage tabPower;
        private TabControl tabControl2;
        private TabPage tabPage18;
        private SpiralLab.Sirius3.UI.WinForms.PowerMeterControl powerMeterControl1;
        private TabPage tabPage19;
        private SpiralLab.Sirius3.UI.WinForms.PowerMapControl powerMapControl1;
        private SpiralLab.Sirius3.UI.WinForms.LogControl logControl1;
        private TabControl tlcRight;
        private TabPage tabProperty;
        private SpiralLab.Sirius3.UI.WinForms.PropertyGridControl propertyGridControl1;
        private ImageList imageList1;
        private ToolStripButton btnLock;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripStatusLabel toolStripStatusLabel2;
    }
}
