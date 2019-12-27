using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CS_DIM_Integration
{
    public partial class LoginFrm : Form
    {
        public LoginFrm()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
              //  using (var connection = new SqlConnection("Server= 172.22.44.10; Database=MM_OFFICE_HIS_test; user id=sa; password=Anz20SQL17#June19;"))
                 using (var connection = new SqlConnection(ConnectStr.strSQLSrv))
                {
                    connection.Open();
                    var cmd = new SqlCommand("SELECT COUNT(*) FROM HHUsers WHERE HHUUserID = @user AND HHUPasswd = @pwd", connection);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@user", textBox1.Text);
                    cmd.Parameters.AddWithValue("@pwd", textBox2.Text);

                    if (cmd.ExecuteScalar().ToString() == "1")
                    {
                       
                        /* I have made a new page called home page. If the user is successfully authenticated then the form will be moved to the next form */
                        Form1 mf = new Form1(textBox1.Text);
                      //  mf.ShowDialog();
                        mf.Show();
                        this.Hide();
                    }
                    else
                    {
                        connection.Close();
                        MessageBox.Show("Invalid username or password");                       
                    }
                    connection.Close();
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login Error : " + ex.ToString());

            }

        }

        private void Txt_pwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Button1_Click(sender, e); 
            }

        }
    }//LoginFrm
}
