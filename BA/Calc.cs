using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BA
{
    public partial class Calc : Form
    {
        public Calc(string connStr)
        {
            InitializeComponent();
            this.connStr = connStr;
        }

        Point last;
        double total;
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

        private void label10_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main f = new Main(connStr);
            f.Show();
            this.Hide();
        }

        private void Calc_Load(object sender, EventArgs e)
        {
            DataTable table = new DataTable();
            SQLiteConnection conn = new SQLiteConnection(connStr);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Продукт FROM Products", conn);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(table);
            comboBox1.DataSource = table;
            comboBox1.DisplayMember = "Продукт";
            cmd = new SQLiteCommand("SELECT Название FROM Box", conn);
            da = new SQLiteDataAdapter(cmd);
            table = new DataTable();
            da.Fill(table);
            comboBox2.DataSource = table;
            comboBox2.DisplayMember = "Название";
            textBox1.Text = "100";
            textBox2.Text = "1";
            total = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string command = "SELECT Цена_за_100_грамм FROM Products WHERE Продукт = @pN";
            string productName = comboBox1.Text;
            SQLiteConnection conn = new SQLiteConnection(connStr);
            SQLiteCommand cmd = new SQLiteCommand(command, conn);
            SQLiteParameter pN = new SQLiteParameter("@pN", productName);
            cmd.Parameters.Add(pN);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataTable table = new DataTable();
            da.Fill(table);
            int price;
            double weigth;
            if (int.TryParse(table.Rows[0][0].ToString(), out price))
            {
                if (double.TryParse(textBox1.Text, out weigth))
                { 
                    listBox1.Items.Add(comboBox1.Text + " " + (weigth / 100).ToString() + " * " + price.ToString() + " =" + (weigth / 100 * price).ToString());
                    total += weigth / 100 * price;
                }
            }
            label8.Text = total.ToString() + " руб.";
            int count;
            int.TryParse(textBox2.Text, out count);
            if (count > 0)
            {
                label6.Text = (total / count).ToString() + " руб.";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                string[] a = listBox1.Text.Split('=');
                int price = Convert.ToInt32(a[1]);
                total -= price;
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
            label8.Text = total.ToString() + " руб.";
            int count;
            int.TryParse(textBox2.Text, out count);
            if (count > 0)
            {
                label6.Text = (total / count).ToString() + " руб.";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string command = "SELECT Цена_за_штуку FROM Box WHERE Название = @pN";
            string productName = comboBox2.Text;
            SQLiteConnection conn = new SQLiteConnection(connStr);
            SQLiteCommand cmd = new SQLiteCommand(command, conn);
            SQLiteParameter pN = new SQLiteParameter("@pN", productName);
            cmd.Parameters.Add(pN);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataTable table = new DataTable();
            da.Fill(table);
            int price;

            if(int.TryParse(table.Rows[0][0].ToString(), out price))
            { 
                listBox1.Items.Add(comboBox2.Text + " =" + price.ToString());
                total += price;
            }
            label8.Text = total.ToString() + " руб.";
            int count;
            int.TryParse(textBox2.Text, out count);
            if (count > 0)
            {
                label6.Text = (total / count).ToString() + " руб.";
            }
        }
    }
}
