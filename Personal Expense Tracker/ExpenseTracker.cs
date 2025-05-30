using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Personal_Expense_Tracker
{
    public partial class ExpenseTracker: Form
    {
        public ExpenseTracker()
        {
            InitializeComponent();
        }

        private void ExpenseTracker_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form Tracker = new Tracker();
            Tracker.Show();
            this.Close();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form goalTracker = new GoalTracker();
            goalTracker.Show();
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form subtracker = new SubscriptionTracker();
            subtracker.Show();
            this.Close();
        }

        private void btnViewTransactions_Click(object sender, EventArgs e)
        {
            TransactionHistoryForm historyForm = new TransactionHistoryForm();
            historyForm.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form healthscore = new HealthScore();
            healthscore.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Logout Successfully");          
            this.Close();
            Form form1 = new Form1();
            form1.Show();

;        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
