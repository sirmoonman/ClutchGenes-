using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Personal_Expense_Tracker
{
    public partial class RegisterForm : Form
    {
        // DATABASE NI ZEJI
         //DATABASE CONNECTION TO, WAG NIYONG GAGALAWIN - ZEJI

      SqlConnection dbZ = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\agaca\OneDrive - STI College Ortigas-Cainta\Documents\UsersExpense.mdf"";Integrated Security=True;Connect Timeout=30");

        public RegisterForm()
        {
            InitializeComponent();
        }

        //DATABASE CONNECTION TO, WAG NIYONG GAGALAWIN - ZEJI
         public bool checkConnection()
          {
              return (dbZ.State == ConnectionState.Closed ? true : false);
          }
        
        private void panel5_Paint(object sender, PaintEventArgs e) { }

        private void RegisterForm_Load(object sender, EventArgs e) {
        
        
        
        
        }

        private void label5_Click(object sender, EventArgs e) { }

        private void panel4_Paint(object sender, PaintEventArgs e) { }

        // HIDE AND UNHIDE PASSWORD NI ZEJI
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            PassReg1.PasswordChar = checkBox1.Checked ? '\0' : '*';
            PassReg2.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        private void Header_Click(object sender, EventArgs e) { }

        private void SignUp_Click(object sender, EventArgs e)
        {











            //DATABASE CONNECTION TO, WAG NIYONG GAGALAWIN - ZEJI

            if (RegText1.Text == "" || PassReg1.Text == "" || PassReg2.Text == "")
             {
                 MessageBox.Show("Please fill the blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
             }
             else
             {
                 if (checkConnection())
                 {
                     try
                     {
                         dbZ.Open();
                         // CHECKING OF USERNAME SA DATABASE NI ZEJI
                         string SelectUser = "SELECT * FROM UsersExpense WHERE username = @usern";

                         using (SqlCommand CheckingUser = new SqlCommand(SelectUser, dbZ))
                         {
                             CheckingUser.Parameters.AddWithValue("@usern", RegText1.Text.Trim());

                             SqlDataAdapter adt = new SqlDataAdapter(CheckingUser); 
                             DataTable DT = new DataTable();
                             adt.Fill(DT);

                             // VALIDATOR NI ZEJI
                             if (DT.Rows.Count != 0)
                             {
                                 string UpperUser = RegText1.Text.Substring(0, 1).ToUpper() + RegText1.Text.Substring(1);
                                 MessageBox.Show(UpperUser + " is already exist.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                             }
                             else if (PassReg1.Text.Length < 8)
                             {
                                 MessageBox.Show("Invalid Password. Please enter at least 8 characters", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                             }
                             else if (PassReg1.Text != PassReg2.Text)
                             {
                                 MessageBox.Show("Password does not match.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                             }
                             else
                             {

                                 string InsertingData = "INSERT INTO UsersExpense(username, password, date_create) VALUES (@usern, @pass, @date)";
                                 using (SqlCommand InsertingUser = new SqlCommand(InsertingData, dbZ))
                                 {
                                     InsertingUser.Parameters.AddWithValue("@usern", RegText1.Text.Trim());
                                     InsertingUser.Parameters.AddWithValue("@pass", PassReg1.Text.Trim());
                                     InsertingUser.Parameters.AddWithValue("@date", DateTime.Today);

                                     InsertingUser.ExecuteNonQuery();

                                     MessageBox.Show("Registered Successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                     Form1 lform = new Form1();
                                     lform.Show();
                                     this.Hide();
                                 }
                             }
                         }
                     }
                     catch (Exception ex)
                     {
                         MessageBox.Show("Failed Connection: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     }
                     finally
                     {
                         dbZ.Close();
                     }
                 }
             }
        }

        private void header2_Click(object sender, EventArgs e) {
        
        }

        private void header3_Click(object sender, EventArgs e) {
        
        }

        private void textBox2_TextChanged(object sender, EventArgs e) {
        
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
        
        }

        private void textBox3_TextChanged(object sender, EventArgs e) {
        
        }

        private void label1_Click(object sender, EventArgs e) { 
        
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { 
        
        }

        private void label2_Click(object sender, EventArgs e) { 
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 F1 = new Form1();
            F1.Show();
            this.Hide();
        }
    }
}
