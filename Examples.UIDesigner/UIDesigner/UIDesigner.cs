using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Examples.UIDesigner.UIDesigner
{
    public partial class UIDesigner : Form
    {
        /// <summary>
        /// Internal data list
        /// </summary>
        private BindingList<UData> DataList { get; set; }
        /// <summary>
        /// Resulting data after this control has been closed
        /// </summary>
        public List<UIDesignerComponent.MyTuple<Texture2D, Rectangle, string>> ResultList { get; set; }
        /// <summary>
        /// Content manager link
        /// </summary>
        private readonly ContentManager _content;

        /// <summary>
        /// Create UIDesigner helper window
        /// </summary>
        /// <param name="content">MonoGame content manager</param>
        /// <param name="list">Data list</param>
        public UIDesigner(ContentManager content, List<UIDesignerComponent.MyTuple<Texture2D, Rectangle, string>> list = null)
        {
            InitializeComponent();
            DataList = new BindingList<UData>();
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = false;
            _content = content;

            UpdateData(list);

            but_remove.Click += but_remove_Click;
            but_add.Click += but_add_Click;
            but_close.Click += but_close_Click;
            but_gencode.Click += but_gencode_Click;
            but_cancel.Click += but_cancel_Click;
            but_copytex.Click += but_copytex_Click;
            but_up.Click += but_up_Click;
            but_down.Click += but_down_Click;
            dgv.KeyDown += dgv_KeyDown;
        }

        void but_down_Click(object sender, EventArgs e)
        {
            if (DataList.Count == 0 || dgv.SelectedRows.Count == 0) return;
            var data = dgv.SelectedRows[0].DataBoundItem as UData;
            var oldIndex = DataList.IndexOf(data);
            if (oldIndex == DataList.Count-1) return;
            DataList.RemoveAt(oldIndex);
            DataList.Insert(oldIndex + 1, data);
            dgv.ClearSelection();
            dgv.Rows[oldIndex + 1].Selected = true;
        }

        void but_up_Click(object sender, EventArgs e)
        {
            if (DataList.Count == 0 || dgv.SelectedRows.Count == 0) return;
            var data = dgv.SelectedRows[0].DataBoundItem as UData;
            var oldIndex = DataList.IndexOf(data);
            if (oldIndex == 0) return;
            DataList.RemoveAt(oldIndex);
            DataList.Insert(oldIndex - 1, data);
            dgv.ClearSelection();
            dgv.Rows[oldIndex - 1].Selected = true;
        }

        /// <summary>
        /// Update data inside this control
        /// </summary>
        /// <param name="list">Data list</param>
        public void UpdateData(List<UIDesignerComponent.MyTuple<Texture2D, Rectangle, string>> list)
        {
            if (list == null) return;
            DataList.Clear();
            foreach (var item in list)
                DataList.Add(new UData { Name = item.Item3, StoredRectangle = item.Item2, StoredTexture = item.Item1 });
            dgv.DataSource = DataList;
        }

        void but_copytex_Click(object sender, EventArgs e)
        {
            if (DataList.Count == 0 || dgv.SelectedRows.Count == 0) return;
            foreach (var item in dgv.SelectedRows)
            {
                var dataGridViewRow = item as DataGridViewRow;
                if (dataGridViewRow != null) {
                    var data = dataGridViewRow.DataBoundItem as UData;
                    if (data != null) DataList.Add(data.Clone());
                }
            }
            dgv.DataSource = DataList;
        }

        void but_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab && (dgv.CurrentCell.ColumnIndex == 0 || dgv.CurrentCell.ColumnIndex == 2))
            {
                e.Handled = true;
                DataGridViewCell cell = dgv.Rows[0].Cells[0];
                dgv.CurrentCell = cell;
                dgv.BeginEdit(true);
            }
        }

        void but_gencode_Click(object sender, EventArgs e)
        {
            var text = DataList.Aggregate("", (current, item) => current + string.Format("//{4}({5})\nnew Rectangle({0},{1},{2},{3})\n", item.StoredRectangle.X, item.StoredRectangle.Y, item.StoredRectangle.Width, item.StoredRectangle.Height, item.Name, item.Texture));
            Clipboard.SetText(text);
            MessageBox.Show("Generated code successfuly copied to your clipboard!");
        }

        void but_close_Click(object sender, EventArgs e)
        {
            ResultList = new List<UIDesignerComponent.MyTuple<Texture2D, Rectangle, string>>();
            foreach (var item in DataList)
                ResultList.Add(new UIDesignerComponent.MyTuple<Texture2D, Rectangle, string>(item.StoredTexture, item.StoredRectangle, item.Name));
            DialogResult = DialogResult.OK;
        }

        void but_add_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog { CheckFileExists = true, Filter="Supported Images|*.png;*.jpg;*.bmp;*.xnb", Multiselect = true, RestoreDirectory = true, Title = "Selec images to add as textures" };
            dlg.ShowDialog();
            foreach (var item in dlg.FileNames)
            {

                var tex = _content.Load<Texture2D>(item.EndsWith(".xnb") ? item.Substring(0, item.Length - 4) : item);
                var name = Path.GetFileName(tex.Name);
                var t = new UData { Name = name, StoredRectangle = new Rectangle(0,0, tex.Width, tex.Height), StoredTexture = tex };
                DataList.Add(t);
            }
        }


        void but_remove_Click(object sender, EventArgs e)
        {
            if (DataList.Count == 0 || dgv.SelectedRows.Count == 0) return;
            foreach (var item in dgv.SelectedRows)
                if (item != null) dgv.Rows.Remove(item as DataGridViewRow);
        }

        class UData : IDisposable
        {
            public string Name { get; set; }
            public string Texture { get { return StoredTexture.Name; } }
/*
            public string Rectangle 
            {
                get { return string.Format("{0},{1},{2},{3}", StoredRectangle.X, StoredRectangle.Y, StoredRectangle.Width, StoredRectangle.Height); }
                set 
                {
                    var lst = value.Split(new char[] { ',' });
                    if (lst.Length != 4)
                    {
                        MessageBox.Show("Number of params must be equal to 4!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    try
                    {
                        StoredRectangle.X = Convert.ToInt32(lst[0]);
                        StoredRectangle.Y = Convert.ToInt32(lst[1]);
                        StoredRectangle.Width = Convert.ToInt32(lst[2]);
                        StoredRectangle.Height = Convert.ToInt32(lst[3]);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid string format!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
*/
            public Texture2D StoredTexture;
            public Rectangle StoredRectangle;

            public void Dispose()
            {
                StoredTexture = null;
            }

            public UData Clone()
            {
                return (UData)MemberwiseClone();
            }

        }
    }
}
