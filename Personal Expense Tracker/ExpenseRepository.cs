using System.Collections.Generic;

namespace Personal_Expense_Tracker
{
    public static class ExpenseRepository
    {
        public static List<Expense> Expenses { get; } = new List<Expense>();
    }
}
