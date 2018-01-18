using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jcamacho_budgeter1.Models;

namespace jcamacho_budgeter1.Controllers
{
    public class ItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Items
        public ActionResult Index()
        {
            var items = db.Items.Include(i => i.Budget).Include(i => i.Category);
            return View(items.ToList());
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public ActionResult Create(int householdId, int budgetId)
        {
            Budget budget = db.Budgets.Find(budgetId);
            List<Item> budgetItems = db.Items.Where(i => i.BudgetId == budgetId).ToList();
            List<Category> allCats = db.Categories.Where(c=>c.Name != "Deposit").ToList();
            List<Category> usedCats = new List<Category>();
            List<Category> listCats = db.Categories.Where(c => c.Name != "Deposit").ToList();

            foreach (Category category in allCats)
            {
                foreach (Item budgetItem in budgetItems)
                {
                    if (category.Id == budgetItem.CategoryId)
                    {
                        listCats.Remove(category);
                    }
                }
            }

            ViewBag.HouseholdId = householdId;
            ViewBag.CategoryId = new SelectList(listCats.Where(c => c.Name != "Deposit"), "Id", "Name");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryId,BudgetId,Amount,IsDeleted")] Item item, int householdId, int budgetId)
        {
            if (ModelState.IsValid)
            {
                item.BudgetId = budgetId;
                var budget = db.Budgets.Find(item.BudgetId);
                var category = db.Categories.Find(item.CategoryId);
                db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Details", "Households", new { id = householdId });
            }

            ViewBag.BudgetId = budgetId;
            ViewBag.HouseholdId = householdId;
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", item.BudgetId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,BudgetId,Amount,IsDeleted")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", item.BudgetId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
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