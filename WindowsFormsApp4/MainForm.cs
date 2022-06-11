using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Load += Form1_Load;
            dgv.EditingControlShowing += onEditingControlShowing;
            dgv.CellBeginEdit += onCellBeginEdit;
            dgv.CellEndEdit += onCellEndEdit;
            dgv.CurrentCellChanged += onCurrentCellChanged;
            dgv.CurrentCellDirtyStateChanged += onCurrentCellDirtyStateChanged;
        }

        private void onCurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"{nameof(onCurrentCellDirtyStateChanged)} {dgv.IsCurrentCellDirty} {dgv.IsCurrentCellInEditMode}");
        }

        private void onCurrentCellChanged(object sender, EventArgs e)
        {
            if(dgv.CurrentCell == null)
            {
                Debug.WriteLine($"{nameof(onCurrentCellChanged)} null");
            }
            else
            {
                Debug.WriteLine($"{nameof(onCurrentCellChanged)} [{dgv.CurrentCell.ColumnIndex}, {dgv.CurrentCell.RowIndex}]");
            }
        }

        private void onCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Debug.WriteLine(nameof(onCellBeginEdit));
            if((dgv.CurrentCell != null) && (_cbEdit != null))
            {
                // dgv.CurrentCell.Value = _cbEdit.Text;
            }
        }

        private void onCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine(nameof(onCellEndEdit));
        }

        private void onEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if(e.Control is DataGridViewComboBoxEditingControl cbEdit)
            {
                CBEdit = cbEdit; 
            }
        }
        DataGridViewComboBoxEditingControl _cbEdit = null;
        public DataGridViewComboBoxEditingControl CBEdit
        {
            get => _cbEdit;
            set
            {
                if(!ReferenceEquals(_cbEdit, value))
                {
                    if(_cbEdit != null)
                    {
                        _cbEdit.GotFocus -= onCBEdit_GotFocus;
                        _cbEdit.LostFocus -= onCBEdit_LostFocus;
                    }
                    _cbEdit = value;
                    if (_cbEdit != null)
                    {
                        _cbEdit.GotFocus += onCBEdit_GotFocus;
                        _cbEdit.LostFocus += onCBEdit_LostFocus;
                    }
                }
            }
        }

        private void onCBEdit_GotFocus(object sender, EventArgs e)
        {
            Debug.WriteLine($"CBEdit got focus with text='{_cbEdit.Text}'");
        }

        private void onCBEdit_LostFocus(object sender, EventArgs e)
        {
            Debug.WriteLine($"CBEdit losing focus with text='{_cbEdit.Text}'");
        }

        DataTable dtString1;
        DataTable dtString2;
        DataTable dtString3;

        private void Form1_Load(object sender, EventArgs e)
        {
            // create three combobox columns and put them side-by-side:
            // first column:
            DataGridViewComboBoxColumn dgvcbc1 = new DataGridViewComboBoxColumn();
            dgvcbc1.DataPropertyName = "String1";
            dgvcbc1.Name = "String1";

            dtString1 = new DataTable("String1Options");
            dtString1.Columns.Add("String1Long", typeof(string));

            dtString1.Rows.Add("apple");
            dtString1.Rows.Add("bob");
            dtString1.Rows.Add("clobber");
            dtString1.Rows.Add("dilbert");
            dtString1.Rows.Add("ether");

            dgv.Columns.Insert(0, dgvcbc1);

            dgvcbc1.DisplayMember = dtString1.Columns[0].ColumnName;
            dgvcbc1.ValueMember = dtString1.Columns[0].ColumnName;
            dgvcbc1.DataSource = dtString1;

            dgvcbc1.FlatStyle = FlatStyle.Flat;

            // create the second column:
            DataGridViewComboBoxColumn dgvcbc2 = new DataGridViewComboBoxColumn();
            dgvcbc2.DataPropertyName = "String2";
            dgvcbc2.Name = "String2";

            dtString2 = new DataTable("String2Options");
            dtString2.Columns.Add("String2Long", typeof(string));

            dtString2.Rows.Add("apple");
            dtString2.Rows.Add("bob");
            dtString2.Rows.Add("clobber");
            dtString2.Rows.Add("dilbert");
            dtString2.Rows.Add("ether");

            dgv.Columns.Insert(1, dgvcbc2);

            dgvcbc2.DisplayMember = dtString2.Columns[0].ColumnName;
            dgvcbc2.ValueMember = dtString2.Columns[0].ColumnName;
            dgvcbc2.DataSource = dtString2;

            dgvcbc2.FlatStyle = FlatStyle.Flat;

            // create the third column:
            DataGridViewComboBoxColumn dgvcbc3 = new DataGridViewComboBoxColumn();
            dgvcbc3.DataPropertyName = "String3";
            dgvcbc3.Name = "String3";

            dtString3 = new DataTable("String3Options");
            dtString3.Columns.Add("String3Long", typeof(string));

            dtString3.Rows.Add("apple");
            dtString3.Rows.Add("bob");
            dtString3.Rows.Add("clobber");
            dtString3.Rows.Add("dilbert");
            dtString3.Rows.Add("ether");

            dgv.Columns.Insert(2, dgvcbc3);

            dgvcbc3.DisplayMember = dtString3.Columns[0].ColumnName;
            dgvcbc3.ValueMember = dtString3.Columns[0].ColumnName;
            dgvcbc3.DataSource = dtString3;

            dgvcbc3.FlatStyle = FlatStyle.Flat;
        }
    }
}
