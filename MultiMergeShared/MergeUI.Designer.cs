using System.Windows.Forms;

namespace MultiMerge
{
    partial class MergeUI
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeUI));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ContainingCommentText = new System.Windows.Forms.TextBox();
            this.ContainingCommentLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.ByUserText = new System.Windows.Forms.TextBox();
            this.ByUserLabel = new System.Windows.Forms.Label();
            this.OptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkIncludeRelevantChangesets = new System.Windows.Forms.CheckBox();
            this.chkKeepChangesets = new System.Windows.Forms.CheckBox();
            this.FindButton = new System.Windows.Forms.Button();
            this.chkMergeAllChanges = new System.Windows.Forms.CheckBox();
            this.chkMergeIntersectingFiles = new System.Windows.Forms.CheckBox();
            this.useRegularExpressions = new System.Windows.Forms.CheckBox();
            this.ToDatePicker = new System.Windows.Forms.DateTimePicker();
            this.ToDateLabel = new System.Windows.Forms.Label();
            this.FromDateLabel = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.txtBasePath = new System.Windows.Forms.TextBox();
            this.FromDatePicker = new System.Windows.Forms.DateTimePicker();
            this.CancelFindButton = new System.Windows.Forms.Button();
            this.ChangesetsAndFiles = new System.Windows.Forms.TabControl();
            this.ChangesetList = new System.Windows.Forms.TabPage();
            this.ResultsList = new System.Windows.Forms.ListView();
            this.ChangesetColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.OwnerColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DateColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CommentColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FileColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FileList = new System.Windows.Forms.TabPage();
            this.FilesList = new System.Windows.Forms.ListView();
            this.colFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.AnalyzeChangesetBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.MergeButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panBottom = new System.Windows.Forms.Panel();
            this.MergeBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.OptionsGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.ChangesetsAndFiles.SuspendLayout();
            this.ChangesetList.SuspendLayout();
            this.FileList.SuspendLayout();
            this.panBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContainingCommentText
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.ContainingCommentText, 2);
            this.ContainingCommentText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContainingCommentText.Location = new System.Drawing.Point(3, 123);
            this.ContainingCommentText.Name = "ContainingCommentText";
            this.ContainingCommentText.Size = new System.Drawing.Size(895, 20);
            this.ContainingCommentText.TabIndex = 9;
            // 
            // ContainingCommentLabel
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.ContainingCommentLabel, 2);
            this.ContainingCommentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContainingCommentLabel.Location = new System.Drawing.Point(3, 96);
            this.ContainingCommentLabel.Name = "ContainingCommentLabel";
            this.ContainingCommentLabel.Size = new System.Drawing.Size(895, 24);
            this.ContainingCommentLabel.TabIndex = 8;
            this.ContainingCommentLabel.Text = "Containing comment:";
            this.ContainingCommentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(804, 15);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(98, 29);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // ByUserText
            // 
            this.ByUserText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ByUserText.Location = new System.Drawing.Point(453, 27);
            this.ByUserText.Name = "ByUserText";
            this.ByUserText.Size = new System.Drawing.Size(445, 20);
            this.ByUserText.TabIndex = 3;
            // 
            // ByUserLabel
            // 
            this.ByUserLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ByUserLabel.Location = new System.Drawing.Point(453, 0);
            this.ByUserLabel.Name = "ByUserLabel";
            this.ByUserLabel.Size = new System.Drawing.Size(445, 24);
            this.ByUserLabel.TabIndex = 2;
            this.ByUserLabel.Text = "By user:";
            this.ByUserLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OptionsGroupBox
            // 
            this.OptionsGroupBox.Controls.Add(this.tableLayoutPanel1);
            this.OptionsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.OptionsGroupBox.Location = new System.Drawing.Point(3, 0);
            this.OptionsGroupBox.Name = "OptionsGroupBox";
            this.OptionsGroupBox.Size = new System.Drawing.Size(914, 202);
            this.OptionsGroupBox.TabIndex = 0;
            this.OptionsGroupBox.TabStop = false;
            this.OptionsGroupBox.Text = "Find options";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.ToDatePicker, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.ToDateLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.FromDateLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ContainingCommentText, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblPath, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ByUserLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ContainingCommentLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtBasePath, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ByUserText, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.FromDatePicker, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(901, 176);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel2, 2);
            this.panel2.Controls.Add(this.chkMergeIntersectingFiles);
            this.panel2.Controls.Add(this.chkIncludeRelevantChangesets);
            this.panel2.Controls.Add(this.chkKeepChangesets);
            this.panel2.Controls.Add(this.FindButton);
            this.panel2.Controls.Add(this.chkMergeAllChanges);
            this.panel2.Controls.Add(this.useRegularExpressions);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 147);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(895, 26);
            this.panel2.TabIndex = 0;
            // 
            // chkIncludeRelevantChangesets
            // 
            this.chkIncludeRelevantChangesets.AutoSize = true;
            this.chkIncludeRelevantChangesets.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkIncludeRelevantChangesets.Location = new System.Drawing.Point(466, 0);
            this.chkIncludeRelevantChangesets.Name = "chkIncludeRelevantChangesets";
            this.chkIncludeRelevantChangesets.Size = new System.Drawing.Size(161, 26);
            this.chkIncludeRelevantChangesets.TabIndex = 16;
            this.chkIncludeRelevantChangesets.Text = "Include relevant Changesets";
            this.chkIncludeRelevantChangesets.UseVisualStyleBackColor = true;
            // 
            // chkKeepChangesets
            // 
            this.chkKeepChangesets.AutoSize = true;
            this.chkKeepChangesets.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkKeepChangesets.Location = new System.Drawing.Point(284, 0);
            this.chkKeepChangesets.Name = "chkKeepChangesets";
            this.chkKeepChangesets.Size = new System.Drawing.Size(182, 26);
            this.chkKeepChangesets.TabIndex = 11;
            this.chkKeepChangesets.Text = "Keep previous found changesets";
            this.chkKeepChangesets.UseVisualStyleBackColor = true;
            // 
            // FindButton
            // 
            this.FindButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindButton.Location = new System.Drawing.Point(797, 0);
            this.FindButton.Name = "FindButton";
            this.FindButton.Size = new System.Drawing.Size(98, 26);
            this.FindButton.TabIndex = 14;
            this.FindButton.Text = "&Find";
            this.FindButton.UseVisualStyleBackColor = true;
            this.FindButton.Click += new System.EventHandler(this.FindButton_Click);
            // 
            // chkMergeAllChanges
            // 
            this.chkMergeAllChanges.AutoSize = true;
            this.chkMergeAllChanges.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkMergeAllChanges.Location = new System.Drawing.Point(138, 0);
            this.chkMergeAllChanges.Name = "chkMergeAllChanges";
            this.chkMergeAllChanges.Size = new System.Drawing.Size(146, 26);
            this.chkMergeAllChanges.TabIndex = 12;
            this.chkMergeAllChanges.Text = "Merge all changes to files";
            this.chkMergeAllChanges.UseVisualStyleBackColor = true;
            // 
            // chkMergeIntersectingFiles
            // 
            this.chkMergeIntersectingFiles.AutoSize = true;
            this.chkMergeIntersectingFiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkMergeIntersectingFiles.Location = new System.Drawing.Point(627, 0);
            this.chkMergeIntersectingFiles.Name = "chkMergeIntersectingFiles";
            this.chkMergeIntersectingFiles.Size = new System.Drawing.Size(134, 26);
            this.chkMergeIntersectingFiles.TabIndex = 13;
            this.chkMergeIntersectingFiles.Text = "Merge intersecting files";
            this.chkMergeIntersectingFiles.UseVisualStyleBackColor = true;
            // 
            // useRegularExpressions
            // 
            this.useRegularExpressions.AutoSize = true;
            this.useRegularExpressions.Dock = System.Windows.Forms.DockStyle.Left;
            this.useRegularExpressions.Location = new System.Drawing.Point(0, 0);
            this.useRegularExpressions.Name = "useRegularExpressions";
            this.useRegularExpressions.Size = new System.Drawing.Size(138, 26);
            this.useRegularExpressions.TabIndex = 11;
            this.useRegularExpressions.Text = "Use regular expressions";
            this.useRegularExpressions.UseVisualStyleBackColor = true;
            // 
            // ToDatePicker
            // 
            this.ToDatePicker.Checked = false;
            this.ToDatePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToDatePicker.Location = new System.Drawing.Point(453, 75);
            this.ToDatePicker.Name = "ToDatePicker";
            this.ToDatePicker.ShowCheckBox = true;
            this.ToDatePicker.Size = new System.Drawing.Size(445, 20);
            this.ToDatePicker.TabIndex = 7;
            // 
            // ToDateLabel
            // 
            this.ToDateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToDateLabel.Location = new System.Drawing.Point(453, 48);
            this.ToDateLabel.Name = "ToDateLabel";
            this.ToDateLabel.Size = new System.Drawing.Size(445, 24);
            this.ToDateLabel.TabIndex = 6;
            this.ToDateLabel.Text = "To date:";
            this.ToDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FromDateLabel
            // 
            this.FromDateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FromDateLabel.Location = new System.Drawing.Point(3, 48);
            this.FromDateLabel.Name = "FromDateLabel";
            this.FromDateLabel.Size = new System.Drawing.Size(444, 24);
            this.FromDateLabel.TabIndex = 4;
            this.FromDateLabel.Text = "From date:";
            this.FromDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPath
            // 
            this.lblPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPath.Location = new System.Drawing.Point(3, 0);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(444, 24);
            this.lblPath.TabIndex = 0;
            this.lblPath.Text = "Base path:";
            this.lblPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBasePath
            // 
            this.txtBasePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBasePath.Location = new System.Drawing.Point(3, 27);
            this.txtBasePath.Name = "txtBasePath";
            this.txtBasePath.Size = new System.Drawing.Size(444, 20);
            this.txtBasePath.TabIndex = 1;
            // 
            // FromDatePicker
            // 
            this.FromDatePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FromDatePicker.Location = new System.Drawing.Point(3, 75);
            this.FromDatePicker.Name = "FromDatePicker";
            this.FromDatePicker.ShowCheckBox = true;
            this.FromDatePicker.Size = new System.Drawing.Size(444, 20);
            this.FromDatePicker.TabIndex = 5;
            // 
            // CancelFindButton
            // 
            this.CancelFindButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelFindButton.Enabled = false;
            this.CancelFindButton.Location = new System.Drawing.Point(700, 15);
            this.CancelFindButton.Name = "CancelFindButton";
            this.CancelFindButton.Size = new System.Drawing.Size(98, 29);
            this.CancelFindButton.TabIndex = 15;
            this.CancelFindButton.Text = "C&ancel";
            this.CancelFindButton.UseVisualStyleBackColor = true;
            this.CancelFindButton.Click += new System.EventHandler(this.CancelFindButton_Click);
            // 
            // ChangesetsAndFiles
            // 
            this.ChangesetsAndFiles.Controls.Add(this.ChangesetList);
            this.ChangesetsAndFiles.Controls.Add(this.FileList);
            this.ChangesetsAndFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChangesetsAndFiles.Location = new System.Drawing.Point(3, 202);
            this.ChangesetsAndFiles.Name = "ChangesetsAndFiles";
            this.ChangesetsAndFiles.SelectedIndex = 0;
            this.ChangesetsAndFiles.Size = new System.Drawing.Size(914, 146);
            this.ChangesetsAndFiles.TabIndex = 1;
            // 
            // ChangesetList
            // 
            this.ChangesetList.Controls.Add(this.ResultsList);
            this.ChangesetList.Location = new System.Drawing.Point(4, 22);
            this.ChangesetList.Name = "ChangesetList";
            this.ChangesetList.Padding = new System.Windows.Forms.Padding(3);
            this.ChangesetList.Size = new System.Drawing.Size(906, 120);
            this.ChangesetList.TabIndex = 0;
            this.ChangesetList.Text = "Changesets";
            this.ChangesetList.UseVisualStyleBackColor = true;
            // 
            // ResultsList
            // 
            this.ResultsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ChangesetColumn,
            this.OwnerColumn,
            this.DateColumn,
            this.FileColumn,
            this.CommentColumn});
            this.ResultsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultsList.FullRowSelect = true;
            this.ResultsList.Location = new System.Drawing.Point(3, 3);
            this.ResultsList.Name = "ResultsList";
            this.ResultsList.Size = new System.Drawing.Size(900, 114);
            this.ResultsList.TabIndex = 3;
            this.ResultsList.UseCompatibleStateImageBehavior = false;
            this.ResultsList.View = System.Windows.Forms.View.Details;
            this.ResultsList.DoubleClick += new System.EventHandler(this.ResultsList_DoubleClick);
            this.ResultsList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ResultsList_KeyUp);
            // 
            // ChangesetColumn
            // 
            this.ChangesetColumn.Text = "Changeset";
            this.ChangesetColumn.Width = 80;
            // 
            // OwnerColumn
            // 
            this.OwnerColumn.Text = "Owner";
            this.OwnerColumn.Width = 120;
            // 
            // DateColumn
            // 
            this.DateColumn.Text = "Date";
            this.DateColumn.Width = 90;
            // 
            // CommentColumn
            // 
            this.CommentColumn.Text = "Comment";
            this.CommentColumn.Width = 390;
            // 
            // CommentColumn
            // 
            this.FileColumn.Text = "File Count";
            this.FileColumn.Width = 90;
            // 
            // FileList
            // 
            this.FileList.Controls.Add(this.FilesList);
            this.FileList.Location = new System.Drawing.Point(4, 22);
            this.FileList.Name = "FileList";
            this.FileList.Padding = new System.Windows.Forms.Padding(3);
            this.FileList.Size = new System.Drawing.Size(906, 120);
            this.FileList.TabIndex = 1;
            this.FileList.Text = "Files";
            this.FileList.UseVisualStyleBackColor = true;
            // 
            // FilesList
            // 
            this.FilesList.CheckBoxes = true;
            this.FilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFile});
            this.FilesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilesList.FullRowSelect = true;
            this.FilesList.Location = new System.Drawing.Point(3, 3);
            this.FilesList.MultiSelect = false;
            this.FilesList.Name = "FilesList";
            this.FilesList.ShowItemToolTips = true;
            this.FilesList.Size = new System.Drawing.Size(900, 114);
            this.FilesList.SmallImageList = this.imageList1;
            this.FilesList.TabIndex = 0;
            this.FilesList.UseCompatibleStateImageBehavior = false;
            this.FilesList.View = System.Windows.Forms.View.Details;
            this.FilesList.DoubleClick += new System.EventHandler(this.FilesList_DoubleClick);
            this.FilesList.ItemChecked += this.FilesList_ItemChecked;

            // 
            // colFile
            // 
            this.colFile.Text = "";
            this.colFile.Width = 833;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "OK_16.png");
            this.imageList1.Images.SetKeyName(1, "Notification_Teal_16.png");
            // 
            // AnalyzeChangesetBackgroundWorker
            // 
            this.AnalyzeChangesetBackgroundWorker.WorkerReportsProgress = true;
            this.AnalyzeChangesetBackgroundWorker.WorkerSupportsCancellation = true;
            this.AnalyzeChangesetBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.AnalyzeChangesetBackgroundWorker_DoWork);
            this.AnalyzeChangesetBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.FindChangesetsWorker_ProgressChanged);
            this.AnalyzeChangesetBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FindChangesetsWorker_RunWorkerCompleted);
            // 
            // MergeButton
            // 
            this.MergeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MergeButton.Enabled = false;
            this.MergeButton.Location = new System.Drawing.Point(12, 15);
            this.MergeButton.Name = "MergeButton";
            this.MergeButton.Size = new System.Drawing.Size(98, 29);
            this.MergeButton.TabIndex = 2;
            this.MergeButton.Text = "&Merge";
            this.MergeButton.UseVisualStyleBackColor = true;
            this.MergeButton.Click += new System.EventHandler(this.MergeButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(526, 19);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(168, 23);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(116, 23);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "lblStatus";
            // 
            // panBottom
            // 
            this.panBottom.Controls.Add(this.lblStatus);
            this.panBottom.Controls.Add(this.CloseButton);
            this.panBottom.Controls.Add(this.progressBar1);
            this.panBottom.Controls.Add(this.CancelFindButton);
            this.panBottom.Controls.Add(this.MergeButton);
            this.panBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panBottom.Location = new System.Drawing.Point(3, 348);
            this.panBottom.Name = "panBottom";
            this.panBottom.Size = new System.Drawing.Size(914, 57);
            this.panBottom.TabIndex = 6;
            // 
            // MergeBackgroundWorker
            // 
            this.MergeBackgroundWorker.WorkerReportsProgress = true;
            this.MergeBackgroundWorker.WorkerSupportsCancellation = true;
            this.MergeBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.MergeBackgroundWorker_DoWork);
            this.MergeBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.MergeBackgroundWorker_ProgressChanged);
            this.MergeBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.MergeBackgroundWorker_RunWorkerCompleted);
            // 
            // MergeUI
            // 
            this.AcceptButton = this.FindButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(920, 405);
            this.Controls.Add(this.ChangesetsAndFiles);
            this.Controls.Add(this.panBottom);
            this.Controls.Add(this.OptionsGroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MergeUI";
            this.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multi Merge";
            this.OptionsGroupBox.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ChangesetsAndFiles.ResumeLayout(false);
            this.ChangesetList.ResumeLayout(false);
            this.FileList.ResumeLayout(false);
            this.panBottom.ResumeLayout(false);
            this.panBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.TextBox ContainingCommentText;
        protected System.Windows.Forms.Label ContainingCommentLabel;
        protected System.Windows.Forms.Button CloseButton;
        protected System.Windows.Forms.TextBox ByUserText;
        protected System.Windows.Forms.Label ByUserLabel;
        protected System.Windows.Forms.GroupBox OptionsGroupBox;
        protected System.Windows.Forms.TextBox txtBasePath;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        protected System.Windows.Forms.DateTimePicker ToDatePicker;
        protected System.Windows.Forms.Label ToDateLabel;
        protected System.Windows.Forms.Label FromDateLabel;
        protected System.Windows.Forms.Label lblPath;
        protected System.Windows.Forms.DateTimePicker FromDatePicker;
        protected System.Windows.Forms.TabControl ChangesetsAndFiles;
        protected System.Windows.Forms.TabPage ChangesetList;
        protected System.Windows.Forms.ListView ResultsList;
        protected System.Windows.Forms.ColumnHeader ChangesetColumn;
        protected System.Windows.Forms.ColumnHeader OwnerColumn;
        protected System.Windows.Forms.ColumnHeader DateColumn;
        protected System.Windows.Forms.ColumnHeader CommentColumn;
        protected System.Windows.Forms.ColumnHeader FileColumn;
        protected System.Windows.Forms.TabPage FileList;
        protected System.Windows.Forms.ListView FilesList;
        protected System.ComponentModel.BackgroundWorker AnalyzeChangesetBackgroundWorker;
        protected System.Windows.Forms.Button CancelFindButton;
        protected System.Windows.Forms.Button FindButton;
        protected System.Windows.Forms.Button MergeButton;
        protected System.Windows.Forms.Panel panel2;
        protected System.Windows.Forms.CheckBox chkKeepChangesets;
        protected System.Windows.Forms.CheckBox useRegularExpressions;
        protected System.Windows.Forms.ProgressBar progressBar1;
        protected System.Windows.Forms.Label lblStatus;
        protected System.Windows.Forms.CheckBox chkMergeAllChanges;
        protected System.Windows.Forms.CheckBox chkMergeIntersectingFiles;
        protected System.Windows.Forms.ImageList imageList1;
        protected System.Windows.Forms.ColumnHeader colFile;
        protected System.Windows.Forms.CheckBox chkIncludeRelevantChangesets;
        protected System.Windows.Forms.Panel panBottom;
        protected System.ComponentModel.BackgroundWorker MergeBackgroundWorker;
        protected System.Windows.Forms.ToolTip toolTip1;
    }
}
