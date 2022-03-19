using System.Drawing;
using System.Windows.Forms;

namespace BA
{
    public partial class Main : Form
    {
        public Main(string connStr)
        {
            InitializeComponent();
            this.connStr = connStr;
        }
        Point last;
        string connStr;

        private void BackGround_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - last.X;
                this.Top += e.Y - last.Y;
            }
        }

        private void BackGround_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Point(e.X, e.Y);
        }

        private void label1_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Calc f = new Calc(connStr);
            f.Show();
            this.Hide();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            Products f = new Products(connStr);
            f.Show();
            this.Hide();
        }
    }
}
