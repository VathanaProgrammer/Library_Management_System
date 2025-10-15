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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Libray_Mnagement_Systemm
{
    public partial class Form1 : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";
        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPw.Text) || string.IsNullOrEmpty(txtusername.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtusername.Focus();
                return;
            }
            bool isAuthenticated = UserLogins(txtusername.Text, txtPw.Text);
            if (isAuthenticated)
            {
                BookFrm bookFrm = new BookFrm();
                bookFrm.SetUserInfo(txtusername.Text, txtPw.Text);
                bookFrm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private bool UserLogins (string Username , string Password )
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand LogoCmd = new SqlCommand("UserLogin", con);
                    LogoCmd.CommandType = CommandType.StoredProcedure;
                    LogoCmd.Parameters.AddWithValue("@Name", Username);
                    LogoCmd.Parameters.AddWithValue("@Pw", Password);
                    
                    int Count = (int)LogoCmd.ExecuteScalar();
                    if (Count > 0)
                    {
                       return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch(Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
                
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            txtusername.Text = "Username";
            txtusername.ForeColor = Color.Gray;
            txtPw.Text = "Password";
            txtPw.ForeColor = Color.Gray;
            this.ActiveControl =txtusername;
        }

        private void txtusername_Enter(object sender, EventArgs e)
        {
            if (txtusername.Text == "Username")
            {
                txtusername.Text = ""; // Clear the placeholder text
                txtusername.ForeColor = Color.White; // Change the text color to normal
            }
        }

        private void txtPw_Enter(object sender, EventArgs e)
        {
            if(txtPw.Text == "Password")
            {
                txtPw.Text = "";
                txtPw.ForeColor = Color.White;
            }
        }

        private void txtusername_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                txtPw.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txtPw_KeyDown(object sender, KeyEventArgs e)
        {
            
            if(e.KeyCode == Keys.Enter){
                bool isAuthenticated = UserLogins(txtusername.Text, txtPw.Text);
                if (isAuthenticated)
                {
                    BookFrm bookFrm = new BookFrm();
                    bookFrm.SetUserInfo(txtusername.Text, txtPw.Text);
                    bookFrm.ShowDialog();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        // Import necessary Windows API functions
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        // Define constants for the API
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Release the mouse capture and send a message to the form to simulate dragging
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtPw.PasswordChar = '\0';
            }
            else
            {
                txtPw.PasswordChar = '*';
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
