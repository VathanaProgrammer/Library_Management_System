using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Libray_Mnagement_Systemm
{
    public partial class StaffRegister : Form
    {
        public StaffRegister()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

           
            string ConnectionString = "Data Source=DESKTOP-BCAQFC2\\SQLEXPRESS;Initial Catalog=Library_Management_System;Integrated Security=True;";
            using (SqlConnection  conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE UserLogins SET Photo = @Photo WHERE UserID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));   
                if(pictureBox1.Image != null)
                {
                    MemoryStream ms = new MemoryStream();
                   pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte [] data = ms.ToArray();
                    cmd.Parameters.AddWithValue("@Photo", data);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Photo", DBNull.Value);
                }
                cmd.ExecuteNonQuery();
                MessageBox.Show("Success!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Open a file dialog to allow the user to choose an image file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Display the selected image in the PictureBox
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);

                // Convert the image to a byte array to store it for later use
                //MemoryStream memoryStream = new MemoryStream();
                //pictureBox2.Image.Save(memoryStream, pictureBox2.Image.RawFormat);
                //imageBytes = memoryStream.ToArray(); // Store image bytes
            }
        }
    }
}
