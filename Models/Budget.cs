using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jcamacho_budgeter1.Models
{
    public class Budget
    {
        public Budget()
        {
            this.Items = new HashSet<Item>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int HouseholdId { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Amount { get; set; }
        public virtual Household Household { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}