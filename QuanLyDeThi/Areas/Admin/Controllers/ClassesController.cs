using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuanLyDeThi.Models;

namespace QuanLyDeThi.Areas.Admin.Controllers
{
    public class ClassesController : Controller
    {
        private QuanLyDeThiEntities db = new QuanLyDeThiEntities();

        // GET: Admin/Classes
        public ActionResult Index()
        {
            return View(db.Class.ToList());
        }

        // GET: Admin/Classes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Class.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // GET: Admin/Classes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassID,ClassName")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Class.Add(@class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@class);
        }

        // GET: Admin/Classes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Class.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Admin/Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassID,ClassName")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@class).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@class);
        }

     

     
        public ActionResult Delete(int id)
        {
            Class @class = db.Class.Find(id);
            db.Class.Remove(@class);
            db.SaveChanges();
            return RedirectToAction("Index", "Admin/Classes", "Areas");
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
