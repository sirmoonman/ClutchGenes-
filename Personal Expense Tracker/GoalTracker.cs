using System;
using System.Windows.Forms;

namespace Personal_Expense_Tracker
{
    public partial class GoalTracker : Form
    {
        public GoalTracker()
        {
            InitializeComponent();
        }

        private void GoalTracker_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = 4;
            dataGridView1.Columns[0].Name = "Goal Name";
            dataGridView1.Columns[1].Name = "Target Amount";
            dataGridView1.Columns[2].Name = "Saved Money";
            dataGridView1.Columns[3].Name = "Goal Progress";
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!double.TryParse(textBox2.Text, out double amount))
                {
                    throw new Exception("Amount must be a valid number (decimal/double).");
                }

                Goal goal = new Goal(
                    textBox1.Text,
                    amount
                );

                dataGridView1.Rows.Add(
                    goal.GoalName,
                    goal.Amount.ToString("F2"),
                    "0.00",
                    "0%"
                );

                MessageBox.Show("Successfully Added");
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    if (!double.TryParse(textBox2.Text, out double amount))
                    {
                        throw new Exception("Amount must be a valid number.");
                    }

                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    selectedRow.Cells[0].Value = textBox1.Text;
                    selectedRow.Cells[1].Value = amount.ToString("F2");

                    double saved = double.Parse(selectedRow.Cells[2].Value.ToString());
                    double progress = (saved / amount) * 100;
                    if (progress > 100) progress = 100;
                    selectedRow.Cells[3].Value = $"{progress:F0}%";

                    MessageBox.Show("Successfully Updated");
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                MessageBox.Show("Successfully Deleted");
                ClearFields();
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells[0].Value != null)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
            Form expenseT = new ExpenseTracker();
            expenseT.Show();
        }

        private void ClearFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            FundsTextbox.Clear();
            dataGridView1.ClearSelection();
        }

        private void fundsButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a goal to add funds.");
                return;
            }

            if (!double.TryParse(FundsTextbox.Text, out double addedAmount))
            {
                MessageBox.Show("Please enter a valid amount to add.");
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

            double targetAmount = double.Parse(selectedRow.Cells[1].Value.ToString());
            double currentSaved = double.Parse(selectedRow.Cells[2].Value.ToString());

            double newSaved = currentSaved + addedAmount;
            if (newSaved > targetAmount)
            {
                newSaved = targetAmount;
            }

            double progress = (newSaved / targetAmount) * 100;
            if (progress > 100) progress = 100;

            selectedRow.Cells[2].Value = newSaved.ToString("F2");
            selectedRow.Cells[3].Value = $"{progress:F0}%";

            FundsTextbox.Clear();
        }

        private void deductFunds_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a goal to deduct funds from.");
                return;
            }

            if (!double.TryParse(FundsTextbox.Text, out double deductAmount))
            {
                MessageBox.Show("Please enter a valid amount to deduct.");
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

            double targetAmount = double.Parse(selectedRow.Cells[1].Value.ToString());
            double currentSaved = double.Parse(selectedRow.Cells[2].Value.ToString());

            double newSaved = currentSaved - deductAmount;
            if (newSaved < 0)
            {
                newSaved = 0;
            }

            double progress = (newSaved / targetAmount) * 100;
            if (progress < 0) progress = 0;

            selectedRow.Cells[2].Value = newSaved.ToString("F2");
            selectedRow.Cells[3].Value = $"{progress:F0}%";

            FundsTextbox.Clear();
        }
    }

    public class Goal
    {
        public string GoalName { get; set; }
        public double Amount { get; set; }

        public Goal(string goalName, double amount)
        {
            GoalName = goalName;
            Amount = amount;
        }
    }
}
