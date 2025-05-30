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
    public partial class HealthScore : Form
    {
        private double totalSavings = 0.0;

        public HealthScore()
        {
            InitializeComponent();
        }

        private void HealthScore_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Date";
            dataGridView1.Columns[1].Name = "Amount";
            dataGridView1.Columns[2].Name = "Note";

            comboBox2.Items.AddRange(new string[] {
                "Food", "Social Life", "Pets", "Transport",
                "Beauty", "Health", "Gift", "Education", "Others (Please type it in the box)"
            });

            dataGridView2.ColumnCount = 3;
            dataGridView2.Columns[0].Name = "Date";
            dataGridView2.Columns[1].Name = "Category";
            dataGridView2.Columns[2].Name = "Amount";

            TotalDebt.Text = "₱0.00";
            TotalSavings.Text = "₱0.00";
            TipLabel.Text = "";
            UpdateTotalDebt();
            UpdateTotalSavings();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
            Form expenseT = new ExpenseTracker();
            expenseT.Show();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                string date = dateTimePicker1.Value.ToShortDateString();
                string note = textBox3.Text;

                if (!double.TryParse(textBox2.Text, out double amount))
                {
                    throw new Exception("Amount must be a valid number.");
                }

                dataGridView1.Rows.Add(date, amount.ToString("F2"), note);
                totalSavings += amount; // Add to total savings
                UpdateTotalSavings();

                MessageBox.Show("Expense added successfully.");
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Add Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    string date = dateTimePicker1.Value.ToShortDateString();
                    string note = textBox3.Text;

                    if (!double.TryParse(textBox2.Text, out double newAmount))
                    {
                        throw new Exception("Amount must be a valid number.");
                    }

                    DataGridViewRow row = dataGridView1.SelectedRows[0];

                    if (double.TryParse(row.Cells[1].Value.ToString(), out double oldAmount))
                    {
                        totalSavings -= oldAmount; // Remove old amount
                        totalSavings += newAmount; // Add new amount
                        UpdateTotalSavings();
                    }

                    row.Cells[0].Value = date;
                    row.Cells[1].Value = newAmount.ToString("F2");
                    row.Cells[2].Value = note;

                    MessageBox.Show("Entry updated successfully.");
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Edit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to edit.");
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];

                if (double.TryParse(row.Cells[1].Value.ToString(), out double amount))
                {
                    totalSavings -= amount;
                    if (totalSavings < 0) totalSavings = 0;
                    UpdateTotalSavings();
                }

                dataGridView1.Rows.RemoveAt(row.Index);
                MessageBox.Show("Entry deleted successfully.");
                ClearFields();
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void borrowButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!double.TryParse(textBox4.Text, out double borrowAmount))
                {
                    throw new Exception("Amount must be a valid number (decimal/double).");
                }

                string category = comboBox2.Text;
                if (string.IsNullOrWhiteSpace(category))
                {
                    throw new Exception("Please select or enter a category.");
                }

                if (dataGridView1.SelectedRows.Count == 0)
                {
                    throw new Exception("Please select a row in the Expenses table (dataGridView1) to deduct from.");
                }

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string currentAmountText = selectedRow.Cells[1].Value?.ToString();

                if (!double.TryParse(currentAmountText, out double currentAmount))
                {
                    throw new Exception("The selected row contains an invalid amount.");
                }

                if (borrowAmount > currentAmount)
                {
                    throw new Exception("Borrow amount cannot exceed the current amount in the selected row.");
                }

                double newAmount = currentAmount - borrowAmount;
                selectedRow.Cells[1].Value = newAmount.ToString("F2");

                totalSavings -= borrowAmount; // Deduct from savings
                if (totalSavings < 0) totalSavings = 0;
                UpdateTotalSavings();

                string borrowDate = dateTimePicker2.Value.ToShortDateString();
                dataGridView2.Rows.Add(borrowDate, category, borrowAmount.ToString("F2"));

                UpdateTotalDebt();

                MessageBox.Show("Borrowed amount deducted and recorded successfully.");

                textBox4.Clear();
                comboBox2.SelectedIndex = -1;
                dataGridView1.ClearSelection();
                dataGridView2.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Borrow Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotalDebt()
        {
            double total = 0;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells[2].Value != null && double.TryParse(row.Cells[2].Value.ToString(), out double amount))
                {
                    total += amount;
                }
            }

            TotalDebt.Text = "₱-" + total.ToString("F2");

            if (total == 0)
            {
                TipLabel.Text = "Great job! You're managing your finances well.";
            }
            else if (total <= 500)
            {
                TipLabel.Text = "Keep an eye on small debts before they pile up.";
            }
            else if (total <= 2000)
            {
                TipLabel.Text = "Try to reduce your debt. Consider cutting non-essential expenses.";
            }
            else
            {
                TipLabel.Text = "High debt alert! Focus on debt repayment and limit borrowing.";
            }
        }

        private void UpdateTotalSavings()
        {
            TotalSavings.Text = "₱" + totalSavings.ToString("F2");
        }

        private void ClearFields()
        {
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            comboBox2.SelectedIndex = -1;
            dataGridView1.ClearSelection();
        }

        private void paydeptbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!double.TryParse(paydept.Text, out double paymentAmount) || paymentAmount <= 0)
                {
                    throw new Exception("Please enter a valid positive number to pay.");
                }

                // Calculate current total debt
                double currentTotalDebt = 0;
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells[2].Value != null && double.TryParse(row.Cells[2].Value.ToString(), out double amount))
                    {
                        currentTotalDebt += amount;
                    }
                }

                // Subtract payment from current total debt (but do not let it go below zero)
                double newTotalDebt = currentTotalDebt - paymentAmount;
                if (newTotalDebt < 0) newTotalDebt = 0;

                // Update TotalDebt label
                TotalDebt.Text = "₱-" + newTotalDebt.ToString("F2");

                // ✅ Add paid amount to totalSavings
                totalSavings += paymentAmount;
                UpdateTotalSavings();

                // Update TipLabel based on new debt
                if (newTotalDebt == 0)
                {
                    TipLabel.Text = "Great job! You're managing your finances well.";
                }
                else if (newTotalDebt <= 500)
                {
                    TipLabel.Text = "Keep an eye on small debts before they pile up.";
                }
                else if (newTotalDebt <= 2000)
                {
                    TipLabel.Text = "Try to reduce your debt. Consider cutting non-essential expenses.";
                }
                else
                {
                    TipLabel.Text = "High debt alert! Focus on debt repayment and limit borrowing.";
                }

                paydept.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
