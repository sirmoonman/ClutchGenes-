using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Personal_Expense_Tracker
{
    public partial class TransactionHistoryForm : Form
    {
        public TransactionHistoryForm()
        {
            InitializeComponent();
            cmbCategory.SelectedIndexChanged += cmbCategory_SelectedIndexChanged;
        }

        private void TransactionHistoryForm_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadTransactions();
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All");

            var categories = ExpenseRepository.Expenses
                .Select(e => e.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToArray();

            cmbCategory.Items.AddRange(categories);

            cmbCategory.SelectedIndex = 0;
        }

        private void LoadTransactions(string filter = "")
        {
            var allExpenses = ExpenseRepository.Expenses;

            var filtered = allExpenses
                .Where(exp => exp.Date >= dtpFrom.Value.Date && exp.Date <= dtpTo.Value.Date)
                .ToList();

            if (cmbCategory.SelectedItem != null && cmbCategory.SelectedItem.ToString() != "All")
            {
                string selectedCategory = cmbCategory.SelectedItem.ToString();
                filtered = filtered
                    .Where(exp => exp.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            string amountText = txtAmount.Text.Trim();
            if (!string.IsNullOrEmpty(amountText))
            {
                if (amountText.Contains("-"))
                {
                    var parts = amountText.Split('-');
                    if (parts.Length == 2 &&
                        decimal.TryParse(parts[0].Trim(), out decimal minAmount) &&
                        decimal.TryParse(parts[1].Trim(), out decimal maxAmount))
                    {
                        filtered = filtered
                            .Where(exp => decimal.TryParse(exp.Amount, out decimal amt) && amt >= minAmount && amt <= maxAmount)
                            .ToList();
                    }
                    else
                    {
                        MessageBox.Show("Invalid amount range format. Use min-max, e.g. 100-500.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    if (decimal.TryParse(amountText, out decimal exactAmount))
                    {
                        filtered = filtered
                            .Where(exp => decimal.TryParse(exp.Amount, out decimal amt) && amt == exactAmount)
                            .ToList();
                    }
                    else
                    {
                        MessageBox.Show("Invalid amount value.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            // Convert to DataTable for display
            DataTable table = new DataTable();
            table.Columns.Add("Amount", typeof(string));
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("Category", typeof(string));
            table.Columns.Add("PaymentMethod", typeof(string));

            foreach (var exp in filtered)
            {
                table.Rows.Add(exp.Amount, exp.Date, exp.Category, exp.Account);
            }

            dataGridView1.DataSource = table;

            UpdateSummary(table);
            UpdateChart(table);
        }

        private void UpdateSummary(DataTable table)
        {
            decimal total = 0;
            var summary = new StringBuilder();

            var groups = table.AsEnumerable()
                .GroupBy(row => row.Field<string>("Category"));

            foreach (var group in groups)
            {
                decimal categoryTotal = group.Sum(row => decimal.TryParse(row.Field<string>("Amount"), out decimal amt) ? amt : 0);
                total += categoryTotal;
                summary.AppendLine($"{group.Key}: ₱{categoryTotal:N2}");
            }

            lblTotal.Text = $"Total Spending: ₱{total:N2}";
            lblCategorySummary.Text = summary.Length > 0 ? summary.ToString() : "No data to summarize.";
        }

        private void UpdateChart(DataTable table)
        {
            chartSummary.Series.Clear();
            chartSummary.ChartAreas.Clear();

            var chartArea = new ChartArea();
            chartSummary.ChartAreas.Add(chartArea);

            var series = new Series
            {
                Name = "SpendingByCategory",
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                LabelFormat = "₱{0:N2}"
            };

            chartSummary.Series.Add(series);

            var groups = table.AsEnumerable()
                .GroupBy(row => row.Field<string>("Category"));

            foreach (var group in groups)
            {
                decimal totalAmount = group.Sum(row => decimal.TryParse(row.Field<string>("Amount"), out decimal amt) ? amt : 0);
                series.Points.AddXY(group.Key, totalAmount);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadTransactions();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
            Form expenseT = new ExpenseTracker();
            expenseT.Show();
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTransactions();
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Save transaction report",
                FileName = "TransactionReport.pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Document doc = new Document(PageSize.A4, 10, 10, 10, 10);
                    PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    doc.Open();

                    Paragraph title = new Paragraph("Transaction History", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16));
                    title.Alignment = Element.ALIGN_CENTER;
                    doc.Add(title);
                    doc.Add(new Paragraph("\n"));

                    PdfPTable table = new PdfPTable(dataGridView1.Columns.Count);
                    table.WidthPercentage = 100;

                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        table.AddCell(new Phrase(column.HeaderText, FontFactory.GetFont(FontFactory.HELVETICA, 12)));
                    }

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                string cellText = cell.Value?.ToString() ?? "";
                                table.AddCell(new Phrase(cellText, FontFactory.GetFont(FontFactory.HELVETICA, 11)));
                            }
                        }
                    }

                    doc.Add(table);
                    doc.Close();

                    MessageBox.Show("PDF Exported Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error exporting PDF: " + ex.Message);
                }
            }
        }

        private void TransactionHistoryForm_Load_1(object sender, EventArgs e) { }
        private void lblCategorySummary_Click(object sender, EventArgs e) { }
    }
}
