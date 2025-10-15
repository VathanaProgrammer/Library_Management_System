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
    public partial class AddBookQuantity : Form
    {
        private string ConnectionString = "Data Source=ASUS\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;Encrypt=False";

        public AddBookQuantity()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtQty.Text) || txtQty.Text == "0")
            {
                MessageBox.Show("Please enter a valid quantity.", "Empty quantity", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("AddBookQTY", conn);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", lblBookID.Text);
                    cmd.Parameters.AddWithValue("@QTY", txtQty.Text);
                    if(cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Quantity added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if(this.Owner is BookFrm bookFrm)
                        {
                            bookFrm.DisplayBooks();
                        }
                    }
                }catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
