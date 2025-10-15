using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace Libray_Mnagement_Systemm
{
    public partial class AddBorrowBook : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";
        public string UserName;
        public int ID;
        public AddBorrowBook(string userName, int iD)
        {
            InitializeComponent();
            UserName = userName;
            ID = iD;
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
        public bool CheckUserID (string userID)
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
                    if(result == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtQTY.Text) || txtQTY.Text == "0")
            {
                MessageBox.Show("Please enter a valide quantity.", "Empty quantity", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool ExistsUserID = CheckUserID(txtUserID.Text);
            if (!ExistsUserID)
            {
                MessageBox.Show("Invalid user ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool IsAvailable = CheckBookAvailability(txtBookID.Text);
            if (!IsAvailable)
            {
                MessageBox.Show("The book is invalied.", "invalied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (SqlConnection  conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    int IsReturn = 0;
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("InsertBorrowBook", conn);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", txtUserID.Text);
                    cmd.Parameters.AddWithValue("@BookID", txtBookID.Text);
                    cmd.Parameters.AddWithValue("@BorrowDate", dateTimePicker1.Value);
                    cmd.Parameters.AddWithValue("@ReturnDate", dateTimePicker2.Value);
                    cmd.Parameters.AddWithValue("@IsReturn", IsReturn);
                    cmd.Parameters.AddWithValue("@StaffName", UserName);
                    cmd.Parameters.AddWithValue("@StaffID", ID);
                    cmd.Parameters.AddWithValue("@QTY", txtQTY.Text);
                    if(cmd.ExecuteNonQuery () > 0)
                    {
                        MessageBox.Show("Borrow added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if(this.Owner is BorrowForm borrowForm)
                        {
                            borrowForm.DisplayBorrowBook("DisplayBorrowBook_UsersWithOverdueBooks");
                            borrowForm.comboBox1.SelectedIndex = 1;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Opss! we could not add borrow.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
