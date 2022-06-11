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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
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

#if false
            // ALSO SEEMS TO WORK IF PLACED HERE
            if((dgv.CurrentCell != null) && (_cbEdit != null))
            {
                dgv.CurrentCell.Value = _cbEdit.Text;
            }
#endif
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
            if (btnWorkaround.Checked)
            {
                if ((dgv.CurrentCell != null) && (_cbEdit != null))
                {
                    dgv.CurrentCell.Value = _cbEdit.Text;
                }
            }
        }

        private void onCBEdit_LostFocus(object sender, EventArgs e)
        {
            Debug.WriteLine($"CBEdit losing focus with text='{_cbEdit.Text}'");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // create three combobox columns and put them side-by-side:
            // first column:
            var c = new DataGridViewComboBoxColumn();
            c.DataPropertyName = "String1";
            c.Name = "String1";

            var dtString1 = new DataTable("String1Options");
            dtString1.Columns.Add("String1Long", typeof(string));

            dtString1.Rows.Add("apple");
            dtString1.Rows.Add("bob");
            dtString1.Rows.Add("clobber");
            dtString1.Rows.Add("dilbert");
            dtString1.Rows.Add("ether");

            dgv.Columns.Insert(0, c);

            c.DisplayMember = dtString1.Columns[0].ColumnName;
            c.ValueMember = dtString1.Columns[0].ColumnName;
            c.DataSource = dtString1;

            c.FlatStyle = FlatStyle.Flat;

            // create the second column:
            c = new DataGridViewComboBoxColumn();
            c.DataPropertyName = "String2";
            c.Name = "String2";

            var dtString2 = new DataTable("String2Options");
            dtString2.Columns.Add("String2Long", typeof(string));

            dtString2.Rows.Add("apple");
            dtString2.Rows.Add("bob");
            dtString2.Rows.Add("clobber");
            dtString2.Rows.Add("dilbert");
            dtString2.Rows.Add("ether");

            dgv.Columns.Insert(1, c);

            c.DisplayMember = dtString2.Columns[0].ColumnName;
            c.ValueMember = dtString2.Columns[0].ColumnName;
            c.DataSource = dtString2;

            c.FlatStyle = FlatStyle.Flat;

            // create the third column:
            c = new DataGridViewComboBoxColumn();
            c.DataPropertyName = "String3";
            c.Name = "String3";

            var dtString3 = new DataTable("String3Options");
            dtString3.Columns.Add("String3Long", typeof(string));

            dtString3.Rows.Add("apple");
            dtString3.Rows.Add("bob");
            dtString3.Rows.Add("clobber");
            dtString3.Rows.Add("dilbert");
            dtString3.Rows.Add("ether");

            dgv.Columns.Insert(2, c);

            c.DisplayMember = dtString3.Columns[0].ColumnName;
            c.ValueMember = dtString3.Columns[0].ColumnName;
            c.DataSource = dtString3;

            c.FlatStyle = FlatStyle.Flat;
        }

        public void SendKeyPlusTab(string keys)
        {
            var nRowsB4 = dgv.Rows.Count;
            if (!dgv.Focused)
            {
                dgv.Focus();
                Task.Delay(100).Wait();
            }
            foreach (var key in keys)
            {
                SendKeys.SendWait($"{key}\t");
            }
            if(dgv.Rows.Count == nRowsB4)
            {
                MessageBox.Show(msg);
            }
        }
        const string msg =
@"New bug detected!

The DGV should make a row as soon as a cell 
goes into edit mode. This only seems to
occur when repeating the same button.
        
Alternating the ABC and CDE buttons seems
to work always.";

        private void buttonABC_Click(object sender, EventArgs e)
        {
            SendKeyPlusTab("abc");
        }

        private void buttonCDE_Click(object sender, EventArgs e)
        {
            SendKeyPlusTab("cde");
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            dgv.Rows.Clear();
        }

        private void btnWorkaround_CheckedChanged(object sender, EventArgs e)
        {
            if(btnWorkaround.Checked)
            {
                btnWorkaround.Text = "Workaround is ON";
            }
            else
            {
                btnWorkaround.Text = "Workaround is OFF";
            }
        }
    }

#if false
    // N O R M A L    T R A C E
    onCurrentCellChanged [0, 0]
    onCellBeginEdit
    CBEdit got focus with text='apple'
    onCurrentCellDirtyStateChanged True True
    onCurrentCellDirtyStateChanged False True
    CBEdit losing focus with text='apple'
    onCellEndEdit
    onCurrentCellChanged [1, 0]
    onCellBeginEdit
    CBEdit got focus with text='apple'
    onCurrentCellDirtyStateChanged True True
    onCurrentCellDirtyStateChanged False True
    CBEdit losing focus with text='bob'
    onCellEndEdit
    onCurrentCellChanged [2, 0]
    onCellBeginEdit
    CBEdit got focus with text='apple'
    onCurrentCellDirtyStateChanged True True
    onCurrentCellDirtyStateChanged False True
    CBEdit losing focus with text='clobber'
    onCellEndEdit
    onCurrentCellChanged [0, 1]

    // P A T H O L O G I C A L    T R A C E
    onCellBeginEdit
    CBEdit got focus with text='apple'
    onCurrentCellDirtyStateChanged True True
    onCurrentCellDirtyStateChanged False True
    CBEdit losing focus with text='apple'
    onCellEndEdit
    onCurrentCellChanged [1, 1]
    onCellBeginEdit
    CBEdit got focus with text='bob'
    CBEdit losing focus with text='bob'
    onCellEndEdit
    onCurrentCellChanged [2, 1]
    onCellBeginEdit
    CBEdit got focus with text='clobber'
    CBEdit losing focus with text='clobber'
    onCellEndEdit
    onCurrentCellChanged [0, 2]
#endif
}
