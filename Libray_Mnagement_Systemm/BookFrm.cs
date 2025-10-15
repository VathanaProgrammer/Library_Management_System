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
using System.IO;

namespace Libray_Mnagement_Systemm
{
    public partial class BookFrm : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";

        public BookFrm()
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

      

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void BookFrm_Load(object sender, EventArgs e)
        {
            DisplayBooks();
            textBox1.Text = "Search here ...";
            textBox1.ForeColor = Color.Gray;
        }
        public void SetUserInfoOnanotherForm(string userName, int ID, System.Drawing.Image userImage)
        {
            lbluername.Text = userName;
            lblId.Text = ID.ToString();
            MakePictureBoxCircular(pictureBox2);
            if (userImage != null)
            {
                pictureBox2.Image = userImage;
            }
            else
            {
                pictureBox2.Image = null;
            }
        }
        public void SetUserInfo(string username, string PW)
        {
            lbluername.Text = username;
            string UserID = GetUserID(PW);
            pictureBox2.Image = GetUserImage(PW);
            MakePictureBoxCircular(pictureBox2);
            if (UserID != null)
            {
                lblId.Text = UserID.ToString();
            }
           
        }
        private void MakePictureBoxCircular(PictureBox pictureBox)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pictureBox.Width, pictureBox.Height);
            pictureBox.Region = new Region(gp);
        }

        public Image GetUserImage(string Pw)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Photo FROM UserLogins WHERE Password = @PW", conn);
                cmd.Parameters.AddWithValue("@PW", Pw);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    byte[] imageData = (byte[])reader["Photo"];
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        return Image.FromStream(ms);
                    }
                }
            }
            return null;
        }

        public string GetUserID (string Pw)
        {
            string ID = string.Empty;
      
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT UserID FROM UserLogins WHERE Password = @PW", conn);
                cmd.Parameters.AddWithValue("@PW", Pw);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ID = reader["UserID"].ToString();
             
                }
                return ID;
            }
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Release the mouse capture and send a message to the form to simulate dragging
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            this.Hide();
        }
        public void DisplayBooks()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM DisplayBooks()", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
                setColumn();
            }
        }
        public void setColumn()
        {
            dataGridView1.Columns["Book ID"].Width = 150;
            dataGridView1.Columns["Title"].Width = 200;
            dataGridView1.Columns["Author"].Width = 200;
            dataGridView1.Columns["Published Year"].Width= 170;
            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(11, 95, 155);
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.ForeColor = Color.White;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            


            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 0, 64); ;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }
        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this book.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    int ID = Convert.ToInt32(row.Cells[0].Value);
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        try
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("DeleteBook", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID", ID);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("The book has been successfully deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                DisplayBooks();
                            }
                            else
                            {
                                MessageBox.Show("Opss! we could not delete book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void button1_Click(object sender, EventArgs e)
        {
            AddBookForm addBookForm = new AddBookForm();
            addBookForm.Owner = this;  
            addBookForm.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Check if the row contains valid data (based on a key column, like BookID)
                if (selectedRow.Cells[0].Value != null && !string.IsNullOrEmpty(selectedRow.Cells[0].Value.ToString()))
                {
                    // Retrieve data from the selected row
                    string bookID = selectedRow.Cells[0].Value.ToString(); // BookID
                    string title = selectedRow.Cells[1].Value.ToString();  // Title
                    string author = selectedRow.Cells[2].Value.ToString(); // Author
                    string year = selectedRow.Cells[3].Value.ToString();   // Published Year
                    int Qty = Convert.ToInt32(selectedRow.Cells[4].Value); // Availability

                    // Pass data to your edit form
                    EditBookForm editBookForm = new EditBookForm();
                    editBookForm.lblBookID.Text = bookID;
                    editBookForm.txtTitle.Text = title;
                    editBookForm.txtAuthor.Text = author;
                    editBookForm.txtYear.Text = year;
                    editBookForm.txtQty.Text = Qty.ToString();

                    // Show the EditBookForm
                    editBookForm.Owner = this;
                    editBookForm.ShowDialog();
                   
                }
                else
                {
                    // Notify the user if the selected row is invalid
                    MessageBox.Show("The selected row does not contain valid data.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                // Notify the user if no row is selected
                MessageBox.Show("Please select a row before clicking Edit.", "No Row Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            UserForm userForm = new UserForm();
            userForm.SetUserInfo(lbluername.Text, Int32.Parse(lblId.Text), pictureBox2.Image);
            userForm.Show();
            this.Hide();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            BorrowForm borrowForm = new BorrowForm();
            borrowForm.Show();
            borrowForm.setUserInfo(lbluername.Text, Convert.ToInt32(lblId.Text), pictureBox2.Image);
            this.Hide();    
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text.Trim()) || textBox1.Text == "Search here ...")
            {
                DisplayBooks();
                return;
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT * FROM SearchDisplayBooks (@Search)", conn);
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
                            dataGridView1.DataSource=null;
                            
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
            if (textBox1.Text == "Search here ...")
            {
                textBox1.Text = ""; // Clear the placeholder text
                textBox1.ForeColor = Color.Black; // Change the text color to normal
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

        private void button4_Click(object sender, EventArgs e)
        {
            AddBookQuantity addBookQuantity = new AddBookQuantity();
            if(dataGridView1.SelectedRows.Count >= 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                if (row.Cells[0].Value != null && !string.IsNullOrEmpty(row.Cells[0].Value.ToString()))
                {
                    addBookQuantity.lblBookID.Text = row.Cells[0].Value.ToString();
                    addBookQuantity.Owner = this;
                    addBookQuantity.ShowDialog();
                }
                else
                {
                    // Notify the user if the selected row is invalid
                    MessageBox.Show("The selected row does not contain valid data.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
