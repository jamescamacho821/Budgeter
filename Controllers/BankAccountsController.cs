using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jcamacho_budgeter1.Models;

namespace Budgeter.Controllers
{
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        public ActionResult Index()
        {
            var bankAccounts = db.BankAccounts.Include(b => b.Household);
            foreach (var bankAccount in bankAccounts)
            {
                if (bankAccount.Balance <= 0)
                {
                    ViewBag.OverdraftError = "You are in danger of overdrawing your account!";
                }
            }
            return View(bankAccounts.ToList());
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public ActionResult Create(int id)
        {
            ViewBag.HouseholdId = id;
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Name,Balance,IsDeleted")] BankAccount bankAccount, int id)
        {
            if (ModelState.IsValid)
            {
                bankAccount.HouseholdId = id;
                bankAccount.IsDeleted = false;
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("Details", "Households", new { id = bankAccount.HouseholdId });
            }

            ViewBag.HouseholdId = id;
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int householdId, int accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(accountId);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = householdId;
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name,Balance,IsDeleted")] BankAccount bankAccount, int householdId)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Households", new { id = householdId });
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            int householdId = bankAccount.HouseholdId;
            bankAccount.IsDeleted = true;
            db.Entry(bankAccount).State = EntityState.Modified;
            db.SaveChanges();
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