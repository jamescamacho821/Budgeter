using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jcamacho_budgeter1.Models
{
    public class HouseholdViewModel
    {
        public Household Households { get; set; }
        public List<Transaction> MyTransactions { get; set; }
        public List<BankAccount> MyBankAccounts { get; set; }
        public List<Item> MyBudgetItems { get; set; }
        public int[] MyCategories { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double[] CategoryTotals { get; set; }
        public int CategoryCount { get; set; }
    }
}