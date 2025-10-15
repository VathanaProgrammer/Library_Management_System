using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Libray_Mnagement_Systemm
{
    public partial class ReportForm : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";

        public ReportForm()
        {
            InitializeComponent();
        }
        public void DisplayReport()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM DisplayReports()", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    setColumn();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void setColumn()
        {
            dataGridView1.Columns["Borrow ID"].Width = 120;
            dataGridView1.Columns["User ID"].Width = 120;
            dataGridView1.Columns["Book ID"].Width = 120;
            dataGridView1.Columns["Staff ID"].Width = 150;
            dataGridView1.Columns["Staff Name"].Width = 150;
            dataGridView1.Columns["Quantity"].Width = 80;
            dataGridView1.Columns["Borrow Date"].Width = 155;
            dataGridView1.Columns["Return Date"].Width = 156;
            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(11, 95, 155);
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.ForeColor = Color.White;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);



            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 0, 64); ;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }
        public void SetUserInfo(string username, int ID, System.Drawing.Image userImage)
        {
            lbluername.Text = username;
            lblId.Text = ID.ToString();
            MakePictureBoxCircular(pictureBox2);
            if(userImage != null)
            {
                pictureBox2.Image = userImage;
            }
            else
            {
                pictureBox2.Image = null;
            }
        }
        private void MakePictureBoxCircular(PictureBox pictureBox)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pictureBox.Width, pictureBox.Height);
            pictureBox.Region = new Region(gp);

        }
        public void DisplayReportReturned()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM DisplayReportsReturned()", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    setColumn();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void DisplayReportReturnedAndNot(string Function)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM {Function}()", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    setColumn();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void PopulateComboBoxes()
        {

            // For overdue users
            comboBox1.Items.Add("Returned");
            comboBox1.Items.Add("Not returned");
            comboBox1.SelectedIndex = 0;
        }
        private void ReportForm_Load(object sender, EventArgs e)
        {
            DisplayReportReturned();
            PopulateComboBoxes();
            comboBox1.SelectedIndex = 0;
            textBox1.Text = "Search here ...";
            textBox1.ForeColor = Color.Gray;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BorrowForm borrowForm = new BorrowForm();
            borrowForm.Show();
            borrowForm.setUserInfo(lbluername.Text,Convert.ToInt32( lblId.Text), pictureBox2.Image);
            this.Close();
        }
        // Import necessary Windows API functions
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        // Define constants for the API
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Release the mouse capture and send a message to the form to simulate dragging
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text.Trim()) || textBox1.Text == "Search here ...")
            {
               // DisplayReport();
                return;
            }
            else
            {
                using (SqlConnection  conn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT * FROM SearchDisplayReports(@Search)", conn);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Search", "%" + textBox1.Text.Trim() + "%");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        if(dt.Rows.Count > 0)
                        {
                            dataGridView1.DataSource = dt;
                            setColumn();
                        }
                        else
                        {
                            dataGridView1.DataSource = null;
                        }
                    }catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text == "Search here ...")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Search here ...";
                textBox1.ForeColor= Color.Gray;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string Function = "";
            if(comboBox1.SelectedIndex == 0)
            {
                Function = "DisplayReportsReturned";
            }
            else if(comboBox1.SelectedIndex == 1)
            {
                Function = "DisplayReportsNotReturned";
            }
            DisplayReportReturnedAndNot(Function);
        }
    }
}
