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
    public partial class EditBorrowBook : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";

        public EditBorrowBook()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public bool CheckBookAvailability(string bookID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("CheckBookAvailable", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BookID", bookID);  // Pass the BookID

                    // Execute the query and get the result
                    var result = cmd.ExecuteScalar();

                    // Check if the result indicates availability
                    if (Convert.ToInt32(result) == 1)
                    {
                        return true;  // Book is available
                    }
                    else
                    {
                        return false;  // Book is not available
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public bool CheckUserID(string userID)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("CkeckUserID", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", userID);
                    int result = (int)cmd.ExecuteScalar();
                    if (result == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            bool ExsitsUserID = CheckUserID(txtUserID.Text);
            if (!ExsitsUserID)
            {
                MessageBox.Show("Invalid user ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool IsAvailable = CheckBookAvailability(txtBookID.Text);
            if(!IsAvailable)
            {
                MessageBox.Show("Invalid book ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);   
                return;
            }
            
            int QTY;
            if (int.TryParse(txtQTY.Text, out QTY))
            {
                if (QTY == 0)
                {
                    MessageBox.Show("Please enter a valid quantity.", "Invalid quantity", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter a valid number for quantity.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UpdateBorrowBook", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID", lblid.Text);
                    cmd.Parameters.AddWithValue("@UserID", txtUserID.Text);
                    cmd.Parameters.AddWithValue("@BookID", txtBookID.Text);
                    cmd.Parameters.AddWithValue("@BorrowDate", dateTimePicker2.Value);
                    cmd.Parameters.AddWithValue("@ReturnDate", dateTimePicker1.Value);
                    cmd.Parameters.AddWithValue("@QTY", txtQTY.Text);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Borrow record saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (this.Owner is BorrowForm borrowForm)
                        {
                            borrowForm.LoadAllUsers();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No changes were made. Borrow record was not updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (SqlException sqlEx)
                {
                    // Specific error handling for SQL exceptions
                    MessageBox.Show($" {sqlEx.Message}", " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // General error handling
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void panel2_MouseDown(object sender, MouseEventArgs e)
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
