using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiMerge
{
    public partial class MessageBox : Form
    {
        public MessageBox()
        {
            InitializeComponent();
        }
        public static DialogResult Show(string message, MessageBoxButtons buttons = MessageBoxButtons.OK, bool dialog = true)
        {
            return Show(new[] {message}, null, buttons, dialog);
        }

        public static DialogResult Show(string[] messages, MessageBoxButtons buttons = MessageBoxButtons.OK, bool dialog = true)
        {
            return Show(messages, null, buttons, dialog);
        }

        public static DialogResult Show(string message, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, bool dialog = true)
        {
            return Show(new[] {message}, title, buttons, dialog);
        }

        public static DialogResult Show(string[] messages, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, bool dialog = true)
        {
            return Show(messages, title, null, buttons, dialog);
        }

        public static DialogResult Show(string message, string title, MessageBoxIcon? information, MessageBoxButtons buttons = MessageBoxButtons.OK, bool dialog = true)
        {
            return Show(new[] {message}, title, information, buttons, dialog);
        }

        public static DialogResult Show(string[] messages, string title, MessageBoxIcon? information, MessageBoxButtons buttons = MessageBoxButtons.OK, bool dialog = true)
        {
            var frm = new MessageBox();
            frm.lblText.Left = frm.pictureBox1.Left;
            frm.lblText.Text = Environment.NewLine + string.Join(Environment.NewLine, messages);
            if (buttons == MessageBoxButtons.YesNo)
            {
                frm.btnOk.Left -= (frm.btnNo.Width + 10);
                frm.btnOk.Text = "Yes";
                frm.btnNo.Visible = true;
            }
            if (title != null)
            {
                frm.Text = title;
            }
            if (information != null)
            {
                frm.lblText.Padding = new Padding(66,0,0,0);
                frm.pictureBox1.Visible = true;
                frm.pictureBox1.Image = Convert(information.Value).ToBitmap();
            }

            if (frm.btnOk.Top < frm.lblText.Top + frm.lblText.Height)
            {
                int diff = Math.Min(Screen.PrimaryScreen.WorkingArea.Height - (frm.Top+ frm.Height), frm.lblText.Top + frm.lblText.Height - frm.btnOk.Top);
                frm.Height += diff;
            }
            if (dialog)
                return frm.ShowDialog();

            frm.Show();
            while (frm.Visible)
            {
                Application.DoEvents();
            }
            return frm.DialogResult;
        }

        public static Icon Convert(MessageBoxIcon parameter)
        {
            Icon icon = (Icon)typeof(SystemIcons).GetProperty(parameter.ToString(), BindingFlags.Public | BindingFlags.Static).GetValue(null, null);
            return icon;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (btnNo.Visible)
                this.DialogResult = DialogResult.Yes;
            Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            Close();
        }
    }
}
