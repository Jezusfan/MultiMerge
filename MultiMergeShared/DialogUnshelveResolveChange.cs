// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PowerTools.CommandLine.CommandUnshelve
// Assembly: TFPT, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A843CA-B1F0-495E-8F2B-DAE1FEAC9C4F
// Assembly location: D:\Temp\powertools\TFPT.EXE

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using MultiMerge;

namespace MultiMergeShared
{
    internal class DialogUnshelveResolveChange : BaseDialog
    {
        private IContainer components;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label labelShelvePath;
        private TextBox textBoxShelvedPath;
        private Button buttonCancel;
        private TableLayoutPanel tableLayoutPanel4;
        private RadioButton rbKeepLocal;
        private RadioButton rbTakeShelved;
        private RadioButton rbMerge;
        private Button buttonOK;
        private CheckBox checkBoxAutomerge;

        public DialogUnshelveResolveChange(MergeWorker.ShelvedItem item)
        {
            this.InitializeComponent();
            this.textBoxShelvedPath.Text = item.ShelvedChange.ServerItem;
            this.rbKeepLocal.Text = item.KeepLocalText;
            this.rbTakeShelved.Text = item.TakeShelvedText;
            if (item.MergeText != null)
            {
                this.rbMerge.Text = item.MergeText;
                this.rbMerge.Checked = true;
            }
            else
            {
                this.rbMerge.Visible = false;
                this.checkBoxAutomerge.Visible = false;
                this.rbTakeShelved.Checked = true;
            }
            if (item.UnshelveConflictState != MergeWorker.UnshelveConflictState.Unresolved || item.SameBranch)
                return;
            this.checkBoxAutomerge.Checked = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.rbKeepLocal.Checked)
                this.DialogResult = DialogResult.No;
            else if (this.rbTakeShelved.Checked)
                this.DialogResult = DialogResult.Yes;
            else if (this.rbMerge.Checked)
            {
                if (this.checkBoxAutomerge.Checked)
                    this.DialogResult = DialogResult.Retry;
                else
                    this.DialogResult = DialogResult.Ignore;
            }
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this.tableLayoutPanel2 = new TableLayoutPanel();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.checkBoxAutomerge = new CheckBox();
            this.tableLayoutPanel3 = new TableLayoutPanel();
            this.labelShelvePath = new Label();
            this.textBoxShelvedPath = new TextBox();
            this.tableLayoutPanel4 = new TableLayoutPanel();
            this.rbKeepLocal = new RadioButton();
            this.rbTakeShelved = new RadioButton();
            this.rbMerge = new RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            this.tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel1.Controls.Add((Control)this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add((Control)this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add((Control)this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel1.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.tableLayoutPanel1.Location = new Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
            this.tableLayoutPanel1.Size = new Size(563, 157);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.Controls.Add((Control)this.buttonOK, 1, 0);
            this.tableLayoutPanel2.Controls.Add((Control)this.buttonCancel, 2, 0);
            this.tableLayoutPanel2.Controls.Add((Control)this.checkBoxAutomerge, 0, 0);
            this.tableLayoutPanel2.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.tableLayoutPanel2.Location = new Point(3, 131);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel2.Size = new Size(557, 23);
            this.tableLayoutPanel2.TabIndex = 3;
            this.buttonOK.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.buttonOK.Location = new Point(336, 0);
            this.buttonOK.Margin = new Padding(3, 0, 3, 0);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Padding = new Padding(10, 0, 10, 0);
            this.buttonOK.Size = new Size(106, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new EventHandler(this.buttonOK_Click);
            this.buttonCancel.DialogResult = DialogResult.Cancel;
            this.buttonCancel.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.buttonCancel.Location = new Point(448, 0);
            this.buttonCancel.Margin = new Padding(3, 0, 3, 0);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Padding = new Padding(10, 0, 10, 0);
            this.buttonCancel.Size = new Size(106, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new EventHandler(this.buttonCancel_Click);
            this.checkBoxAutomerge.AutoSize = true;
            this.checkBoxAutomerge.Location = new Point(3, 3);
            this.checkBoxAutomerge.Name = "checkBoxAutomerge";
            this.checkBoxAutomerge.Size = new Size(266, 17);
            this.checkBoxAutomerge.TabIndex = 2;
            this.checkBoxAutomerge.Text = "Merge changes for me (automerge) when merging";
            this.checkBoxAutomerge.UseVisualStyleBackColor = true;
            this.tableLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel3.Controls.Add((Control)this.labelShelvePath, 0, 0);
            this.tableLayoutPanel3.Controls.Add((Control)this.textBoxShelvedPath, 1, 0);
            this.tableLayoutPanel3.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.tableLayoutPanel3.Location = new Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));
            this.tableLayoutPanel3.Size = new Size(557, 27);
            this.tableLayoutPanel3.TabIndex = 4;
            this.labelShelvePath.Anchor = AnchorStyles.Left;
            this.labelShelvePath.AutoSize = true;
            this.labelShelvePath.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.labelShelvePath.Location = new Point(3, 7);
            this.labelShelvePath.Name = "labelShelvePath";
            this.labelShelvePath.Size = new Size(74, 13);
            this.labelShelvePath.TabIndex = 0;
            this.labelShelvePath.Text = "Shelved Path:";
            this.textBoxShelvedPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.textBoxShelvedPath.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.textBoxShelvedPath.Location = new Point(83, 3);
            this.textBoxShelvedPath.Name = "textBoxShelvedPath";
            this.textBoxShelvedPath.Size = new Size(471, 21);
            this.textBoxShelvedPath.TabIndex = 2;
            this.tableLayoutPanel4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel4.Controls.Add((Control)this.rbKeepLocal, 0, 0);
            this.tableLayoutPanel4.Controls.Add((Control)this.rbTakeShelved, 0, 1);
            this.tableLayoutPanel4.Controls.Add((Control)this.rbMerge, 0, 2);
            this.tableLayoutPanel4.Location = new Point(3, 36);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel4.Size = new Size(557, 89);
            this.tableLayoutPanel4.TabIndex = 5;
            this.rbKeepLocal.AutoSize = true;
            this.rbKeepLocal.Location = new Point(3, 3);
            this.rbKeepLocal.Name = "rbKeepLocal";
            this.rbKeepLocal.Size = new Size(14, 13);
            this.rbKeepLocal.TabIndex = 0;
            this.rbKeepLocal.TabStop = true;
            this.rbKeepLocal.UseVisualStyleBackColor = true;
            this.rbTakeShelved.AutoSize = true;
            this.rbTakeShelved.Location = new Point(3, 22);
            this.rbTakeShelved.Name = "rbTakeShelved";
            this.rbTakeShelved.Size = new Size(14, 13);
            this.rbTakeShelved.TabIndex = 1;
            this.rbTakeShelved.TabStop = true;
            this.rbTakeShelved.UseVisualStyleBackColor = true;
            this.rbMerge.AutoSize = true;
            this.rbMerge.Location = new Point(3, 41);
            this.rbMerge.Name = "rbMerge";
            this.rbMerge.Size = new Size(14, 13);
            this.rbMerge.TabIndex = 2;
            this.rbMerge.TabStop = true;
            this.rbMerge.UseVisualStyleBackColor = true;
            this.AcceptButton = (IButtonControl)this.buttonOK;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = (IButtonControl)this.buttonCancel;
            this.ClientSize = new Size(585, 184);
            this.ControlBox = false;
            this.Controls.Add((Control)this.tableLayoutPanel1);
            this.MinimumSize = new Size(482, 200);
            this.Name = nameof(DialogUnshelveResolveChange);
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Resolve Unshelve Conflict";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
