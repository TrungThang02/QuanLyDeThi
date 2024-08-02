using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyDeThi.Models;

namespace QuanLyDeThi.Areas.Admin.Controllers
{
    public class HomesController : Controller
    {
        QuanLyDeThiEntities db = new QuanLyDeThiEntities();
        // GET: Admin/Homes

        public ActionResult Index()
        {
            var tsbc = (from s in db.Exam select s).Count();
            ViewBag.tongbaocao = tsbc;


            var tstk = (from s in db.Users select s).Count();
            ViewBag.tongtaikhoan = tstk;

            int totalViews = (int)db.Exam.Sum(r => r.ViewCount);
            ViewBag.totalView = totalViews;

            int totalDowns = (int)db.Exam.Sum(r => r.DownloadCount);
            ViewBag.totalDown = totalDowns;
            if (Session["ADMIN"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Erorr", "Home");
            }


        }
    }
}