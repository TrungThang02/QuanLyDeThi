using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuanLyDeThi.Models;
using PagedList;

namespace QuanLyDeThi.Areas.Admin.Controllers
{
    public class ExamsController : Controller
    {
        private QuanLyDeThiEntities db = new QuanLyDeThiEntities();

        // GET: Admin/Exams
        public ActionResult Index(int? page)
        {
            int iSize = 3;
            int iPageNum = (page ?? 1);
            var exam = db.Exam.Include(e => e.Class).Include(e => e.Subject).Include(e => e.Users).ToList();
            return View(exam.OrderBy(x => x.ExamID).ToPagedList(iPageNum, iSize));
        }

        // GET: Admin/Exams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exam.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // GET: Admin/Exams/Create
        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.Class, "ClassID", "ClassName");
            ViewBag.SubjectID = new SelectList(db.Subject, "SubjectID", "SubjectName");
        
            return View();
        }

        // POST: Admin/Exams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Exam exam ,HttpPostedFileBase image, HttpPostedFileBase file, FormCollection f)
        {
            ViewBag.ClassID = new SelectList(db.Class, "ClassID", "ClassName");
            ViewBag.SubjectID = new SelectList(db.Subject, "SubjectID", "SubjectName");

            if (image == null && file == null)
            {



                exam.ExamName = f["sTenSach"];
                exam.Description = f["sMoTa"].Replace("<p>", "").Replace("<p>", "\n");
                exam.Keyword = f["sKeyword"];
                exam.UploadDate = Convert.ToDateTime(f["dNgayCapNhat"]);
                exam.Author = f["tentacgia"];

                ViewBag.ClassID = new SelectList(db.Class, "ClassID", "ClassName");
                ViewBag.SubjectID = new SelectList(db.Subject, "SubjectID", "SubjectName");

                return View();

            }
            else
            {
                if (ModelState.IsValid)
                {
                    var newFile = Guid.NewGuid();
                    var renamefile = Path.GetExtension(file.FileName);
                    string newName = newFile + renamefile;
                    //lấy tên file, khai báo thư viện(System IO)

                    string sFileName2 = Path.GetFileName(newName);

                    var newImage = Guid.NewGuid();
                    var renameImage = Path.GetExtension(image.FileName);
                    string newImageName = newImage + renameImage;
                    //lấy tên file, khai báo thư viện(System IO)

                    var sFileName = Path.GetFileName(newImageName);



                    //Lấy đường dẫn lưu file
                    var path = Path.Combine(Server.MapPath("~/Content/images/AnhBaoCao"), sFileName);
                    string path2 = Path.Combine(Server.MapPath("~/File"), sFileName2);
                    file.SaveAs(path2);
                    image.SaveAs(path);

                    //Kiểm tra ảnh đã được tải lê
                    //n chưa
                    if (!System.IO.File.Exists(path))
                    {
                        image.SaveAs(path);
                        file.SaveAs(path2);
                    }



                    exam.ExamName = f["sTenSach"];
                    exam.Description = f["sMoTa"].Replace("<p>", "").Replace("<p>", "\n");
                    exam.Image = sFileName;
                    exam.UrlFile = sFileName2;
                    exam.Keyword = f["sKeyword"];
                    exam.UploadDate = Convert.ToDateTime(f["dNgayCapNhat"]);
                    exam.Author = f["tentacgia"];
                    //sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                    exam.Status = 1;
                    exam.DownloadCount = 0;
                    exam.ViewCount = 0;
                    exam.ClassID = Convert.ToInt32(f["ClassID"]);
                    exam.SubjectID = Convert.ToInt32(f["SubjectID"]);
                  
                    db.Exam.Add(exam);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View();
            }

           
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var baocao = db.Exam.SingleOrDefault(n => n.ExamID == id);
            if (baocao == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            else
            {
                ViewBag.ClassID = new SelectList(db.Class, "ClassID", "ClassName");
                ViewBag.SubjectID = new SelectList(db.Subject, "SubjectID", "SubjectName");

                return View(baocao);
            }
        }



        // POST: Admin/GraduationReporjfgkjhjdkts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase image, HttpPostedFileBase file, FormCollection f)
        {
            var exam = db.Exam.AsEnumerable().SingleOrDefault(n => n.ExamID == int.Parse(f["iMaSach"]));
            ViewBag.ClassID = new SelectList(db.Class, "ClassID", "ClassName");
            ViewBag.SubjectID = new SelectList(db.Subject, "SubjectID", "SubjectName");

            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null && image.ContentLength > 0)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var newFile = Guid.NewGuid();
                            var renamefile = Path.GetExtension(file.FileName);
                            string newName = newFile + renamefile;
                            //lấy tên file, khai báo thư viện(System IO)

                            string sFileName2 = Path.GetFileName(newName);

                            var newImage = Guid.NewGuid();
                            var renameImage = Path.GetExtension(image.FileName);
                            string newImageName = newImage + renameImage;
                            //lấy tên file, khai báo thư viện(System IO)

                            var sFileName = Path.GetFileName(newImageName);



                            //Lấy đường dẫn lưu file
                            var path = Path.Combine(Server.MapPath("~/Content/images/AnhBaoCao"), sFileName);
                            string path2 = Path.Combine(Server.MapPath("~/File"), sFileName2);
                            file.SaveAs(path2);
                            image.SaveAs(path);
                            //Kiểm tra ảnh đã được tải lên chưa
                            //Kiểm tra ảnh đã được tải lên chưa
                            if (!System.IO.File.Exists(path) && !System.IO.File.Exists(path2))
                            {
                                System.IO.File.Delete(path);
                                System.IO.File.Delete(path2);

                            }
                            else
                            {
                                image.SaveAs(path);
                                file.SaveAs(path2);
                                exam.Image = sFileName;
                                exam.UrlFile = sFileName2;
                            }

                        }


                    }
                    else
                    {
                        ViewBag.Message = "Please select a file to upload";
                    }

                    exam.ExamName = f["sTenSach"];
                    exam.Description = f["sMoTa"].Replace("<p>", "").Replace("<p>", "\n");
                    exam.Author = f["tentacgia"];

                    exam.Keyword = f["sKeyword"];
                    exam.UploadDate = Convert.ToDateTime(f["dNgayCapNhat"]);
                    //sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);

                    exam.Status = 1;
                    exam.ClassID = Convert.ToInt32(f["ClassID"]);
                    exam.SubjectID = Convert.ToInt32(f["SubjectID"]);

                    db.SaveChanges();


                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    ViewBag.Thongbao = "Chưa chọn file";
                }



            }
            return View();
        }



        public ActionResult Delete(int id)
        {
            Exam exam = db.Exam.Find(id);
            db.Exam.Remove(exam);
            db.SaveChanges();
            return RedirectToAction("Index", "Admin/Exams", "Areas");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult UnapprovedReports()
        {
            var reports = db.Exam.Where(r => r.Status == 0).ToList();
            return View(reports);
        }

        public ActionResult ApproveDocument(int id)
        {
            var report = db.Exam.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            report.Status = 1;
            db.Entry(report).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("UnapprovedReports");
        }
    }
}
