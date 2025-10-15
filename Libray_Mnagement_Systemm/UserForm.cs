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
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Libray_Mnagement_Systemm
{
    public partial class UserForm : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";

        public UserForm()
        {
            InitializeComponent();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            
            textBox1.Text = "Search here ...";
            textBox1.ForeColor = Color.Gray;
            DisplayUsers();
        }
        public void DisplayUsers()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM DisplayUsers()", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                    setColumn();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void SetUserInfo( string Username, int ID, System.Drawing.Image userImage)
        {
            lbluername.Text = Username;
            lblId.Text = ID.ToString();
            MakePictureBoxCircular(pictureBox2);
            if(userImage != null )
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
        public void setColumn()
        {
            dataGridView1.Columns["User ID"].Width = 140;
            dataGridView1.Columns["Full Name"].Width = 175;
            dataGridView1.Columns["Phone Number"].Width = 175;
            dataGridView1.Columns["Address"].Width = 170;
            dataGridView1.Columns["Join Date"].Width = 170;
            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(11, 95, 155);
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.ForeColor = Color.White;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);



            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 0, 64); ;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {

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

        private void button1_Click(object sender, EventArgs e)
        {
            AddUserForm addUserForm = new AddUserForm();
            addUserForm.Owner = this;
            addUserForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count >= 0)
            {
                DataGridViewRow SelectRow = dataGridView1.SelectedRows[0];
                if(SelectRow.Cells[0].Value != null)
                {
                    string UserID = SelectRow.Cells[0].Value.ToString();
                    string FullName = SelectRow.Cells[1].Value.ToString();
                    string Contact = SelectRow .Cells[2].Value.ToString();
                    string Address = SelectRow .Cells[3].Value.ToString();
                    DateTime JoinDate = Convert.ToDateTime(SelectRow.Cells[4].Value);

                    EditUser editUser = new EditUser();
                    editUser.lblID.Text = UserID;
                    editUser.txtName.Text = FullName;
                    editUser.txtContact.Text = Contact;
                    editUser.txtAddress.Text = Address;
                    editUser.dateTimePicker1.Value = JoinDate;
                    
                    editUser.Owner = this;
                    editUser.ShowDialog();
                }
                else
                {
                    // Notify the user if the selected row is invalid
                    MessageBox.Show("The selected row does not contain valid data.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row  in dataGridView1.SelectedRows)
            {
                int UserId = Convert.ToInt32(row.Cells[0].Value);
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this user's information?", "Delete confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(dialogResult == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        try
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("DeleteUser", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID", UserId);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("User deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                DisplayUsers();
                            }
                            else
                            {
                                MessageBox.Show("Opss! we could not delete user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            BookFrm bookFrm = new BookFrm();
            bookFrm.SetUserInfoOnanotherForm(lbluername.Text, Convert.ToInt32(lblId.Text), pictureBox2.Image);
            bookFrm.Show();
            this.Hide();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            BorrowForm borrowForm = new BorrowForm();
            borrowForm.setUserInfo(lbluername.Text, Int32.Parse(lblId.Text), pictureBox2.Image);
            borrowForm.Show();
            this.Hide();
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();

            form1.Show();
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text.Trim()) || textBox1.Text == "Search here ...")
            {
                DisplayUsers();
                return;
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT * FROM SearchDisplayUsers (@Search)", conn);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Search", "%" +  textBox1.Text.Trim() + "%");
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
                            dataGridView1.DataSource= null;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Search here ..."; // Set placeholder text
                textBox1.ForeColor = Color.Gray; // Set the color to indicate it's a placeholder
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Search here ...")
            {
                textBox1.Text = ""; // Clear the placeholder text
                textBox1.ForeColor = Color.Black; // Change the text color to normal
            }
        }
    }
}
