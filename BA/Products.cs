using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace BA
{
    public partial class Products : Form
    {
        public Products(string connStr)
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

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - last.X;
                this.Top += e.Y - last.Y;
            }
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            conn.Close();
            Application.Exit();
        }

        private void loadData()
        {
            try
            {
                da = new SQLiteDataAdapter("SELECT *, 'Delete' AS [Действие]  FROM Products", conn);
                commBuild = new SQLiteCommandBuilder(da);
                commBuild.GetInsertCommand();
                commBuild.GetUpdateCommand();
                commBuild.GetDeleteCommand();
                dataSet = new DataSet();
                da.Fill(dataSet, "Products");
                dataGridView1.DataSource = dataSet.Tables["Products"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    dataGridView1[5, i] = cell;
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
                dataSet.Tables["Products"].Clear();
                da.Fill(dataSet, "Products");
                dataGridView1.DataSource = dataSet.Tables["Products"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    dataGridView1[5, i] = cell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Products_Load(object sender, EventArgs e)
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
                if (e.ColumnIndex == 5)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();

                    if (task == "Delete")
                    {
                        int rowIndex = e.RowIndex;
                        dataGridView1.Rows.RemoveAt(rowIndex);
                        dataSet.Tables["Products"].Rows[rowIndex].Delete();
                        da.Update(dataSet, "Products");
                    }
                    else if (task == "Add")
                    {
                        int rowIndex = dataGridView1.Rows.Count - 2;
                        DataRow row = dataSet.Tables["Products"].NewRow();
                        row["Продукт"] = dataGridView1.Rows[rowIndex].Cells["Продукт"].Value;
                        row["Цена_за_100_грамм"] = dataGridView1.Rows[rowIndex].Cells["Цена_за_100_грамм"].Value;
                        row["Срок_годности"] = dataGridView1.Rows[rowIndex].Cells["Срок_годности"].Value;
                        row["Примерный_остаток"] = dataGridView1.Rows[rowIndex].Cells["Примерный_остаток"].Value;
                        dataSet.Tables["Products"].Rows.Add(row);
                        dataSet.Tables["Products"].Rows.RemoveAt(dataSet.Tables["Products"].Rows.Count - 1);
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = "Delete";
                        da.Update(dataSet, "Products");
                        newRowAdd = false;
                    }
                    else if (task == "Update")
                    {
                        int rowIndex = e.RowIndex;
                        dataSet.Tables["Products"].Rows[rowIndex]["Продукт"] = dataGridView1.Rows[rowIndex].Cells["Продукт"].Value;
                        dataSet.Tables["Products"].Rows[rowIndex]["Цена_за_100_грамм"] = dataGridView1.Rows[rowIndex].Cells["Цена_за_100_грамм"].Value;
                        dataSet.Tables["Products"].Rows[rowIndex]["Срок_годности"] = dataGridView1.Rows[rowIndex].Cells["Срок_годности"].Value;
                        dataSet.Tables["Products"].Rows[rowIndex]["Примерный_остаток"] = dataGridView1.Rows[rowIndex].Cells["Примерный_остаток"].Value;
                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = "Delete";
                        da.Update(dataSet, "Products");
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
                dataGridView1[5, lastRow] = cell;
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
                    dataGridView1[5, rowIndex] = cell;
                    edRow.Cells["Действие"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            reLoadData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            conn.Close();
            Box f = new Box(connStr);
            f.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conn.Close();
            Main f = new Main(connStr);
            f.Show();
            this.Hide();
        }
    }
}
