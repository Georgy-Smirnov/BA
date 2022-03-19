using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;

namespace BA
{
    public partial class Box : Form
    {
        public Box(string connStr)
        {
            InitializeComponent();
            this.connStr = connStr;
        }

        Point last;
        SQLiteConnection conn;
        SQLiteCommandBuilder commBuild;
        SQLiteDataAdapter da;
        DataSet dataSet;
        bool newRowAdd;
        string connStr;

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - last.X;
                this.Top += e.Y - last.Y;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Point(e.X, e.Y);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            conn.Close();
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conn.Close();
            Products f = new Products(connStr);
            f.Show();
            this.Hide();
        }

        private void loadData()
        {
            try
            {
                da = new SQLiteDataAdapter("SELECT *, 'Delete' AS [Действие]  FROM Box", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                commBuild = new SQLiteCommandBuilder(da);
                commBuild.GetInsertCommand();
                commBuild.GetUpdateCommand();
                commBuild.GetDeleteCommand();
                dataSet = new DataSet();
                da.Fill(dataSet, "Box");
                dataGridView1.DataSource = dataSet.Tables["Box"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    dataGridView1[4, i] = cell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void reLoadData()
        {
            try
            {
                dataSet.Tables["Box"].Clear();
                da.Fill(dataSet, "Box");
                dataGridView1.DataSource = dataSet.Tables["Box"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    dataGridView1[4, i] = cell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Box_Load(object sender, EventArgs e)
        {
            dataGridView1.RowHeadersVisible = false;
         conn = new SQLiteConnection(connStr);
            conn.Open();
            loadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 4)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                    if (task == "Delete")
                    {
                        int rowIndex = e.RowIndex;
                        dataGridView1.Rows.RemoveAt(rowIndex);
                        dataSet.Tables["Box"].Rows[rowIndex].Delete();
                        da.Update(dataSet, "Box");
                    }
                    else if (task == "Add")
                    {
                        int rowIndex = dataGridView1.Rows.Count - 2;
                        DataRow row = dataSet.Tables["Box"].NewRow();
                        row["Название"] = dataGridView1.Rows[rowIndex].Cells["Название"].Value;
                        row["Цена_за_штуку"] = dataGridView1.Rows[rowIndex].Cells["Цена_за_штуку"].Value;
                        row["Заметка"] = dataGridView1.Rows[rowIndex].Cells["Заметка"].Value;
                        dataSet.Tables["Box"].Rows.Add(row);
                        dataSet.Tables["Box"].Rows.RemoveAt(dataSet.Tables["Box"].Rows.Count - 1);
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                        dataGridView1.Rows[e.RowIndex].Cells[4].Value = "Delete";
                        da.Update(dataSet, "Box");
                        newRowAdd = false;
                    }
                    else if (task == "Update")
                    {
                        int rowIndex = e.RowIndex;
                        dataSet.Tables["Box"].Rows[rowIndex]["Название"] = dataGridView1.Rows[rowIndex].Cells["Название"].Value;
                        dataSet.Tables["Box"].Rows[rowIndex]["Цена_за_штуку"] = dataGridView1.Rows[rowIndex].Cells["Цена_за_штку"].Value;
                        dataSet.Tables["Box"].Rows[rowIndex]["Заметка"] = dataGridView1.Rows[rowIndex].Cells["Заметка"].Value;
                        dataGridView1.Rows[e.RowIndex].Cells[4].Value = "Delete";
                        da.Update(dataSet, "Box");
                    }
                    reLoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAdd == false)
                    newRowAdd = true;
                int lastRow = dataGridView1.Rows.Count - 2;
                DataGridViewRow row = dataGridView1.Rows[lastRow];
                DataGridViewLinkCell cell = new DataGridViewLinkCell();
                dataGridView1[4, lastRow] = cell;
                row.Cells["Действие"].Value = "Add";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAdd == false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                    DataGridViewRow edRow = dataGridView1.Rows[rowIndex];
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    dataGridView1[4, rowIndex] = cell;
                    edRow.Cells["Действие"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
