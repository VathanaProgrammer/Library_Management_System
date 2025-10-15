using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Libray_Mnagement_Systemm
{
    public partial class AddBookForm : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";

        public AddBookForm()
        {
            InitializeComponent();
        }
        // Import necessary Windows API functions
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        // Define constants for the API
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        private void AddBookForm_Load(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    bool Delete = false;
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("InsertBook", conn);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
                    cmd.Parameters.AddWithValue("@Year", txtYear.Text);
                    cmd.Parameters.AddWithValue("@QTY", txtQty.Text);
                    cmd.Parameters.AddWithValue("@Delete",Delete);
                    if(cmd.ExecuteNonQuery () > 0)
                    {
                        MessageBox.Show("Book saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if(this.Owner is BookFrm book)
                        {
                            book.DisplayBooks();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Opss! something went wrong.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    }
                } catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtTitle.Clear();
            txtAuthor.Clear();
            txtYear.Clear();
            txtQty.Clear();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Release the mouse capture and send a message to the form to simulate dragging
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
