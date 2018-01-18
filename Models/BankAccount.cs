using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jcamacho_budgeter1.Models
{
    public class BankAccount
    {
        public BankAccount()
        {
            this.Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Balance { get; set; }
        public bool IsDeleted { get; set; }

        //Navigation
        public virtual Household Household { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}