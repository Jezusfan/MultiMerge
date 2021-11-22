using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace MultiMergeShared
{
    public class ColumnSorter : IComparer
    {
        private int m_dateColumn = -1;
        private int m_currentColumn;
        private bool m_reverse;
        private SortMethod m_sortMethod;

        public int Compare(object x, object y)
        {
            int num1;
            switch (this.SortMethod)
            {
                case SortMethod.Date:
                    num1 = DateTime.Compare((DateTime) ((ListViewItem) x).Tag, (DateTime) ((ListViewItem) y).Tag);
                    break;
                case SortMethod.Ordinal:
                    int num2 = int.Parse(((ListViewItem) x).SubItems[this.m_currentColumn].Text, (IFormatProvider) CultureInfo.CurrentCulture);
                    int num3 = int.Parse(((ListViewItem) y).SubItems[this.m_currentColumn].Text, (IFormatProvider) CultureInfo.CurrentCulture);
                    num1 = num2 <= num3 ? (num2 >= num3 ? 0 : -1) : 1;
                    break;
                default:
                    num1 = string.Compare(((ListViewItem) x).SubItems[this.m_currentColumn].Text, ((ListViewItem) y).SubItems[this.m_currentColumn].Text, StringComparison.CurrentCultureIgnoreCase);
                    break;
            }
            if (this.m_reverse)
                num1 *= -1;
            return num1;
        }

        public int DateColumn
        {
            get
            {
                return this.m_dateColumn;
            }
            set
            {
                this.m_dateColumn = value;
            }
        }

        public int CurrentColumn
        {
            get
            {
                return this.m_currentColumn;
            }
            set
            {
                this.m_currentColumn = value;
            }
        }

        public bool Reverse
        {
            get
            {
                return this.m_reverse;
            }
            set
            {
                this.m_reverse = value;
            }
        }

        public SortMethod SortMethod
        {
            get
            {
                return this.m_sortMethod;
            }
            set
            {
                this.m_sortMethod = value;
            }
        }

        
    }

    public enum SortMethod
    {
        String,
        Date,
        Ordinal,
    }
}
