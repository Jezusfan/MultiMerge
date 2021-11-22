using System.Collections.Generic;
using System.Windows.Forms;

namespace MultiMerge
{
    public partial class InputDialog : Form
    {
        private InputDialog()
        {
            InitializeComponent();
        }

        public static DialogResult Show(string text, string prompt, List<string> paths, ref string input)
        {
            InputDialog dialog = new InputDialog();
            dialog.lblText.Text = text;
            dialog.textBox1.Text = input;
            if (paths == null || paths.Count == 0)
                dialog.cboChoices.Visible = false;
            else
            {
                dialog.cboChoices.Items.AddRange(paths.ToArray());
                dialog.cboChoices.SelectedIndex = 0;
            }
            if (!string.IsNullOrEmpty(prompt))
                dialog.Text = prompt;
            var result = dialog.ShowDialog();
            input = dialog.textBox1.Text;
            return result;
        }

        public static DialogResult Show(string text, string prompt, ref string input)
        {
            return Show(text, prompt, new List<string>(), ref input);
        }

        private void cboChoices_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            textBox1.Text = cboChoices.Text;
        }

        private void cboChoices_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1.Text = cboChoices.Text;
        }
    }
}
