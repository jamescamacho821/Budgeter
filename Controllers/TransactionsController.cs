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

namespace Budgeter.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Where(t => t.IsDeleted != false).Include(t => t.BankAccount).Include(t => t.Category).Include(t => t.Type);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int householdId, int? transactionId)
        {
            ViewBag.HouseholdId = householdId;
            if (transactionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(transactionId);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Deposit
        public ActionResult Deposit(int householdId, int accountId)
        {
            //ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.HouseholdId = householdId;
            ViewBag.BankAccountId = accountId;
            //ViewBag.CategoryId = 1;
            //ViewBag.TypeId = 1;
            ViewBag.EnteredbyId = User.Identity.GetUserId();
            return View();
        }

        // POST: Transactions/Deposit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit([Bind(Include = "Id,BankAccountId,Payee,Description,Created,Amount,IsDeleted,TypeId,CategoryId")] Transaction transaction, int householdId, int accountId)
        {
            
            if (ModelState.IsValid)
            {
                transaction.BankAccountId = accountId;
                var bankAccount = db.BankAccounts.Find(transaction.BankAccountId);
                var enteredBy = db.Users.Find(User.Identity.GetUserId());
                if (bankAccount != null)
                {
                    transaction.BankAccount = bankAccount;
                    transaction.EnteredBy = enteredBy;// ViewBag.EnteredbyId;
                    transaction.CategoryId = 9; //Bank - Deposit
                    transaction.TypeId = 1;
                    transaction.Created = DateTimeOffset.Now;
                    transaction.IsDeleted = false;
                    transaction.Payee = "Bank - Deposit";
                    transaction.BankAccount.Balance += transaction.Amount;
                }
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Details", "Households", new { id = householdId });
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", transaction.TypeId);
            ViewBag.HouseholdId = householdId;
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create(int? householdId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            Household household = db.Households.Find(householdId);
            var ownAccounts = context.BankAccounts.Where(b => b.Household.Id == household.Id);

            ViewBag.HouseholdId = householdId;
            ViewBag.BankAccountId = new SelectList(ownAccounts, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories.Where(c => c.Name != "Deposit"), "Id", "Name");
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name");
            ViewBag.EnteredbyId = User.Identity.GetUserId();
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BankAccountId,Payee,Description,Created,Amount,IsDeleted,TypeId,CategoryId,EnteredById")] Transaction transaction, int householdId)
        {
            if (ModelState.IsValid)
            {
                var enteredBy = db.Users.Find(User.Identity.GetUserId());

                var bankAccount = db.BankAccounts.Find(transaction.BankAccountId);
                if (bankAccount != null)
                {
                    transaction.BankAccount = bankAccount;
                    transaction.EnteredBy = enteredBy;
                    transaction.TypeId = 2;
                    transaction.Created = DateTimeOffset.Now;
                    transaction.IsDeleted = false;
                    transaction.BankAccount.Balance -= transaction.Amount;
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Households", new { id = householdId });
                }
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", transaction.TypeId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int householdId, int? transactionId)
        {
            if (transactionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(transactionId);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            //transaction.BankAccount = db.BankAccounts.Find(transaction.BankAccountId);
            transaction.BankAccount = db.BankAccounts.Find(transaction.BankAccountId);
            Household household = db.Households.Find(householdId);

            ViewBag.OriginalAmount = transaction.Amount;
            ViewBag.OriginalAccountid = transaction.BankAccountId;
            ViewBag.HouseholdId = householdId;
            ViewBag.BankAccountId = new SelectList(household.BankAccounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories.Where(c => c.Name != "Deposit"), "Id", "Name", transaction.CategoryId);
            //ViewBag.TypeId = transaction.TypeId;
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankAccountId,Payee,Description,Created,Amount,IsDeleted,TypeId,CategoryId,EnteredById")] Transaction transaction, int householdId, double originalAmount, int originalAccountId)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                transaction.BankAccount = db.BankAccounts.Find(transaction.BankAccountId);
                var oldBankAccount = db.BankAccounts.Find(originalAccountId);

                if (transaction.IsDeleted == true)
                {
                    if (transaction.TypeId == 1) // Deposit
                    {
                        oldBankAccount.Balance = oldBankAccount.Balance - originalAmount;
                    }
                    else // Withdrawal
                    {
                        oldBankAccount.Balance = oldBankAccount.Balance + originalAmount;
                    }
                }

                else if (originalAccountId != transaction.BankAccountId || originalAmount != transaction.Amount) // If the bank account is changed or the amount is changed
                {
                    if (transaction.TypeId == 1) // Deposit
                    {
                        oldBankAccount.Balance = oldBankAccount.Balance - originalAmount;
                        transaction.BankAccount.Balance = transaction.BankAccount.Balance + transaction.Amount;
                    }
                    else // Withdrawal
                    {
                        oldBankAccount.Balance = oldBankAccount.Balance + originalAmount;
                        transaction.BankAccount.Balance = transaction.BankAccount.Balance - transaction.Amount;
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Details", "Households", new { id = householdId });
            }
            ViewBag.HouseholdId = householdId;
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            //ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", transaction.TypeId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int householdId, int? transactionId)
        {
            if (transactionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(transactionId);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = householdId;
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int householdId, int transactionId)
        {
            Transaction transaction = db.Transactions.Find(transactionId);
            transaction.IsDeleted = true;
            db.Entry(transaction).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.HouseholdId = householdId;
            return RedirectToAction("Details", "Households", new { id = householdId });
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