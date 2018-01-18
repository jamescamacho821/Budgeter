using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jcamacho_budgeter1.Models;
using Microsoft.AspNet.Identity;

namespace jcamacho_budgeter1.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            HouseholdViewModel model = new HouseholdViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            model.Households = db.Households.Find(id);
            if (model.Households == null)
            {
                return HttpNotFound();
            }

            model.Households.Total = 0;

            foreach (var bankAccount in model.Households.BankAccounts)
            {
                if (bankAccount.IsDeleted != true)
                {
                    if (bankAccount.Balance <= 0)
                    {
                        ViewBag.OverdraftError = "You are in danger of overdrawing your account!";
                    }

                    var balance = bankAccount.Balance;
                    model.Households.Total += balance;
                    //total = total + balance;
                }
            }

            model.Households.TotalBudget = 0;

            foreach (var budget in model.Households.Budgets)
            {
                foreach (var item in budget.Items)
                {
                    var amount = item.Amount;
                    model.Households.TotalBudget += amount;
                }
            }

            int catTotal = 0;

            foreach (var category in db.Categories)
            {
                catTotal++;
            }

            int catCount = 0;
            model.MyCategories = new int[catTotal ];
//            model.MyCategories = new int[catTotal - 1];

            foreach (var budget in model.Households.Budgets)
            {
                foreach (var item in budget.Items)
                {
                    model.MyCategories[catCount] = item.Category.Id;
                    catCount++;
                }
            }

            model.CategoryCount = catCount;

            //double categoryTotal = 0;
            int transCount = 0;
            //model.CategoryTotals = new double[catTotal - 1];
            model.CategoryTotals = new double[catTotal];
            foreach (var category in model.MyCategories)
            {
                foreach (var account in model.Households.BankAccounts)
                {
                    foreach (var transaction in account.Transactions)
                    {
                        if (transaction.CategoryId == category)
                        {
                            //model.MyTransactions.Add(transaction);
                            //model.CategoryTotals += transaction.Amount;
                            model.CategoryTotals[transCount] += transaction.Amount;
                        }
                    }
                }
                transCount++;
            }

            db.SaveChanges();
            return View(model);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, Name, CreatedById, IsDeleted")] Household household)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                household.CreatedById = userId;
                db.Households.Add(household);

                HouseholdUsersHelper helper = new HouseholdUsersHelper(db);
                helper.AddUserToHousehold(household.Id, userId);

                Budget budget = new Budget
                {
                    HouseholdId = household.Id,
                    Household = db.Households.Find(household.Id),
                    Name = household.Name + "'s Budget",
                    Amount = 0
                };
                db.Budgets.Add(budget);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View(household);
        }

        // GET: Households/Assign/5
        public ActionResult Assign(int id)
        {
            var household = db.Households.Find(id);
            HouseholdUsersHelper helper = new HouseholdUsersHelper(db);
            var model = new AssignUsersViewModel();

            model.Household = household;
            model.SelectedUsers = helper.ListAssignedUsers(id).ToArray();
            model.Users = new MultiSelectList(model.SelectedUsers.Where(u => (u.DisplayName != "N/A" && u.DisplayName != "(Remove Assigned User)")).OrderBy(u => u.FirstName), "Id", "DisplayName", model.SelectedUsers);
            //model.Users = new MultiSelectList(db.Users.Where(u => (u.DisplayName != "N/A" && u.DisplayName != "(Remove Assigned User)")).OrderBy(u => u.FirstName), "Id", "DisplayName", model.SelectedUsers);

            return View(model);
        }

        // POST: Households/Assign/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Assign(AssignUsersViewModel model)
        {
            var household = db.Households.Find(model.Household.Id);
            HouseholdUsersHelper helper = new HouseholdUsersHelper(db);

            //foreach (var user in db.Users.Select(r => r.Id).ToList())
            //{
            //    helper.RemoveUserFromHousehold(household.Id, user);
            //}
            foreach (var user in db.Users.Select(r => r.Id).ToList())
            {
                if (model.SelectedUsers != null)
                {
                    foreach (var item in model.SelectedUsers)
                    {
                        helper.RemoveUserFromHousehold(household.Id, item.Id);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Households/UnAssign/5
        public ActionResult UnAssign(int householdId, string userId)
        {
            //var household = db.Households.Find(householdId);
            //var user = db.Users.Find(userId);
            //HouseholdUsersHelper helper = new HouseholdUsersHelper(db);
            var model = new AssignUsersViewModel();

            ViewBag.HouseholdId = householdId;
            ViewBag.UserId = userId;

            return View(model);
        }

        // POST: Households/UnAssign/5
        [HttpPost]
        public ActionResult UnAssign(AssignUsersViewModel model, int householdId, string userId)
        {
            if (ModelState.IsValid)
            {
                var household = db.Households.Find(householdId);
                var user = db.Users.Find(userId);
                HouseholdUsersHelper helper = new HouseholdUsersHelper(db);

                helper.RemoveUserFromHousehold(household.Id, user.Id);
                return RedirectToAction("Assign", "Households", new { id = household.Id });
            }
            return View(model);
        }


        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CreatedById,IsDeleted")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = household.Id });
            }
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            household.IsDeleted = true;
            db.Entry(household).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // GET: Households/Leave/5
        public ActionResult Leave(int? id)
        {
            Household household = db.Households.Find(id);
            ViewBag.HouseholdName = household.Name;
            ViewBag.HouseholdId = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Leave/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Leave(int id)
        {
            var user = User.Identity.GetUserId();
            HouseholdUsersHelper helper = new HouseholdUsersHelper(db);
            helper.RemoveUserFromHousehold(id, user);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

