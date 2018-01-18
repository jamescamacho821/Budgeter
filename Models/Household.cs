using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jcamacho_budgeter1.Models
{
    public class Household
    {
        public Household()
        {
            this.Budgets = new HashSet<Budget>();
            this.Users = new HashSet<ApplicationUser>();
            this.BankAccounts = new HashSet<BankAccount>();
        }


        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalBudget { get; set; }


        //Navigation properties
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }



    }
}