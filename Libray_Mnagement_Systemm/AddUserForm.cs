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
    public partial class AddUserForm : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";

        public AddUserForm()
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Please fill in the textBox at least name.", "Empty name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtContact.Text, out _))
            {
                MessageBox.Show("Contact must be a number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    bool Delete = false;
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("InsertUser", conn);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FullName", txtName.Text);
                    cmd.Parameters.AddWithValue("@PhoneNumber", txtContact.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@JoinDate", Convert.ToDateTime(dateTimePicker1.Value));
                    cmd.Parameters.AddWithValue("@Delete", Delete);
                    if(cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("User added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if( this.Owner is UserForm userForm)
                        {
                            userForm.DisplayUsers();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Opss! we could not add user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtAddress.Clear();
            txtContact.Clear();
            txtName.Clear();    
            dateTimePicker1.Value = DateTime.Now;
            this.ActiveControl = txtName;
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
    }
}
