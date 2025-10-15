using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Libray_Mnagement_Systemm
{
    public partial class EditBookForm : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";

        public EditBookForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    bool Delete = false;
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UpdateBook", conn);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", lblBookID.Text); 
                    cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
                    cmd.Parameters.AddWithValue("@Year", txtYear.Text);
                    cmd.Parameters.AddWithValue("@QTY", txtQty.Text);
                    cmd.Parameters.AddWithValue("@Delete", Delete);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Book saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Refresh the DataGridView in the BookFrm form
                        if (this.Owner is BookFrm bookFrm)  // Check if the current form's owner is BookFrm
                        {
                            bookFrm.DisplayBooks();  // Refresh the DataGridView by calling DisplayBooks()
                        }

                    }
                    else
                    {
                        MessageBox.Show("Opss! something went wrong.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
