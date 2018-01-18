using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jcamacho_budgeter1.Models
{
    public class Item
    {
        // Budget Item (category)
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int BudgetId { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Amount { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Category Category { get; set; }
        public virtual Budget Budget { get; set; }
    }
}