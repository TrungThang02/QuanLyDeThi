using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyDeThi.Models;
using PagedList;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace QuanLyDeThi.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        QuanLyDeThiEntities db = new QuanLyDeThiEntities();
        UserController Users = new UserController();
        public ActionResult Index(int? page)
        {


            int iSize = 12;
            int iPageNum = (page ?? 1);

            var d = (from t in db.Exam select t).OrderBy(t => t.UploadDate).Where(r => r.Status == 1);


            return View(d.OrderBy(s => s.ExamID).ToPagedList(iPageNum, iSize));
        }

        public ActionResult Error()
        {
          

            return View();
        }

        public ActionResult About()
        {
      
            return View();
        }
        public ActionResult Class()
        {
            var detai = from dt in db.Class select dt;
            ViewBag.detai = detai;

            return PartialView();
        }
        public ActionResult Subject()
        {
            var detai = from dt in db.Subject select dt;
            ViewBag.detai = detai;
            return PartialView();
        }




        public ActionResult DeThiTheoMonHoc(int? id)
        {
            var fa = from s in db.Exam
                     join c in db.Subject on s.SubjectID equals c.SubjectID
                     where s.SubjectID == id
                     select s;

            var sl = fa.Count();
            string tenMonHoc = db.Subject.FirstOrDefault(s => s.SubjectID == id)?.SubjectName;
            ViewBag.tenMonHoc = tenMonHoc;
            ViewBag.sl = sl;

          
            if (sl == 0)
            {
                return View("Error");
            }
          
            return View(fa.ToList().Where(r => r.Status == 1));
        }


        public ActionResult DeThiTheoLop(int? id)
        {
            var fa = from s in db.Exam
                     join c in db.Class on s.ClassID equals c.ClassID
                     where s.SubjectID == id
                     select s;

            var sl = fa.Count();
            string tenLophoc = db.Class.FirstOrDefault(s => s.ClassID == id)?.ClassName;
            ViewBag.tenMonHoc = tenLophoc;
            ViewBag.sl = sl;
            if (sl == 0)
            {
                return View("Error");
            }
            return View(fa.ToList().Where(r => r.Status == 1));
        }



        public ActionResult Search(string search = "")
        {
           
            List<Exam> products = db.Exam
                                    .Where(p => p.ExamName.Contains(search) || p.Keyword.Contains(search))
                                    .ToList();

            ViewBag.search = search;
            var sl = products.Count();
            ViewBag.sl = sl;

            return View(products.Where(r => r.Status == 1));
        }



        public ActionResult SearchCategory(string searchString, int categoryID = 0, int year = 0)
        {
            
            ViewBag.tukhoa = searchString;
            var tk = from d in db.Exam select d;
            if (!String.IsNullOrEmpty(searchString))
            {
                tk = tk.Where(b => b.Keyword.Contains(searchString) ||
                                   b.ExamName.Contains(searchString) ||
                                   b.Author.Contains(searchString));
            }

            if (categoryID != 0)
            {
                tk = tk.Where(c => c.SubjectID == categoryID);
            }
            ViewBag.CategoryID = new SelectList(db.Subject, "SubjectID", "SubjectName"); 
            ViewBag.ClassID = new SelectList(db.Class, "ClassID", "ClassName"); 

            var sl = tk.Count();
            ViewBag.sl = sl;

            return View(tk.ToList().Where(r => r.Status == 1));
        }



        public ActionResult K()
        {
            ViewBag.ClassID = new SelectList(db.Class, "ClassID", "ClassName");// danh sách Category     
            ViewBag.CategoryID = new SelectList(db.Subject, "SubjectID", "SubjectName"); // dan
            return PartialView();
        }

        //public ActionResult BaoCaoGanDay()
        //{
        //    var d = from t in db.GraduationReport select t;
        //    return PartialView(d.ToList().Take(4));
        //}

        //public ActionResult Khongcodulieu()
        //{
        //    return View();
        //}


        public ActionResult CheckTypeFile(int? id)
        {
            string myString = db.Exam.FirstOrDefault(f => f.ExamID == id)?.UrlFile;
            if (myString.Contains("pdf"))
            {
                return Redirect("/Home/XemBaoCao/" + id);
            }
            else if (myString.Contains("docx"))
            {
                return Redirect("/Home/View/" + id);

            }
            else
            {
                return View();
            }
        }

        public ActionResult XemBaoCao(int? id)
        {
            //if (ModelState.IsValid)
            //{

            var document = db.Exam.Find(id);
            document.ViewCount++;
            db.SaveChanges();

            string currentUrl = Request.Url.AbsoluteUri;
            ViewBag.CurrentUrl = currentUrl;

            var dt = from s in db.Exam where s.ExamID == id select s;

            return View(dt.Single());


        }



        public ActionResult View(int? id)
        {

            var document = db.Exam.Find(id);
            document.ViewCount++;
            db.SaveChanges();
            string currentUrl = Request.Url.AbsoluteUri;
            ViewBag.CurrentUrl = currentUrl;
            var dt = from s in db.Exam where s.ExamID == id select s;
            return View(dt.Single());
        }


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
        public ActionResult Create(Exam exam, HttpPostedFileBase image, HttpPostedFileBase file, FormCollection f)
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
                exam.ID = Convert.ToInt32(f["UserID"]);
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
                    exam.ID = Convert.ToInt32(f["UserID"]);
                    //sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                    exam.Status = 0;
                    exam.DownloadCount = 0;
                    exam.ViewCount = 0;
                    exam.ClassID = Convert.ToInt32(f["ClassID"]);
                    exam.SubjectID = Convert.ToInt32(f["SubjectID"]);

                    db.Exam.Add(exam);
                    db.SaveChanges();

                    return Redirect("/User/Info/" + Session["TaiKhoan3"]);



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
                        //exam.ID = Convert.ToInt32(f["UserID"]);
                        exam.Status = 0;
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



        public ActionResult Delete(int? id)
        {
            Exam graduationReport = db.Exam.Find(id);
            db.Exam.Remove(graduationReport);
            db.SaveChanges();
            return Redirect("/User/Info/" + Session["TaiKhoan3"]);
        }

        public ActionResult DownloadFile(string filePath)
        {
            var document = db.Exam.FirstOrDefault(x => x.UrlFile == filePath);

            document.DownloadCount++;
            db.SaveChanges();

            string fullName = Server.MapPath("~/File/" + filePath);

            int layduoifile = filePath.LastIndexOf('.');
            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, document.ExamName + "." + filePath.Substring(layduoifile + 1));
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        public ActionResult BaoCaoXemNhieu()
        {
            var bc = (from s in db.Exam select s).Where(r => r.Status == 1);
            return PartialView(bc.OrderByDescending(n => n.ViewCount).ToList().Take(8));
        }

        public ActionResult BaoCaoTaiNhieu()
        {
            var bc = (from s in db.Exam select s).Where(r => r.Status == 1);
            return PartialView(bc.OrderByDescending(n => n.DownloadCount).ToList().Take(8));
        }

        public ActionResult DeThiMoiNhat()
        {
            var bc = (from s in db.Exam select s).Where(r => r.Status == 1);
            return PartialView(bc.OrderByDescending(n => n.UploadDate).Take(8).ToList());
        }

    }
}