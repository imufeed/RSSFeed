using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RSSFeed.Models;

namespace RSSFeed.Controllers
{
    public class RssesController : Controller
    {
        // connection to DB.
        private RssDBContext db = new RssDBContext();

        // Main page. Shows the contents of the DB. And provide search.
        // GET: Rsses
        public ActionResult Index(string searchString)
        {
            var rsses = from m in db.Rsses select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                rsses = rsses.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));                
            }

            ViewBag.SEARCHWORD = searchString;

            return View(rsses);            
        }

        // GET: Rsses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rss rss = db.Rsses.Find(id);
            if (rss == null)
            {
                return HttpNotFound();
            }
            return View(rss);
        }

        // GET: Rsses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rsses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Link,Description,Comments,PublishDate")] Rss rss)
        {
            if (ModelState.IsValid)
            {
                db.Rsses.Add(rss);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rss);
        }

        // GET: Rsses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rss rss = db.Rsses.Find(id);
            if (rss == null)
            {
                return HttpNotFound();
            }
            return View(rss);
        }

        // POST: Rsses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Link,Description,Comments,PublishDate")] Rss rss)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rss).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rss);
        }

        // GET: Rsses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rss rss = db.Rsses.Find(id);
            if (rss == null)
            {
                return HttpNotFound();
            }
            return View(rss);
        }

        // POST: Rsses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rss rss = db.Rsses.Find(id);
            db.Rsses.Remove(rss);
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
