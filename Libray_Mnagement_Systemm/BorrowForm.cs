using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Image = System.Windows.Controls.Image;

namespace Libray_Mnagement_Systemm
{
    public partial class BorrowForm : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";

        public BorrowForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a borrowed book to return.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                int ID = Convert.ToInt32(row.Cells[0].Value);
                DateTime ReturnDate = DateTime.Now; 
                int BookID = Convert.ToInt32(row.Cells[2].Value);
                int QTY = Convert.ToInt32(row.Cells[3].Value);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("ReturnBook", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@ReturnDate", ReturnDate);
                        cmd.Parameters.AddWithValue("@BookID", BookID);
                        cmd.Parameters.AddWithValue("@QTY", QTY);
                        if(cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Book returned.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DisplayBorrowBook("DisplayBorrowBooks");
                        }
                        else
                        {
                            MessageBox.Show("Opss! we could not return the book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
           
        }
        public static System.Drawing.Image ConvertWpfImageToDrawingImage(System.Windows.Controls.Image wpfImage)
        {
            // Get the BitmapSource from the WPF image
            var bitmapSource = (System.Windows.Media.Imaging.BitmapSource)wpfImage.Source;

            // Convert BitmapSource to Bitmap
            using (var stream = new MemoryStream())
            {
                var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);

                return new Bitmap(stream);
            }
        }

        public void setUserInfo (string username, int ID, System.Drawing.Image userImage)
        {

            lbluername.Text = username;
            lblId.Text = ID.ToString();
            MakePictureBoxCircular(pictureBox2);
            if (userImage != null)
            {
                pictureBox2.Image = userImage;
            }
            else
            {
                pictureBox2.Image = null; // Set a default image or leave it blank
            }
        }
        private void MakePictureBoxCircular(PictureBox pictureBox)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pictureBox.Width, pictureBox.Height);
            pictureBox.Region = new Region(gp);
           
        }
        private void button1_Click(object sender, EventArgs e)
        {
            AddBorrowBook addBorrowBook = new AddBorrowBook(lbluername.Text, Convert.ToInt32(lblId.Text));
            addBorrowBook.Owner = this;
            addBorrowBook.ShowDialog();
        }
        public void setColumn()
        {
            dataGridView1.Columns["Borrow ID"].Width = 130;
            dataGridView1.Columns["User ID"].Width = 150;
            dataGridView1.Columns["Book ID"].Width = 150;
            dataGridView1.Columns["Quantity"].Width = 80;
            dataGridView1.Columns["Borrow Date"].Width = 160;
            dataGridView1.Columns["Return Date"].Width = 160;
            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(11, 95, 155);
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.ForeColor = Color.White;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);



            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 0, 64); ;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }
        public void LoadAllUsers()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM DisplayBorrowBooks()", conn);
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
        public void DisplayBorrowBook(string Function)
        {
            using(SqlConnection conn = new SqlConnection(ConnectionString))
            {
               try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM {Function} ()", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    setColumn();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void iconButton3_Click(object sender, EventArgs e)
        {
            UserForm userForm = new UserForm();
            userForm.SetUserInfo(lbluername.Text, Int32.Parse(lblId.Text ), pictureBox2.Image);
            userForm.Show();
            this.Hide();
        }
        private void PopulateComboBoxes()
        {

            // For overdue users
            comboBox1.Items.Add("All Users");
            comboBox1.Items.Add("Users With Overdue Books");
            comboBox1.SelectedIndex = 0;
        }

        private void BorrowForm_Load(object sender, EventArgs e)
        {
            PopulateComboBoxes();
            LoadAllUsers();
            textBox1.Text = "Search here ...";
            textBox1.ForeColor = Color.Gray;
            comboBox1.SelectedIndex = 0;
            comboBox1_SelectedIndexChanged(sender, e);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string Function = "";

            if (comboBox1.SelectedItem.ToString() == "All Users")
            {
                Function = "DisplayBorrowBooks";
            }
            else if (comboBox1.SelectedItem.ToString() == "Users With Overdue Books")
            {
                Function = "DisplayBorrowBook_UsersWithOverdueBooks";
            }

            DisplayBorrowBook(Function);

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

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count >= 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                if (row.Cells[0].Value != null && !string.IsNullOrEmpty(row.Cells[0].Value.ToString()))
                {
                  EditBorrowBook editBorrowBook = new EditBorrowBook();
                    editBorrowBook.lblid.Text = row.Cells[0].Value?.ToString();
                    editBorrowBook.txtUserID.Text = row.Cells[1].Value?.ToString();
                    editBorrowBook.txtBookID.Text = row.Cells[2].Value?.ToString();
                    editBorrowBook.txtQTY.Text = row.Cells[3].Value?.ToString();
                    editBorrowBook.dateTimePicker2.Value = Convert.ToDateTime(row.Cells[4].Value);
                    editBorrowBook.dateTimePicker1.Value = Convert.ToDateTime(row.Cells[5].Value);
                    editBorrowBook.Owner = this;
                    editBorrowBook.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please a valid row to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            BookFrm bookFrm = new BookFrm();
            bookFrm.Show();
            bookFrm.SetUserInfoOnanotherForm(lbluername.Text, Convert.ToInt32(lblId.Text), pictureBox2.Image);
            this.Hide();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            BorrowForm borrowForm = new BorrowForm();
            borrowForm.setUserInfo(lbluername.Text , Int32.Parse(lblId.Text), pictureBox2.Image);
            borrowForm.Show();
            this.Hide();
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                string Function = "";
                if(comboBox1.SelectedIndex == 0)
                {
                    Function = "DisplayBorrowBooks";
                }
                else if(comboBox1.SelectedIndex == 1) 
                {
                    Function = "DisplayBorrowBook_UsersWithOverdueBooks";
                }
                DisplayBorrowBook(Function);
                return;
            }
            else
            {
                string SearchFunction = "";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        if (comboBox1.SelectedIndex == 0)
                        {
                            SearchFunction = "SearchDisplayBorrowBook";
                        }
                        else if(comboBox1.SelectedIndex == 1)
                        {
                            SearchFunction = "SearchDisplayBorrowBook_UsersWithOverdueBooks";
                        }
                        conn.Open();
                        SqlCommand cmd = new SqlCommand($"SELECT * FROM {SearchFunction}(@Search)", conn);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Search", "%" + textBox1.Text.Trim() + "%");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count > 0)
                        {
                            dataGridView1.DataSource = dataTable;
                            setColumn();
                        }
                        else
                        {
                            dataGridView1.DataSource = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            ReportForm reportForm = new ReportForm();
            reportForm.SetUserInfo(lbluername.Text, Convert.ToInt32(lblId.Text), pictureBox2.Image);
            reportForm.Show();
            this.Hide();
        }
       
        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Close();
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

       
    }
}
